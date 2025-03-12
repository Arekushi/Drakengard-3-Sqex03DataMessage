using System;

namespace D3_Sqex03DataMessage.CLI.Exceptions
{
    public class FileNotExistException : Exception
    {
        public FileNotExistException(string file)
            : base($"The file does not exist: [{file}]") { }
    }
}
