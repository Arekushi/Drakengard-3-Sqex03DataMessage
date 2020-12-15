using System;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace D3_Sqex03DataMessage
{
    class DiaglogManager
    {
        public static string FolderBrowser(string filename)
        {
            OpenFileDialog folderBrowser = new OpenFileDialog();
            folderBrowser.ValidateNames = false;
            folderBrowser.CheckFileExists = false;
            folderBrowser.CheckPathExists = true;
            folderBrowser.FileName = filename;
            string result = null;
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                result = Path.GetDirectoryName(folderBrowser.FileName);
            }
            return result;
        }

        public static void SaveFile(string name, byte[] data, string filter)
        {
            Thread newThread = new Thread(new ThreadStart(() =>
            {
                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.FileName = name;
                saveFile.Filter = filter;
                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(saveFile.FileName, data);
                }
            }));
            newThread.SetApartmentState(ApartmentState.STA);
            newThread.Start();
            newThread.Join();
        }

        public static string FileBrowser(string filename, string filter)
        {
            OpenFileDialog fileBrowser = new OpenFileDialog();
            fileBrowser.Filter = filter;
            fileBrowser.FileName = filename;
            string result = null;
            
            if (fileBrowser.ShowDialog() == DialogResult.OK)
            {
                result = fileBrowser.FileName;
            }
            return result;
        }
    }
}
