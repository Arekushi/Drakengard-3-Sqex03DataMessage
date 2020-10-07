using System;
using System.Linq;
using System.IO;
using System.IO.Compression;

namespace D3_Sqex03DataMessage
{
    class CompressedChunk
    {
        public UInt32 Uncompressed_Offset { get; set; }
        public UInt32 Uncompressed_Size { get; set; }
        public byte[] Uncompressed_Data { get; set; }
        public UInt32 Compressed_Offset { get; set; }
        public UInt32 Compressed_Size { get; set; }

        public CompressedChunk (UInt32 uncompressed_offset, UInt32 uncompressed_size, UInt32 compressed_offset, UInt32 compressed_size)
        {
            Uncompressed_Offset = uncompressed_offset;
            Uncompressed_Size = uncompressed_size;
            Compressed_Offset = compressed_offset;
            Compressed_Size = compressed_size;
        }

        public void Decompress(byte[] compressed_data)
        {
            MemoryStream stream = new MemoryStream(compressed_data);
            BinaryReaderBE reader = new BinaryReaderBE(stream);
            reader.BaseStream.Position = 8;
            UInt32 chunk_compressed_size = reader.ReadUInt32();
            reader.BaseStream.Position += 4;
            UInt32 offset = Compressed_Size - chunk_compressed_size;
            MemoryStream result = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(result);
            long chunk_offset = offset;
            while (reader.BaseStream.Position < offset)
            {
                UInt32 compressed_size = reader.ReadUInt32();
                reader.BaseStream.Position += 4;
                //UInt32 uncompressed_size = reader.ReadUInt32();
                long current = reader.BaseStream.Position;
                reader.BaseStream.Position = chunk_offset;
                chunk_offset += compressed_size;
                byte[] chunk_compressed_data = reader.ReadBytes((int)compressed_size);
                byte[] chunk_uncompress_data = Zlib_Decompress(chunk_compressed_data);
                writer.Write(chunk_uncompress_data);
                reader.BaseStream.Position = current;
            }
            reader.Close();
            writer.Close();
            Uncompressed_Data = result.ToArray();
        }

        private byte[] Zlib_Decompress(byte[] bytes)
        {
            MemoryStream stream = new MemoryStream(bytes);
            stream.Seek(2, SeekOrigin.Begin);
            MemoryStream result = new MemoryStream();
            using (DeflateStream deflateStream = new DeflateStream(stream, CompressionMode.Decompress, true))
            {
                deflateStream.CopyTo(result);
            }
            return result.ToArray();
        }
    }
}
