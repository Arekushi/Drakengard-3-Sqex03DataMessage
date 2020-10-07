using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Be.IO;

namespace D3_Sqex03DataMessage
{
    class Sqex03DataMessage
    {
        public static byte[] Decompress(byte[] input, ArchiveConfig config, uint offset)
        {
            MemoryStream stream = new MemoryStream(input);
            BinaryReaderBE reader = new BinaryReaderBE(stream);
            reader.BaseStream.Position = offset;
            UInt32 chunk_count = reader.ReadUInt32();
            long position = reader.BaseStream.Position;
            List<CompressedChunk> total_chunk = new List<CompressedChunk>();
            for (int i = 0; i < chunk_count; i++)
            {
                reader.BaseStream.Position = position;
                UInt32 uncompressed_offset = reader.ReadUInt32();
                UInt32 uncompressed_size = reader.ReadUInt32();
                UInt32 compressed_offset = reader.ReadUInt32();
                UInt32 compressed_size = reader.ReadUInt32();
                position = reader.BaseStream.Position;
                reader.BaseStream.Position = compressed_offset;
                byte[] compressed_data = reader.ReadBytes((int)compressed_size);
                CompressedChunk chunk = new CompressedChunk(uncompressed_offset, uncompressed_size, compressed_offset, compressed_size);
                chunk.Decompress(compressed_data);
                total_chunk.Add(chunk);
            }
            MemoryStream result = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(result);
            long unknow_position = position;
            reader.BaseStream.Position = 0;
            writer.Write(reader.ReadBytes((int)config.PackageFlagsOffset));
            writer.Write(config.UncompressedFlagsBytes);
            reader.BaseStream.Position += 4;
            writer.Write(reader.ReadBytes((int)(config.CompressionTypeOffset - config.PackageFlagsOffset - 4)));
            writer.Write(new byte[8]);
            reader.BaseStream.Position = unknow_position;
            writer.Write(reader.ReadBytes(8));
            position = config.CompressionTypeOffset + 16;
            foreach (CompressedChunk chunk in total_chunk)
            {
                if (chunk.Uncompressed_Offset > position)
                {
                    long d = chunk.Uncompressed_Offset - position;
                    writer.Write(new byte[d]);
                    position = chunk.Uncompressed_Offset;
                }
                writer.Write(chunk.Uncompressed_Data);
                position += chunk.Uncompressed_Size;
            }
            writer.Close();
            reader.Close();
            return result.ToArray();
        }

        public static List<DataMessage> Export(byte[] input, ArchiveConfig config)
        {
            List<DataMessage> result = new List<DataMessage>();
            MemoryStream stream = new MemoryStream(input);
            BinaryReaderBE reader = new BinaryReaderBE(stream);
            if (reader.ReadUInt32() == config.Signature)
            {
                reader.BaseStream.Position = config.CompressionTypeOffset;
                UInt32 compress_type = reader.ReadUInt32();
                switch (compress_type)
                {
                    case 0:
                        reader.BaseStream.Position = config.TableOffset;

                        UInt32 name_count = reader.ReadUInt32();
                        UInt32 name_offset = reader.ReadUInt32();
                        UInt32 export_count = reader.ReadUInt32();
                        UInt32 export_offset = reader.ReadUInt32();
                        UInt32 import_count = reader.ReadUInt32();
                        UInt32 import_offset = reader.ReadUInt32();

                        string[] name_table = new string[name_count];
                        reader.BaseStream.Position = name_offset;
                        for (int i = 0; i < name_count; i++)
                        {
                            UInt32 str_length = reader.ReadUInt32();
                            name_table[i] = Encoding.UTF8.GetString(reader.ReadBytes((int)str_length - 1));
                            reader.BaseStream.Position += 9;
                        }
                        string[] name_import = new string[import_count];
                        reader.BaseStream.Position = import_offset;
                        for (int i = 0; i < import_count; i++)
                        {
                            reader.BaseStream.Position += 20;
                            UInt32 index = reader.ReadUInt32();
                            name_import[i] = name_table[(int)index];
                            reader.BaseStream.Position += 4;
                        }
                        reader.BaseStream.Position = export_offset;
                        long current = reader.BaseStream.Position;
                        for (int i = 0; i < export_count; i++)
                        {
                            reader.BaseStream.Position = current;
                            uint index_type = (uint)reader.ReadInt32() ^ 0xFFFFFFFF;
                            reader.BaseStream.Position += 8;
                            UInt32 name_index = reader.ReadUInt32();
                            string name = name_table[(int)name_index];
                            reader.BaseStream.Position += 16;
                            UInt32 size = reader.ReadUInt32();
                            UInt32 offset = reader.ReadUInt32();
                            reader.BaseStream.Position += 4;
                            if (reader.ReadUInt32() > 0) reader.BaseStream.Position += 4;
                            reader.BaseStream.Position += 20;
                            current = reader.BaseStream.Position;
                            
                            if (index_type == 2) //Sqex03DataMessage
                            {
                                reader.BaseStream.Position = offset;
                                UInt32 order = reader.ReadUInt32();
                                if (config.Skip.Contains(order)) continue;
                                reader.BaseStream.Position += 68;
                                UInt64 unknow_data_length = reader.ReadUInt64();
                                reader.BaseStream.Position += (long)unknow_data_length + 16 + 8;
                                List<string> strings = new List<string>();
                                UInt64 total_lines = reader.ReadUInt64();
                                for (int line = 0; line < (long)total_lines; line++)
                                {
                                    long str_length = reader.ReadInt32();
                                    
                                    int zero_bytes = 1;
                                    if (str_length < 0)
                                    {
                                        str_length = (str_length^0xFFFFFFFF)*2;
                                        zero_bytes = 2;
                                    }
                                    
                                    string str = zero_bytes < 2 ? Encoding.UTF8.GetString(reader.ReadBytes((int)str_length - zero_bytes)) : Encoding.Unicode.GetString(reader.ReadBytes((int)str_length));
                                    foreach (KeyValuePair<string, string> entry in config.Replace)
                                    {
                                        str = str.Replace(entry.Key, entry.Value);
                                    }
                                    strings.Add(str);
                                    reader.BaseStream.Position += zero_bytes;
                                }
                                DataMessage data_message = new DataMessage(name, (int)order, strings);
                                result.Add(data_message);
                            }
                        }
                        break;
                    case 1:
                        byte[] decompressed_data = Decompress(input, config, (uint)reader.BaseStream.Position);
                        return Export(decompressed_data, config);
                    default:
                        throw new Exception("Compression type is not supported.");
                }
            }
            else
            {
                throw new Exception("File is not a Drakengard 3 (Unreal 3) file.");
            }
            reader.Close();
            return result;
        }

        public static void ReImport(List<DataMessage> input, string original_directory, string working_directory, ArchiveConfig config)
        {
            Dictionary<UInt32, List<string>> strings = new Dictionary<uint, List<string>>();
            foreach (DataMessage data in input)
            {
                strings.Add((UInt32)data.Index, data.Strings);
            }
            List<string> archive_paths = Directory.GetFiles(original_directory, "*.XXX", SearchOption.AllDirectories)
                .Where(file => config.ArchiveName.Contains(Path.GetFileName(file))).ToList();
            string toc_path = Directory.GetFiles(original_directory, "*.txt", SearchOption.AllDirectories)
                .Where(file => Path.GetFileName(file) == config.TOCName).FirstOrDefault();
            string import_path = Path.Combine(working_directory, "ReImport");
            if (!Directory.Exists(import_path)) Directory.CreateDirectory(import_path);
            foreach (string file in archive_paths)
            {
                string out_path = Path.Combine(import_path, Path.GetFileName(file));
                byte[] bytes = File.ReadAllBytes(file);
                byte[] result = Reimport(bytes, strings, config);
                File.WriteAllBytes(out_path, result);
            }

        }

        /*public static void ReImport (byte[] input, string original_directory, string working_directory, ArchiveConfig config)
        {

        }*/

        private static byte[] Reimport (byte[] input, Dictionary<UInt32, List<string>> data, ArchiveConfig config)
        {
            MemoryStream result = new MemoryStream();
            MemoryStream input_stream = new MemoryStream(input);
            BinaryReaderBE reader = new BinaryReaderBE(input_stream);
            BeBinaryWriter writer = new BeBinaryWriter(result);

            if (reader.ReadUInt32() == config.Signature)
            {
                reader.BaseStream.Position = config.CompressionTypeOffset;
                UInt32 compress_type = reader.ReadUInt32();
                switch (compress_type)
                {
                    case 0:
                        reader.BaseStream.Position = config.TableOffset;

                        UInt32 name_count = reader.ReadUInt32();
                        UInt32 name_offset = reader.ReadUInt32();
                        UInt32 export_count = reader.ReadUInt32();
                        UInt32 export_offset = reader.ReadUInt32();
                        UInt32 import_count = reader.ReadUInt32();
                        UInt32 import_offset = reader.ReadUInt32();

                        reader.BaseStream.Position = 0;
                        writer.Write(reader.ReadBytes((int)export_offset)); //header
                        
                        long bytes_changed = 0;
                        List<byte[]> new_data = new List<byte[]>();
                        long current = export_offset;
                        for (int i = 0; i < export_count; i++)
                        {
                            MemoryStream new_real_data = new MemoryStream();
                            BeBinaryWriter writer_data = new BeBinaryWriter(new_real_data);
                            reader.BaseStream.Position = current;
                            long start = reader.BaseStream.Position;
                            uint index_type = (uint)reader.ReadInt32() ^ 0xFFFFFFFF;
                            reader.BaseStream.Position += 28;
                            UInt32 size = reader.ReadUInt32();
                            UInt32 offset = reader.ReadUInt32();                            
                            long new_offset = offset + bytes_changed;
                            reader.BaseStream.Position += 4;
                            if (reader.ReadUInt32() > 0) reader.BaseStream.Position += 4;
                            reader.BaseStream.Position += 20;
                            long header_length = reader.BaseStream.Position - start;
                            current = reader.BaseStream.Position;
                            reader.BaseStream.Position = offset;
                            UInt32 order = reader.ReadUInt32();

                            reader.BaseStream.Position = offset;
                            List<string> strings;
                            long string_bytes_changed = 0;
                            
                            if (index_type == 2 && !config.Skip.Contains(order) && data.TryGetValue(order, out strings))
                            {
                                writer_data.Write(reader.ReadBytes(72));
                                UInt64 unknow_data_length = reader.ReadUInt64();
                                writer_data.Write(unknow_data_length);
                                writer_data.Write(reader.ReadBytes((int)unknow_data_length + 16));
                                UInt64 old_size = reader.ReadUInt64();
                                UInt64 total_lines = reader.ReadUInt64();
                                int new_str_length = 0;
                                MemoryStream temp = new MemoryStream();
                                BeBinaryWriter writer_temp = new BeBinaryWriter(temp);
                                foreach (string line in strings) 
                                {
                                    string line_str = line;
                                    foreach (KeyValuePair<string, string> entry in config.Replace)
                                    {
                                        line_str = line_str.Replace(entry.Value, entry.Key);
                                    }
                                    long str_length = line_str.Length ^ 0xFFFFFFFF;
                                    writer_temp.Write((Int32)str_length);
                                    byte[] str = Encoding.Unicode.GetBytes(line_str);
                                    writer_temp.Write(str);
                                    writer_temp.Write(new byte[2]);
                                    new_str_length += 4 + str.Length + 2;
                                }
                                writer_temp.Close();
                                new_str_length += 4;
                                string_bytes_changed = (long)new_str_length - (long)old_size;
                                writer_data.Write((UInt64)new_str_length);
                                writer_data.Write(total_lines);
                                writer_data.Write(temp.ToArray());
                                reader.BaseStream.Position += ((long)old_size - 4);
                                writer_data.Write(reader.ReadBytes((int)size - 72 - (int)unknow_data_length - 36 - (int)old_size));
                                
                            }
                            else
                            {
                                writer_data.Write(reader.ReadBytes((int)size));
                            }
                            writer_data.Close();
                            new_data.Add(new_real_data.ToArray());
                            reader.BaseStream.Position = start;
                            writer.Write(reader.ReadBytes(32));
                            if (string_bytes_changed != 0)
                            {
                                writer.Write((UInt32)(size + string_bytes_changed));
                                bytes_changed += string_bytes_changed;
                                reader.BaseStream.Position += 4;
                            }
                            else
                            {
                                writer.Write(reader.ReadBytes(4));
                            }
                            writer.Write((UInt32)new_offset);
                            reader.BaseStream.Position += 4;
                            writer.Write(reader.ReadBytes((int)header_length - 40));
                        }
                        writer.Write(new byte[config.TableToData]);
                        foreach (byte[] data_message in new_data)
                        {
                            writer.Write(data_message);
                        }
                        writer.Write(new byte[4]);
                        break;
                    case 1:
                        byte[] decompressed_data = Decompress(input, config, (uint)reader.BaseStream.Position);
                        return Reimport(decompressed_data, data, config);
                    default:
                        throw new Exception("Compression type is not supported.");
                }
            }
            else
            {
                throw new Exception("File is not a Drakengard 3 (Unreal 3) file.");
            }
            reader.Close();
            writer.Close();
            return result.ToArray();
        }
    }
}
