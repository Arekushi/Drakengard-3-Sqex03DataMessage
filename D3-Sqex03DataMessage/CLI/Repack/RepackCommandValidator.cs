using D3_Sqex03DataMessage.CLI.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace D3_Sqex03DataMessage.CLI.Repack
{
    public class RepackCommandValidator : IValidator<RepackOptions>
    {
        private readonly int TXT_FILE_SIZE = 16879;

        public void Validate(RepackOptions options)
        {
            string[] sourceRequiredFiles = [
                "ALLMESSAGE_SF.XXX",
                "MISSIONMESSAGE_SF.XXX",
                "PS3TOC.TXT"
            ];
            List<string> patchFiles = [.. Directory.EnumerateFiles(options.PatchPath)];

            if (!Directory.Exists(options.SourcePath))
            {
                throw new DirectoryNotExistException(options.SourcePath);
            }

            if (!Directory.Exists(options.PatchPath))
            {
                throw new DirectoryNotExistException(options.PatchPath);
            }

            if (!sourceRequiredFiles.All(file => File.Exists(Path.Combine(options.SourcePath, file))))
            {
                throw new MissingFilesException(options.SourcePath, sourceRequiredFiles);
            }

            if (options.OneFile)
            {
                string txtFile = patchFiles
                    .FirstOrDefault(file => Path.GetExtension(file)
                    .Equals(".txt", StringComparison.OrdinalIgnoreCase), null) ?? throw new MissingTypeException(".txt");

                int txtFileLinesCount = File.ReadLines(txtFile).Count();

                if (txtFileLinesCount != TXT_FILE_SIZE)
                {
                    throw new InvalidTXTFileException(txtFileLinesCount, TXT_FILE_SIZE);
                }
            }
            else
            {
                var txtsFiles = patchFiles
                    .Where(file => Path.GetExtension(file).Equals(".txt", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                var missingFiles = RequiredTextsIDs.ValidateTxtFiles(
                    [.. txtsFiles.Select(Path.GetFileNameWithoutExtension)]
                );

                if (missingFiles.Count != 0)
                {
                    throw new MissingFilesException(options.PatchPath, [.. missingFiles]);
                }

                int txtFilesLinesCount = txtsFiles.Sum(file => File.ReadLines(file).Count());
                if (txtFilesLinesCount != TXT_FILE_SIZE)
                {
                    throw new InvalidTXTFileException(txtFilesLinesCount, TXT_FILE_SIZE);
                }
            }

            string jsonFile = patchFiles
                .FirstOrDefault(file => Path.GetExtension(file)
                .Equals(".json", StringComparison.OrdinalIgnoreCase), null) ?? throw new MissingTypeException(".json");

            var missingAttributes = RequiredTextsIDs.ValidateJsonAttributes(jsonFile);

            if (missingAttributes.Count != 0)
            {
                throw new InvalidJSONException(missingAttributes);
            }
        }
    }
}
