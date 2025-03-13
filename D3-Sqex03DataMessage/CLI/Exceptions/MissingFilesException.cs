using System;

namespace D3_Sqex03DataMessage.CLI.Exceptions
{
    public class MissingFilesException : Exception
    {
        public MissingFilesException(string directory, string[] files)
            : base($"The directory [{directory}]\nMust contain all of the following files:\n{string.Join("\n", files)}") { }
    }
}
