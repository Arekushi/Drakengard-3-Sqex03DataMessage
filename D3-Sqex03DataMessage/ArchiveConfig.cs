using System;
using System.Collections.Generic;

namespace D3_Sqex03DataMessage
{
    class ArchiveConfig
    {
        public Dictionary<string, string> Replace { get; set; }
        public string TOCName { get; set; }
        public List<string> ArchiveName { get; set; }
        public UInt32 Signature { get; set; }
        public UInt32 UncompressedFlags { get; set; }
        public byte[] UncompressedFlagsBytes { get; set; }
        public long CompressionTypeOffset { get; set; }
        public long PackageFlagsOffset { get; set; }
        public long TableOffset { get; set; }
        public long TableToData { get; set; }
        public List<UInt32> Skip { get; set; }

        public ArchiveConfig()
        {
            List<string> archive_name = new List<string>();
            archive_name.Add("ALLMESSAGE_SF.XXX");
            archive_name.Add("MISSIONMESSAGE_SF.XXX");

            List<UInt32> skip = new List<UInt32>();
            skip.Add(7);
            skip.Add(43);
            skip.Add(54);

            Dictionary<string, string> replace = new Dictionary<string, string>();
            replace.Add('\u0001'.ToString(), "{01}");
            replace.Add('\u0003'.ToString(), "{03}");
            replace.Add('\u0004'.ToString(), "{04}");
            replace.Add('\u0005'.ToString(), "{05}");
            replace.Add('\u0006'.ToString(), "{06}");
            replace.Add('\u0008'.ToString(), "{08}");
            replace.Add('\u0009'.ToString(), "{09}");
            replace.Add('\u0010'.ToString(), "{10}");
            replace.Add('\u000a'.ToString(), "{0A}");
            replace.Add('\u000b'.ToString(), "{0B}");
            replace.Add('\u000c'.ToString(), "{0C}");
            replace.Add('\u000d'.ToString(), "{0D}");
            replace.Add('\u000f'.ToString(), "{0F}");
            replace.Add('\u25b2'.ToString(), "{25B2}");
            replace.Add('\u25bc'.ToString(), "{25BC}");

            ArchiveName = archive_name;
            TOCName = "PS3TOC.TXT";
            Signature = 2653586369;
            UncompressedFlags = 2156396553;
            UncompressedFlagsBytes = new byte[] { 0x80, 0x88, 0x00, 0x09 };
            CompressionTypeOffset = 109;
            PackageFlagsOffset = 21;
            TableOffset = 25;
            TableToData = 456;
            Skip = skip;
            Replace = replace;
        }
    }
}
