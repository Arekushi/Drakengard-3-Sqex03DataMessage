using System;
using System.Linq;
using System.IO;
using System.IO.Compression;

namespace D3_Sqex03DataMessage
{
    class CompressedChunk
    {
        public UInt32 UncompressedOffset { get; set; }
        public UInt32 UncompressedSize { get; set; }
        public byte[] UncompressedData { get; set; }
        public UInt32 CompressedOffset { get; set; }
        public UInt32 CompressedSize { get; set; }

        public CompressedChunk (UInt32 uncompressedOffset, UInt32 uncompressedSize, UInt32 compressedOffset, UInt32 compressedSize)
        {
            UncompressedOffset = uncompressedOffset;
            UncompressedSize = uncompressedSize;
            CompressedOffset = compressedOffset;
            CompressedSize = compressedSize;
        }

        public void Decompress(byte[] compressed_data)
        {
            MemoryStream stream = new MemoryStream(compressed_data);
            BinaryReaderBE reader = new BinaryReaderBE(stream);
            reader.BaseStream.Position = 8;
            UInt32 chunkCompressedSize = reader.ReadUInt32();
            reader.BaseStream.Position += 4;
            UInt32 offset = CompressedSize - chunkCompressedSize;
            MemoryStream result = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(result);
            long chunkOffset = offset;
            while (reader.BaseStream.Position < offset)
            {
                UInt32 compressedSize = reader.ReadUInt32();
                reader.BaseStream.Position += 4;
                long current = reader.BaseStream.Position;
                reader.BaseStream.Position = chunkOffset;
                chunkOffset += compressedSize;
                byte[] chunkCompressedData = reader.ReadBytes((int)compressedSize);
                byte[] chunkUncompressData = ZlibDecompress(chunkCompressedData);
                writer.Write(chunkUncompressData);
                reader.BaseStream.Position = current;
            }
            reader.Close();
            writer.Close();
            UncompressedData = result.ToArray();
        }

        private byte[] ZlibDecompress(byte[] bytes)
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
