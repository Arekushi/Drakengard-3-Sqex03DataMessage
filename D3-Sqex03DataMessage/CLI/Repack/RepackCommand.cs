using System.Collections.Generic;

namespace D3_Sqex03DataMessage.CLI.Repack
{
    public class RepackCommand
    {
        public void RepackXXXFiles(string sourcePath, string jsonFilePath)
        {
            List<DataMessage> dataMessages = Operation.Decrypt(sourcePath);
            Operation.ImportFromJSON(jsonFilePath, dataMessages);
            Operation.Repack(sourcePath, dataMessages);
        }
    }
}
