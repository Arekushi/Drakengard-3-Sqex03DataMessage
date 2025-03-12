using Be.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace D3_Sqex03DataMessage
{
    class Sqex03DataMessage
    {
        public static byte[] Decompress(byte[] input, uint offset)
        {
            MemoryStream stream = new MemoryStream(input);
            BinaryReaderBE reader = new BinaryReaderBE(stream);
            reader.BaseStream.Position = offset;
            uint chunkCount = reader.ReadUInt32();
            long position = reader.BaseStream.Position;
            List<CompressedChunk> totalChunk = new List<CompressedChunk>();
            for (int i = 0; i < chunkCount; i++)
            {
                reader.BaseStream.Position = position;
                uint uncompressedOffset = reader.ReadUInt32();
                uint uncompressedSize = reader.ReadUInt32();
                uint compressedOffset = reader.ReadUInt32();
                uint compressedSize = reader.ReadUInt32();
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
                uint compressType = reader.ReadUInt32();
                switch (compressType)
                {
                    case 0:
                        reader.BaseStream.Position = ArchiveConfig.TableOffset;

                        uint nameCount = reader.ReadUInt32();
                        uint nameOffset = reader.ReadUInt32();
                        uint exportCount = reader.ReadUInt32();
                        uint exportOffset = reader.ReadUInt32();
                        uint importCount = reader.ReadUInt32();
                        uint importOffset = reader.ReadUInt32();

                        string[] nameTable = new string[nameCount];
                        reader.BaseStream.Position = nameOffset;
                        for (int i = 0; i < nameCount; i++)
                        {
                            uint strLength = reader.ReadUInt32();
                            nameTable[i] = Encoding.UTF8.GetString(reader.ReadBytes((int)strLength - 1));
                            reader.BaseStream.Position += 9;
                        }
                        string[] nameImport = new string[importCount];
                        reader.BaseStream.Position = importOffset;
                        for (int i = 0; i < importCount; i++)
                        {
                            reader.BaseStream.Position += 20;
                            uint index = reader.ReadUInt32();
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
                            uint nameIndex = reader.ReadUInt32();
                            uint suffixName = reader.ReadUInt32();
                            string name = nameTable[(int)nameIndex];
                            if (suffixName != 0) name += $"_{suffixName}";
                            reader.BaseStream.Position += 12;
                            uint size = reader.ReadUInt32();
                            uint offset = reader.ReadUInt32();
                            reader.BaseStream.Position += 4;
                            if (reader.ReadUInt32() > 0) reader.BaseStream.Position += 4;
                            reader.BaseStream.Position += 20;
                            current = reader.BaseStream.Position;
                            if (nameImport[indexType] == "Sqex03DataMessage")
                            {
                                reader.BaseStream.Position = offset;
                                uint order = reader.ReadUInt32();
                                List<Speaker> speakers = new List<Speaker>();
                                while (true)
                                {
                                    string nameID = nameTable[reader.ReadInt32()];
                                    if (nameID == "None") break;
                                    string classID = nameTable[reader.ReadInt64()];
                                    if (classID == "IntProperty")
                                    {
                                        long lengthProperty = reader.ReadInt64();
                                        long intProperty = reader.ReadInt64();
                                    }
                                    else if (classID == "ArrayProperty")
                                    {
                                        long lengthProperty = reader.ReadInt64();
                                        if (nameID == "m_String")
                                        {
                                            DataMessage dataMessage = GetStrings(ref reader, name, false);
                                            result.Add(dataMessage);
                                        }
                                        else if (nameID == "m_Name")
                                        {
                                            DataMessage dataMessage = GetStrings(ref reader, $"{name}_Name", true);
                                            if (!string.IsNullOrWhiteSpace(string.Join("", dataMessage.Strings))) result.Add(dataMessage);
                                        }
                                        else if (nameID == "m_MesData")
                                        {
                                            GetSpeakers(ref reader, ref speakers, nameTable);
                                        }
                                        else
                                        {
                                            reader.BaseStream.Position += lengthProperty + 4;
                                        }
                                    }
                                }
                                if (result.Any(entry => entry.Name == $"{name}_Name"))
                                {
                                    DataMessage data = result.Find(entry => entry.Name == name);
                                    DataMessage dataName = result.Find(entry => entry.Name == $"{name}_Name");
                                    for (int y = 0; y < speakers.Count; y++)
                                    {
                                        try
                                        {
                                            speakers[y].Name = dataName.Strings[speakers[y].ID];
                                        }
                                        catch
                                        {
                                            speakers[y].Name = "";
                                        }
                                    }
                                    data.Speakers = speakers;
                                }
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

        private static void GetSpeakers(ref BinaryReaderBE reader, ref List<Speaker> speakers, string[] nameTable)
        {
            long strCount = reader.ReadInt64();
            for (int i = 0; i < strCount; i++)
            {
                Speaker speaker = new Speaker();
                while (true)
                {
                    string nameID = nameTable[reader.ReadInt32()];
                    string classID = nameTable[reader.ReadInt64()];
                    if (nameID == "m_iStrData")
                    {
                        long lengthProperty = reader.ReadInt64();
                        long subArrayCount = reader.ReadInt64();
                        reader.BaseStream.Position += lengthProperty + 4;
                        break;
                    }
                    else if (nameID == "m_iName")
                    {
                        long lengthProperty = reader.ReadInt64();
                        speaker.ID = (int)reader.ReadInt64();
                    }
                    else if (nameID == "m_iIndex")
                    {
                        long lengthProperty = reader.ReadInt64();
                        reader.ReadInt64();
                    }
                    else
                    {
                        if (classID == "IntProperty")
                        {
                            long lengthProperty = reader.ReadInt64();
                            long intProperty = reader.ReadInt64();
                        }
                        else if (classID == "ByteProperty")
                        {
                            long lengthProperty = reader.ReadInt64();
                            string nameBytesProperty = nameTable[reader.ReadInt64()];
                            reader.ReadByte();
                            uint padding = reader.ReadUInt32();
                        }
                    }
                }
                speakers.Add(speaker);
            }
        }

        private static DataMessage GetStrings(ref BinaryReaderBE reader, string name, bool isSpeakerName)
        {
            List<string> strings = new List<string>();
            long strCount = reader.ReadInt64();
            for (int line = 0; line < strCount; line++)
            {
                int strLength = reader.ReadInt32();
                if (strLength != 0)
                {
                    int zeroBytes = strLength < 0 ? 2 : 1;
                    strLength = strLength < 0 ? (int)((strLength ^ 0xFFFFFFFF) * 2) : strLength;
                    string str = zeroBytes < 2 ? Encoding.Latin1.GetString(reader.ReadBytes((int)strLength - zeroBytes)) : Encoding.Unicode.GetString(reader.ReadBytes((int)strLength));
                    for (int j = 0; j < ArchiveConfig.GameCodeDict.Count; j++)
                    {
                        string key = ArchiveConfig.GameCodeDict.ElementAt(j).Key;
                        string value = ArchiveConfig.GameCodeDict.ElementAt(j).Value;
                        str = str.Replace(key, value);
                    }
                    strings.Add(str);
                    reader.BaseStream.Position += zeroBytes;
                }
                else
                {
                    strings.Add("{null}");
                }
            }
            DataMessage dataMessage = new DataMessage(name, strings, isSpeakerName);
            return dataMessage;
        }

        public static byte[] ReImport(List<DataMessage> input, string file)
        {
            Dictionary<string, List<string>> strings = new Dictionary<string, List<string>>();
            foreach (DataMessage data in input)
            {
                strings.Add(data.Name, data.Strings);
            }
            byte[] originalFile = File.ReadAllBytes(file);
            byte[] result = Reimport(originalFile, strings);
            return result;
        }

        private static byte[] WriteStrings(ref BinaryReaderBE reader, List<string> strings)
        {
            long strCount = reader.ReadInt64();
            MemoryStream temp = new MemoryStream();
            BeBinaryWriter tempWriter = new BeBinaryWriter(temp);
            tempWriter.Write(strCount);
            for (int i = 0; i < strCount; i++)
            {
                int oldStrLength = reader.ReadInt32();
                if (oldStrLength != 0)
                {
                    string str = strings[i];
                    for (int j = 0; j < ArchiveConfig.GameCodeDict.Count; j++)
                    {
                        string key = ArchiveConfig.GameCodeDict.ElementAt(j).Key;
                        string value = ArchiveConfig.GameCodeDict.ElementAt(j).Value;
                        str = str.Replace(value, key);
                    }
                    long strLength = str.Length ^ 0xFFFFFFFF;
                    tempWriter.Write((int)strLength);
                    byte[] strBytes = Encoding.Unicode.GetBytes(str);
                    tempWriter.Write(strBytes);
                    tempWriter.Write(new byte[2]);
                    if (oldStrLength < 0)
                    {
                        oldStrLength = Math.Abs(oldStrLength) * 2;
                    }
                    reader.BaseStream.Position += oldStrLength;
                }
                else
                {
                    tempWriter.Write(new byte[4]);
                }
            }
            tempWriter.Close();
            return temp.ToArray();
        }

        private static byte[] WriteMesData(ref BinaryReaderBE reader, List<string> strings, string[] nameTable)
        {
            long strCount = reader.ReadInt64();
            MemoryStream ms = new MemoryStream();
            BeBinaryWriter bw = new BeBinaryWriter(ms);
            bw.Write(strCount);
            for (int i = 0; i < strCount; i++)
            {
                while (true)
                {
                    var nameIndex = reader.ReadInt32();
                    var classIndex = reader.ReadInt64();
                    string nameID = nameTable[nameIndex];
                    string classID = nameTable[classIndex];
                    bw.Write(nameIndex);
                    bw.Write(classIndex);
                    if (nameID == "m_iStrData")
                    {
                        long lengthProperty = reader.ReadInt64();
                        reader.BaseStream.Position += lengthProperty;
                        string str = strings[i];
                        for (int j = 0; j < ArchiveConfig.GameCodeDict.Count; j++)
                        {
                            string key = ArchiveConfig.GameCodeDict.ElementAt(j).Key;
                            string value = ArchiveConfig.GameCodeDict.ElementAt(j).Value;
                            str = str.Replace(value, key);
                        }
                        byte[] strBytes = Encoding.GetEncoding("utf-32BE").GetBytes(str);
                        bw.Write((long)(strBytes.Length + 8));
                        bw.Write((long)(str.Length) + 1);
                        bw.Write(strBytes);

                        bw.Write(reader.ReadBytes(12));
                        break;
                    }
                    else
                    {
                        if (classID == "IntProperty")
                        {
                            long lengthProperty = reader.ReadInt64();
                            long intProperty = reader.ReadInt64();
                            bw.Write(lengthProperty);
                            bw.Write(intProperty);
                        }
                        else if (classID == "ByteProperty")
                        {
                            long lengthProperty = reader.ReadInt64();
                            long nameProperty = reader.ReadInt64();
                            bw.Write(lengthProperty);
                            bw.Write(nameProperty);
                            bw.Write(reader.ReadByte());
                            uint padding = reader.ReadUInt32();
                            bw.Write(padding);
                        }
                    }
                }
            }
            bw.Close();
            return ms.ToArray();
        }

        private static byte[] Reimport(byte[] input, Dictionary<string, List<string>> data)
        {
            MemoryStream result = new MemoryStream();
            MemoryStream inputStream = new MemoryStream(input);
            BinaryReaderBE reader = new BinaryReaderBE(inputStream);
            BeBinaryWriter writer = new BeBinaryWriter(result);

            if (reader.ReadUInt32() == ArchiveConfig.Signature)
            {
                reader.BaseStream.Position = ArchiveConfig.CompressionTypeOffset;
                uint compressType = reader.ReadUInt32();
                switch (compressType)
                {
                    case 0:
                        reader.BaseStream.Position = ArchiveConfig.TableOffset;

                        uint nameCount = reader.ReadUInt32();
                        uint nameOffset = reader.ReadUInt32();
                        uint exportCount = reader.ReadUInt32();
                        uint exportOffset = reader.ReadUInt32();
                        uint importCount = reader.ReadUInt32();
                        uint importOffset = reader.ReadUInt32();
                        uint emptyOffset = reader.ReadUInt32();
                        uint unknownOffset = reader.ReadUInt32();
                        long emptyBytes = unknownOffset - emptyOffset;
                        string[] nameTable = new string[nameCount];
                        reader.BaseStream.Position = nameOffset;
                        for (int i = 0; i < nameCount; i++)
                        {
                            uint strLength = reader.ReadUInt32();
                            nameTable[i] = Encoding.UTF8.GetString(reader.ReadBytes((int)strLength - 1));
                            reader.BaseStream.Position += 9;
                        }
                        string[] nameImport = new string[importCount];
                        reader.BaseStream.Position = importOffset;
                        for (int i = 0; i < importCount; i++)
                        {
                            reader.BaseStream.Position += 20;
                            uint index = reader.ReadUInt32();
                            nameImport[i] = nameTable[(int)index];
                            reader.BaseStream.Position += 4;
                        }

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
                            reader.BaseStream.Position += 8;
                            uint nameIndex = reader.ReadUInt32();
                            uint suffixName = reader.ReadUInt32();
                            string name = nameTable[(int)nameIndex];
                            if (suffixName != 0) name += $"_{suffixName}";
                            reader.BaseStream.Position += 12;
                            uint size = reader.ReadUInt32();
                            uint offset = reader.ReadUInt32();
                            long newOffset = offset + bytesChanged;
                            reader.BaseStream.Position += 4;
                            if (reader.ReadUInt32() > 0) reader.BaseStream.Position += 4;
                            reader.BaseStream.Position += 20;
                            long headerLength = reader.BaseStream.Position - start;
                            current = reader.BaseStream.Position;
                            reader.BaseStream.Position = offset;
                            long stringBytesChanged = 0;

                            if (nameImport[indexType] == "Sqex03DataMessage")
                            {
                                uint order = reader.ReadUInt32();
                                writerData.Write(order);
                                while (true)
                                {
                                    int nameIDIndex = reader.ReadInt32();
                                    string nameID = nameTable[nameIDIndex];
                                    writerData.Write(nameIDIndex);
                                    if (nameID == "None")
                                    {
                                        writerData.Write(new byte[4]);
                                        break;
                                    }
                                    long classIDIndex = reader.ReadInt64();
                                    string classID = nameTable[classIDIndex];
                                    writerData.Write(classIDIndex);
                                    if (classID == "IntProperty")
                                    {
                                        writerData.Write(reader.ReadBytes(16));
                                    }
                                    else if (classID == "ArrayProperty")
                                    {
                                        long lengthProperty = reader.ReadInt64();
                                        if (nameID == "m_String")
                                        {
                                            List<string> strings;
                                            if (data.TryGetValue(name, out strings))
                                            {
                                                byte[] temp = WriteStrings(ref reader, strings);
                                                long newStrLength = temp.Length - 4;
                                                writerData.Write(newStrLength);
                                                writerData.Write(temp);
                                                stringBytesChanged += newStrLength - lengthProperty;
                                            }
                                            else
                                            {
                                                writerData.Write(lengthProperty);
                                                writerData.Write(reader.ReadBytes((int)lengthProperty + 4));
                                            }
                                        }
                                        else if (nameID == "m_Name")
                                        {
                                            List<string> strings;
                                            if (data.TryGetValue($"{name}_Name", out strings))
                                            {
                                                byte[] temp = WriteStrings(ref reader, strings);
                                                long newStrLength = temp.Length - 4;
                                                writerData.Write(newStrLength);
                                                writerData.Write(temp);
                                                stringBytesChanged += newStrLength - lengthProperty;
                                            }
                                            else
                                            {
                                                writerData.Write(lengthProperty);
                                                writerData.Write(reader.ReadBytes((int)lengthProperty + 4));
                                            }
                                        }
                                        else if (nameID == "m_MesData")
                                        {
                                            List<string> strings;
                                            if (data.TryGetValue(name, out strings))
                                            {
                                                byte[] temp = WriteMesData(ref reader, strings, nameTable);
                                                long newMesDataLength = temp.Length - 4;
                                                writerData.Write(newMesDataLength);
                                                writerData.Write(temp);
                                                stringBytesChanged += newMesDataLength - lengthProperty;
                                            }
                                            else
                                            {
                                                writerData.Write(lengthProperty);
                                                writerData.Write(reader.ReadBytes((int)(lengthProperty + 4)));
                                            }
                                        }
                                        else
                                        {
                                            writerData.Write(lengthProperty);
                                            writerData.Write(reader.ReadBytes((int)(lengthProperty + 4)));
                                        }
                                    }
                                }
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
                                writer.Write((uint)(size + stringBytesChanged));
                                bytesChanged += stringBytesChanged;
                                reader.BaseStream.Position += 4;
                            }
                            else
                            {
                                writer.Write(reader.ReadBytes(4));
                            }
                            writer.Write((uint)newOffset);
                            reader.BaseStream.Position += 4;
                            writer.Write(reader.ReadBytes((int)headerLength - 40));
                        }
                        writer.Write(new byte[emptyBytes]);
                        foreach (byte[] dataMessage in newData)
                        {
                            writer.Write(dataMessage);
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
