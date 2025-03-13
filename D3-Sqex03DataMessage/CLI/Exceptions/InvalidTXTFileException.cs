using System;

namespace D3_Sqex03DataMessage.CLI.Exceptions
{
    public class InvalidTXTFileException : Exception
    {
        public InvalidTXTFileException(int currentSize, int expectedSize)
            : base($"The length of the provided .txt file(s) [{currentSize} line(s)] does not match the expected size.\n" +
                  $"Please ensure it contains exactly [{expectedSize} lines].")
        { }
    }
}
