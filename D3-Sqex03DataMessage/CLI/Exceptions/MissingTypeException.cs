using System;

namespace D3_Sqex03DataMessage.CLI.Exceptions
{
    public class MissingTypeException : Exception
    {
        public MissingTypeException(string type)
            : base($"The directory does not contain at least one file of type: [{type}]") { }
    }
}
