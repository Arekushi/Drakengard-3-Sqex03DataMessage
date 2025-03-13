using CommandLine;

namespace D3_Sqex03DataMessage.CLI.Repack
{
    [Verb("repack", HelpText = "Repackages modified texts back into .XXX files.")]
    public class RepackOptions
    {
        [Option('s', "source", Required = true, HelpText = "Path to the folder containing the .XXX and TOC files (ALLMESSAGE_SF.XXX, MISSIONMESSAGE_SF.XXX, and PS3TOC.TXT).")]
        public string SourcePath { get; set; }

        [Option('p', "patch", Required = true, HelpText = "Path where the export.json file is located, along with either a single .txt file or multiple separate .txt files.")]
        public string PatchPath { get; set; }

        [Option('o', "one-file", Required = false, Default = false, HelpText = "Determines whether all texts will be read from a single .txt file or if they are separated.")]
        public bool OneFile { get; set; }
    }
}
