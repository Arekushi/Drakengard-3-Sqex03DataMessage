using System;
using System.Collections.Generic;

namespace D3_Sqex03DataMessage.CLI.Exceptions
{
    public class InvalidJSONException : Exception
    {
        public InvalidJSONException(List<string> missingAttributes)
            : base($"The JSON is invalid. The following attributes are missing:\n{string.Join("\n", missingAttributes)}") { }
    }
}
