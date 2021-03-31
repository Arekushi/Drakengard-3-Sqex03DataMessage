using System;
using System.Collections.Generic;

namespace D3_Sqex03DataMessage
{
    public static class ArchiveConfig
    {
        public static readonly string[] TOCName = new string[] { "PS3TOC.TXT", "PS3TOCPATCH.TXT" };
        public static readonly string[] ArchiveName = new string[] { "ALLMESSAGE_SF.XXX", "MISSIONMESSAGE_SF.XXX" };
        public static readonly uint Signature = 0x9E2A83C1;
        public static readonly uint UncompressedFlags = 0x80880009;
        public static readonly byte[] UncompressedFlagsBytes = new byte[] { 0x80, 0x88, 0x00, 0x09 };
        public static readonly long CompressionTypeOffset = 109;
        public static readonly long PackageFlagsOffset = 21;
        public static readonly long TableOffset = 25;
        public static readonly long TableToData = 456;
        public static readonly string[] OriginalChars = new string[]
        {
            '\u0001'.ToString(), '\u0003'.ToString(), '\u0004'.ToString(), '\u0005'.ToString(),
            '\u0006'.ToString(), '\u0008'.ToString(), '\u0009'.ToString(), '\u0010'.ToString(),
            '\u000a'.ToString(), '\u000b'.ToString(), '\u000c'.ToString(), '\u000d'.ToString(),
            '\u000f'.ToString(), '\u25b2'.ToString(), '\u25bc'.ToString()
        };
        public static readonly string[] ReplaceChars = new string[]
        {
            "{01}", "{03}", "{04}", "{LF}", "{06}", "{08}", "{09}", "{10}", "{0A}", "{0B}", "{0C}",
            "{0D}", "{0F}", "{25B2}", "{25BC}"
        };
    }
}
