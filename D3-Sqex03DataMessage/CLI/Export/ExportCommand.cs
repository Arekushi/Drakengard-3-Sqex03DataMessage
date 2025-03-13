using System.Collections.Generic;

namespace D3_Sqex03DataMessage.CLI.Export
{
    public class ExportCommand : BaseCommand<ExportOptions>
    {
        public ExportCommand()
        {
            validators = [
                new ExportCommandValidator()
            ];
        }

        protected override void Execute(ExportOptions options)
        {
            List<DataMessage> dataMessages = Operation.Decrypt(options.SourcePath);

            if (options.OneFile)
            {
                Operation.ExportAllOneFile(options.DestinationPath, dataMessages);
            }
            else
            {
                Operation.ExportAll(options.DestinationPath, dataMessages);
            }
        }
    }
}
