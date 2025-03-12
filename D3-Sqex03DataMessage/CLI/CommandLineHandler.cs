using CommandLine;
using D3_Sqex03DataMessage.CLI.Export;
using D3_Sqex03DataMessage.CLI.Repack;
using System;
using System.Collections.Generic;

namespace D3_Sqex03DataMessage.CLI
{
    public class CommandLineHandler
    {
        private readonly Dictionary<Type, object> validators = new Dictionary<Type, object>()
        {
            { typeof(ExportOptions), new ExportCommandValidator() },
            { typeof(RepackOptions), new RepackCommandValidator() }
        };

        public void Execute(string[] args)
        {
            try
            {
                Parser.Default.ParseArguments<RepackOptions, ExportOptions>(args)
                    .WithParsed<RepackOptions>(opts => ExecuteCommand(opts, new RepackCommand()))
                    .WithParsed<ExportOptions>(opts => ExecuteCommand(opts, new ExportCommand()))
                    .WithNotParsed(opts => Console.WriteLine("Error parsing arguments. Please try again."));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void ExecuteCommand<TOptions>(TOptions options, dynamic command)
        {
            Validate(options);

            switch (options)
            {
                case RepackOptions repack:
                    command.RepackXXXFiles(repack.SourcePath, repack.JSONFilePath);
                    break;
                case ExportOptions export:
                    command.Export(export.SourcePath, export.DestinationPath, export.OneFile);
                    break;
            }
        }

        private void Validate<TOptions>(TOptions options)
        {
            if (validators.TryGetValue(typeof(TOptions), out var validator))
            {
                ((IValidator<TOptions>)validator).Validate(options);
            }
        }
    }
}
