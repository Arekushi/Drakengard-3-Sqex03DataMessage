using CommandLine;

namespace D3_Sqex03DataMessage.CLI.Export
{
    [Verb("export", HelpText = "Extracts texts from .XXX files and exports them to .txt files.")]
    public class ExportOptions
    {
        [Option('s', "source", Required = true, HelpText = "Path to the folder containing .XXX and TOC files (ALLMESSAGE_SF.XXX, MISSIONMESSAGE_SF.XXX, and PS3TOC.TXT).")]
        public string SourcePath { get; set; }

        [Option('d', "destination", Required = true, HelpText = "Directory where the exported files will be saved.")]
        public string DestinationPath { get; set; }

        [Option('o', "one-file", Required = false, Default = false, HelpText = "Determines whether all texts will be saved in a single .txt file (true) or multiple .txt files (false).")]
        public bool OneFile { get; set; }
    }
}
