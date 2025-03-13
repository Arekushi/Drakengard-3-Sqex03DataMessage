using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace D3_Sqex03DataMessage.CLI.Repack
{
    public class RepackCommand : BaseCommand<RepackOptions>
    {
        public RepackCommand()
        {
            validators = [
                new RepackCommandValidator()
            ];
        }

        protected override void Execute(RepackOptions options)
        {
            List<DataMessage> dataMessages = Operation.Decrypt(options.SourcePath);
            List<string> patchFiles = [.. Directory.EnumerateFiles(options.PatchPath)];

            string[] content = patchFiles
                .Where(file => Path.GetExtension(file).Equals(".txt", StringComparison.OrdinalIgnoreCase))
                .OrderBy(filePath => RequiredTextsIDs.REQUIRED_TEXTS_IDS.IndexOf(Path.GetFileNameWithoutExtension(filePath)))
                .SelectMany(File.ReadLines)
                .ToArray();

            string jsonFile = patchFiles
                .FirstOrDefault(file => Path.GetExtension(file)
                .Equals(".json", StringComparison.OrdinalIgnoreCase));

            Operation.ImportFromJSON(jsonFile, content, dataMessages);
            Operation.Repack(options.SourcePath, dataMessages);
        }
    }
}
