using CommandLine;
using D3_Sqex03DataMessage.CLI.Export;
using D3_Sqex03DataMessage.CLI.Repack;
using System;

namespace D3_Sqex03DataMessage.CLI
{
    public class CommandLineHandler
    {
        public void Execute(string[] args)
        {
            try
            {
                Parser.Default.ParseArguments<RepackOptions, ExportOptions>(args)
                    .WithParsed<RepackOptions>(opts => new RepackCommand().ExecuteCommand(opts))
                    .WithParsed<ExportOptions>(opts => new ExportCommand().ExecuteCommand(opts));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
