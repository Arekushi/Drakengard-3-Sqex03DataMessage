using System;

namespace D3_Sqex03DataMessage.CLI.Exceptions
{
    public class DirectoryNotExistException : Exception
    {
        public DirectoryNotExistException(string directory)
            : base($"The directory does not exist: [{directory}]") { }
    }
}
