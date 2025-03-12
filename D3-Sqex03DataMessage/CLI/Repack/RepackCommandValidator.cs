using D3_Sqex03DataMessage.CLI.Exceptions;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace D3_Sqex03DataMessage.CLI.Repack
{
    public class RepackCommandValidator : IValidator<RepackOptions>
    {
        public void Validate(RepackOptions options)
        {
            string[] sourceRequiredFiles = {
                "ALLMESSAGE_SF.XXX",
                "MISSIONMESSAGE_SF.XXX",
                "PS3TOC.TXT"
            };

            string[] jsonRequiredFiles =
            {
                "Sqex03DataMessage.txt"
            };

            if (!Directory.Exists(options.SourcePath))
            {
                throw new DirectoryNotExistException(options.SourcePath);
            }

            if (!File.Exists(options.JSONFilePath))
            {
                throw new FileNotExistException(options.JSONFilePath);
            }

            if (!sourceRequiredFiles.All(file => File.Exists(Path.Combine(options.SourcePath, file))))
            {
                throw new MissingFilesException(options.SourcePath, sourceRequiredFiles);
            }

            if (!jsonRequiredFiles.All(file => File.Exists(Path.Combine(Path.GetDirectoryName(options.JSONFilePath), file))))
            {
                throw new MissingFilesException(options.JSONFilePath, jsonRequiredFiles);
            }

            var (success, missingAttributes) = RequiredJsonFields.ValidateJsonAttributes(options.JSONFilePath);

            if (!success)
            {
                throw new InvalidJSONException(missingAttributes);
            }
        }
    }
}
