using D3_Sqex03DataMessage.CLI.Exceptions;
using System.IO;
using System.Linq;

namespace D3_Sqex03DataMessage.CLI.Export
{
    public class ExportCommandValidator : IValidator<ExportOptions>
    {
        public void Validate(ExportOptions options)
        {
            string[] sourceRequiredFiles = {
                "ALLMESSAGE_SF.XXX",
                "MISSIONMESSAGE_SF.XXX",
                "PS3TOC.TXT"
            };

            if (!Directory.Exists(options.SourcePath))
            {
                throw new DirectoryNotExistException(options.SourcePath);
            }

            if (!Directory.Exists(options.DestinationPath))
            {
                Directory.CreateDirectory(options.DestinationPath);
            }

            if (!sourceRequiredFiles.All(file => File.Exists(Path.Combine(options.SourcePath, file))))
            {
                throw new MissingFilesException(options.SourcePath, sourceRequiredFiles);
            }
        }
    }
}
