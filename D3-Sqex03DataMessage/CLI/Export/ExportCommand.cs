using System.Collections.Generic;

namespace D3_Sqex03DataMessage.CLI.Export
{
    public class ExportCommand
    {
        public void Export(string sourcePath, string destinationPath, bool oneFile = true)
        {
            List<DataMessage> dataMessages = Operation.Decrypt(sourcePath);

            if (oneFile)
            {
                Operation.ExportAllOneFile(destinationPath, dataMessages);
            }
            else
            {
                Operation.ExportAll(destinationPath, dataMessages);
            }
        }
    }
}
