using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Be.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace D3_Sqex03DataMessage
{
    class Sqex03DataMessage
    {
        public static byte[] Decompress(byte[] input, uint offset)
        {
            MemoryStream stream = new MemoryStream(input);
            BinaryReaderBE reader = new BinaryReaderBE(stream);
            reader.BaseStream.Position = offset;
            UInt32 chunkCount = reader.ReadUInt32();
            long position = reader.BaseStream.Position;
            List<CompressedChunk> totalChunk = new List<CompressedChunk>();
            for (int i = 0; i < chunkCount; i++)
            {
                reader.BaseStream.Position = position;
                UInt32 uncompressedOffset = reader.ReadUInt32();
                UInt32 uncompressedSize = reader.ReadUInt32();
                UInt32 compressedOffset = reader.ReadUInt32();
                UInt32 compressedSize = reader.ReadUInt32();
                position = reader.BaseStream.Position;
                reader.BaseStream.Position = compressedOffset;
                byte[] compressedData = reader.ReadBytes((int)compressedSize);
                CompressedChunk chunk = new CompressedChunk(uncompressedOffset, uncompressedSize, compressedOffset, compressedSize);
                chunk.Decompress(compressedData);
                totalChunk.Add(chunk);
            }
            MemoryStream result = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(result);
            long unknowPosition = position;
            reader.BaseStream.Position = 0;
            writer.Write(reader.ReadBytes((int)ArchiveConfig.PackageFlagsOffset));
            writer.Write(ArchiveConfig.UncompressedFlagsBytes);
            reader.BaseStream.Position += 4;
            writer.Write(reader.ReadBytes((int)(ArchiveConfig.CompressionTypeOffset - ArchiveConfig.PackageFlagsOffset - 4)));
            writer.Write(new byte[8]);
            reader.BaseStream.Position = unknowPosition;
            writer.Write(reader.ReadBytes(8));
            position = ArchiveConfig.CompressionTypeOffset + 16;
            foreach (CompressedChunk chunk in totalChunk)
            {
                if (chunk.UncompressedOffset > position)
                {
                    long zeroes = chunk.UncompressedOffset - position;
                    writer.Write(new byte[zeroes]);
                    position = chunk.UncompressedOffset;
                }
                writer.Write(chunk.UncompressedData);
                position += chunk.UncompressedSize;
            }
            writer.Close();
            reader.Close();
            return result.ToArray();
        }

        public static List<DataMessage> Decrypt(byte[] input)
        {
            List<DataMessage> result = new List<DataMessage>();
            MemoryStream stream = new MemoryStream(input);
            BinaryReaderBE reader = new BinaryReaderBE(stream);
            if (reader.ReadUInt32() == ArchiveConfig.Signature)
            {
                reader.BaseStream.Position = ArchiveConfig.CompressionTypeOffset;
                UInt32 compressType = reader.ReadUInt32();
                switch (compressType)
                {
                    case 0:
                        reader.BaseStream.Position = ArchiveConfig.TableOffset;

                        UInt32 nameCount = reader.ReadUInt32();
                        UInt32 nameOffset = reader.ReadUInt32();
                        UInt32 exportCount = reader.ReadUInt32();
                        UInt32 exportOffset = reader.ReadUInt32();
                        UInt32 importCount = reader.ReadUInt32();
                        UInt32 importOffset = reader.ReadUInt32();

                        string[] nameTable = new string[nameCount];
                        reader.BaseStream.Position = nameOffset;
                        for (int i = 0; i < nameCount; i++)
                        {
                            UInt32 strLength = reader.ReadUInt32();
                            nameTable[i] = Encoding.UTF8.GetString(reader.ReadBytes((int)strLength - 1));
                            reader.BaseStream.Position += 9;
                        }
                        string[] nameImport = new string[importCount];
                        reader.BaseStream.Position = importOffset;
                        for (int i = 0; i < importCount; i++)
                        {
                            reader.BaseStream.Position += 20;
                            UInt32 index = reader.ReadUInt32();
                            nameImport[i] = nameTable[(int)index];
                            reader.BaseStream.Position += 4;
                        }
                        reader.BaseStream.Position = exportOffset;
                        long current = reader.BaseStream.Position;
                        for (int i = 0; i < exportCount; i++)
                        {
                            reader.BaseStream.Position = current;
                            uint indexType = (uint)reader.ReadInt32() ^ 0xFFFFFFFF;
                            reader.BaseStream.Position += 8;
                            UInt32 nameIndex = reader.ReadUInt32();
                            string name = nameTable[(int)nameIndex];
                            reader.BaseStream.Position += 16;
                            UInt32 size = reader.ReadUInt32();
                            UInt32 offset = reader.ReadUInt32();
                            reader.BaseStream.Position += 4;
                            if (reader.ReadUInt32() > 0) reader.BaseStream.Position += 4;
                            reader.BaseStream.Position += 20;
                            current = reader.BaseStream.Position;
                            if (indexType == 2) //Sqex03DataMessage
                            {
                                reader.BaseStream.Position = offset;
                                UInt32 order = reader.ReadUInt32();
                                if (ArchiveConfig.Skip.Contains(order)) continue;
                                reader.BaseStream.Position += 68;
                                UInt64 unknowDataLength = reader.ReadUInt64();
                                reader.BaseStream.Position += (long)unknowDataLength + 16 + 8;
                                List<string> strings = new List<string>();
                                UInt64 totalLines = reader.ReadUInt64();
                                for (int line = 0; line < (long)totalLines; line++)
                                {
                                    long strLength = reader.ReadInt32();
                                    
                                    int zeroBytes = 1;
                                    if (strLength < 0)
                                    {
                                        strLength = (strLength^0xFFFFFFFF)*2;
                                        zeroBytes = 2;
                                    }
                                    
                                    string str = zeroBytes < 2 ? Encoding.GetEncoding(1252).GetString(reader.ReadBytes((int)strLength - zeroBytes)) : Encoding.Unicode.GetString(reader.ReadBytes((int)strLength));
                                    for (int k = 0; k < ArchiveConfig.OriginalChars.Length; k++)
                                    {
                                        str = str.Replace(ArchiveConfig.OriginalChars[k], ArchiveConfig.ReplaceChars[k]);
                                    }
                                    strings.Add(str);
                                    reader.BaseStream.Position += zeroBytes;
                                }
                                DataMessage dataMessage = new DataMessage(name, (int)order, strings);
                                result.Add(dataMessage);
                            }
                        }
                        break;
                    case 1:
                        byte[] decompressedData = Decompress(input, (uint)reader.BaseStream.Position);
                        return Decrypt(decompressedData);
                    default:
                        throw new Exception("The file has an unsupported compression type.");
                }
            }
            else
            {
                throw new Exception("The file is not a Drakengard 3 (Unreal 3) file.");
            }
            reader.Close();
            return result;
        }

        public static byte[] ReImport(List<DataMessage> input, string file)
        {
            Dictionary<UInt32, List<string>> strings = new Dictionary<uint, List<string>>();
            foreach (DataMessage data in input)
            {
                strings.Add((uint)data.Index, data.Strings);
            }
            byte[] originalFile = File.ReadAllBytes(file);
            byte[] result = Reimport(originalFile, strings);
            return result;
        }

        /*public static void ReImport (byte[] input, string file, ArchiveConfig config)
        {
            
        }*/

        private static byte[] Reimport (byte[] input, Dictionary<UInt32, List<string>> data)
        {
            MemoryStream result = new MemoryStream();
            MemoryStream inputStream = new MemoryStream(input);
            BinaryReaderBE reader = new BinaryReaderBE(inputStream);
            BeBinaryWriter writer = new BeBinaryWriter(result);

            if (reader.ReadUInt32() == ArchiveConfig.Signature)
            {
                reader.BaseStream.Position = ArchiveConfig.CompressionTypeOffset;
                UInt32 compressType = reader.ReadUInt32();
                switch (compressType)
                {
                    case 0:
                        reader.BaseStream.Position = ArchiveConfig.TableOffset;

                        UInt32 nameCount = reader.ReadUInt32();
                        UInt32 nameOffset = reader.ReadUInt32();
                        UInt32 exportCount = reader.ReadUInt32();
                        UInt32 exportOffset = reader.ReadUInt32();
                        UInt32 importCount = reader.ReadUInt32();
                        UInt32 importOffset = reader.ReadUInt32();

                        reader.BaseStream.Position = 0;
                        writer.Write(reader.ReadBytes((int)exportOffset)); //header
                        
                        long bytesChanged = 0;
                        List<byte[]> newData = new List<byte[]>();
                        long current = exportOffset;
                        for (int i = 0; i < exportCount; i++)
                        {
                            MemoryStream newRealData = new MemoryStream();
                            BeBinaryWriter writerData = new BeBinaryWriter(newRealData);
                            reader.BaseStream.Position = current;
                            long start = reader.BaseStream.Position;
                            uint indexType = (uint)reader.ReadInt32() ^ 0xFFFFFFFF;
                            reader.BaseStream.Position += 28;
                            UInt32 size = reader.ReadUInt32();
                            UInt32 offset = reader.ReadUInt32();                            
                            long newOffset = offset + bytesChanged;
                            reader.BaseStream.Position += 4;
                            if (reader.ReadUInt32() > 0) reader.BaseStream.Position += 4;
                            reader.BaseStream.Position += 20;
                            long headerLength = reader.BaseStream.Position - start;
                            current = reader.BaseStream.Position;
                            reader.BaseStream.Position = offset;
                            UInt32 order = reader.ReadUInt32();

                            reader.BaseStream.Position = offset;
                            List<string> strings;
                            long stringBytesChanged = 0;
                            
                            if (indexType == 2 && !ArchiveConfig.Skip.Contains(order) && data.TryGetValue(order, out strings))
                            {
                                writerData.Write(reader.ReadBytes(72));
                                UInt64 unknowDataLength = reader.ReadUInt64();
                                writerData.Write(unknowDataLength);
                                writerData.Write(reader.ReadBytes((int)unknowDataLength + 16));
                                UInt64 oldSize = reader.ReadUInt64();
                                UInt64 totalLines = reader.ReadUInt64();
                                int newStrLength = 0;
                                MemoryStream temp = new MemoryStream();
                                BeBinaryWriter writerTemp = new BeBinaryWriter(temp);
                                for (int line = 0; line < (long)totalLines; line++)
                                {
                                    string lineStr = strings[line];
                                    for (int k = 0; k < ArchiveConfig.OriginalChars.Length; k++)
                                    {
                                        lineStr = lineStr.Replace(ArchiveConfig.ReplaceChars[k], ArchiveConfig.OriginalChars[k]);
                                    }
                                    long strLength = lineStr.Length ^ 0xFFFFFFFF;
                                    writerTemp.Write((Int32)strLength);
                                    byte[] str = Encoding.Unicode.GetBytes(lineStr);
                                    writerTemp.Write(str);
                                    writerTemp.Write(new byte[2]);
                                    newStrLength += 4 + str.Length + 2;
                                }
                                writerTemp.Close();
                                newStrLength += 4;
                                stringBytesChanged = (long)newStrLength - (long)oldSize;
                                writerData.Write((UInt64)newStrLength);
                                writerData.Write(totalLines);
                                writerData.Write(temp.ToArray());
                                reader.BaseStream.Position += ((long)oldSize - 4);
                                writerData.Write(reader.ReadBytes((int)size - 72 - (int)unknowDataLength - 36 - (int)oldSize));
                                
                            }
                            else
                            {
                                writerData.Write(reader.ReadBytes((int)size));
                            }
                            writerData.Close();
                            newData.Add(newRealData.ToArray());
                            reader.BaseStream.Position = start;
                            writer.Write(reader.ReadBytes(32));
                            if (stringBytesChanged != 0)
                            {
                                writer.Write((UInt32)(size + stringBytesChanged));
                                bytesChanged += stringBytesChanged;
                                reader.BaseStream.Position += 4;
                            }
                            else
                            {
                                writer.Write(reader.ReadBytes(4));
                            }
                            writer.Write((UInt32)newOffset);
                            reader.BaseStream.Position += 4;
                            writer.Write(reader.ReadBytes((int)headerLength - 40));
                        }
                        writer.Write(new byte[ArchiveConfig.TableToData]);
                        foreach (byte[] data_message in newData)
                        {
                            writer.Write(data_message);
                        }
                        writer.Write(new byte[4]);
                        break;
                    case 1:
                        byte[] decompressedData = Decompress(input, (uint)reader.BaseStream.Position);
                        return Reimport(decompressedData, data);
                    default:
                        throw new Exception("The file has an unsupported compression type.");
                }
            }
            else
            {
                throw new Exception("The file is not a Drakengard 3 (Unreal 3) file.");
            }
            reader.Close();
            writer.Close();
            return result.ToArray();
        }
    }
}
