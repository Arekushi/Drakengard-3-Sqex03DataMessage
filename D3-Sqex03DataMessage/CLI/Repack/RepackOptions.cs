using CommandLine;

namespace D3_Sqex03DataMessage.CLI.Repack
{
    [Verb("repack", HelpText = "Repackages modified texts back into .XXX files.")]
    public class RepackOptions
    {
        [Option('s', "source", Required = true, HelpText = "Path to the folder containing the .XXX and TOC files (ALLMESSAGE_SF.XXX, MISSIONMESSAGE_SF.XXX, and PS3TOC.TXT).")]
        public string SourcePath { get; set; }

        [Option('j', "jsonfile", Required = true, HelpText = "Path to the .json file that must be accompanied by a .txt file (Sqex03DataMessage.txt) containing all texts in the correct order according to the .json.")]
        public string JSONFilePath { get; set; }
    }
}
