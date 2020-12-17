using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web.Script.Serialization;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace D3_Sqex03DataMessage
{
    class Operation
    {
        public static void ProgressBar(ProgressBar progressBar, int percent)
        {
            progressBar.BeginInvoke((MethodInvoker)delegate
            {
                progressBar.Value = percent > 100 ? 100 : percent;
            });

        }
        public static void Backup(string source, string dest)
        {
            if (!Directory.Exists(dest))
            {
                Directory.CreateDirectory(dest);
                List<string> files = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories)
                    .Where(file => ArchiveConfig.ArchiveName.Contains(Path.GetFileName(file)) || ArchiveConfig.TOCName == Path.GetFileName(file)).ToList();
                if (files.Count > 0)
                {
                    foreach (string file in files)
                    {
                        File.Copy(file, Path.Combine(dest, Path.GetFileName(file)), true);
                    }
                }
            }
        }

        public static List<DataMessage> Decrypt(string app_dir, string game_dir)
        {
            
            List<DataMessage> result = new List<DataMessage>();
            /*string bak_dir = Path.Combine(app_dir, "Backup");
            Backup(game_dir, bak_dir);*/
            string archive = Directory.GetFiles(game_dir, "*.XXX", SearchOption.AllDirectories)
                    .Where(file => ArchiveConfig.ArchiveName.Contains(Path.GetFileName(file))).FirstOrDefault();
            if (!string.IsNullOrEmpty(archive))
            {
                result = Sqex03DataMessage.Decrypt(File.ReadAllBytes(archive));
            }
            return result;
        }
        public static void ExportAll(string fileName, List<DataMessage> dataMessage, bool oneFile, ProgressBar progressBar)
        {
            double percent = 100.0 / dataMessage.Count;
            if (oneFile)
            {
                string exportDir = Path.GetDirectoryName(fileName);
                Dictionary<string, int> json = new Dictionary<string, int>();
                List<string> content = new List<string>();
                foreach (DataMessage data in dataMessage)
                {
                    string data_strings = String.Join("\r\n", data.Strings.ToArray());
                    content.Add(data_strings);
                    json.Add(data.Name, data.Strings.Count);
                    percent += 100.0 / dataMessage.Count;
                    ProgressBar(progressBar, (int)percent);

                }
                File.WriteAllText(fileName, String.Join("\r\n", content.ToArray()));
                string jsonSerializer = new JavaScriptSerializer().Serialize(json);
                File.WriteAllText(Path.Combine(exportDir, "export.json"), jsonSerializer);
            }
            else
            {
                foreach (DataMessage data in dataMessage)
                {
                    string file = $"{data.Name}.txt";
                    string filePath = Path.Combine(fileName, file);
                    string content = String.Join("\r\n", data.Strings.ToArray());
                    File.WriteAllText(filePath, content);
                    percent += 100.0 / dataMessage.Count;
                    ProgressBar(progressBar, (int)percent);
                }
            }
        }

        public static void Export(DataMessage data_message)
        {
            byte[] data = Encoding.UTF8.GetBytes(String.Join("\r\n", data_message.Strings.ToArray()));
            Thread newThread = new Thread(new ThreadStart(() =>
            {
                string fileName = DiaglogManager.SaveFile($"{data_message.Name}", "Text files (*.txt)|*.txt|All files (*.*)|*.*");
                if (!string.IsNullOrEmpty(fileName))
                {
                    File.WriteAllBytes(fileName, data);
                }
            }));
            newThread.SetApartmentState(ApartmentState.STA);
            newThread.Start();
            newThread.Join();
        }

        public static void ImportJSON(string jsonFile, ProgressBar progressBar)
        {
            Dictionary<string, string> exportJson = new JavaScriptSerializer().Deserialize<dynamic>(File.ReadAllText(jsonFile));
            double percent = 100.0 / jsonFile.Length;

            string[] strings = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(jsonFile), "Sqex03DataMessage.txt"));
            int line = 0;
            foreach (KeyValuePair<string, string> entry in exportJson)
            {
                DataMessage data = MainUI._DataMessage.Find(e => e.Name == entry.Key);
                int start = line;
                int index = 0;
                line += int.Parse($"{entry.Value}");
                for (int i = start; i < line; i++, index++)
                {
                    data.Strings[index] = strings[i];
                }
                percent += 100.0 / jsonFile.Length;
                ProgressBar(progressBar, (int)percent);
            }
        }
        public static void ImportDirectory(string dir, ProgressBar progressBar)
        {
            string[] files = Directory.GetFiles(dir, ".txt", SearchOption.TopDirectoryOnly);
            double percent = 100.0 / MainUI._DataMessage.Count;
            ProgressBar(progressBar, (int)percent);
            foreach (DataMessage data in MainUI._DataMessage)
            {
                string file = Array.Find(files, element => Path.GetFileName(element) == data.Name);
                if (!string.IsNullOrEmpty(file))
                {
                    string[] strings = File.ReadAllLines(file);
                    for (int i = 0; i < data.Strings.Count; i++)
                    {
                        data.Strings[i] = strings[i];
                    }
                }
                percent += 100.0 / MainUI._DataMessage.Count;
                ProgressBar(progressBar, (int)percent);
            }
        }
        public static void Repack(string app_dir, string game_dir, List<DataMessage> data, ProgressBar progressBar)
        {
            /*string bak_dir = Path.Combine(app_dir, "Backup");
            Backup(game_dir, bak_dir);*/
            List<string> archives = Directory.GetFiles(game_dir, "*.XXX", SearchOption.AllDirectories)
                    .Where(file => ArchiveConfig.ArchiveName.Contains(Path.GetFileName(file))).ToList();
            string toc = Directory.GetFiles(game_dir, "*.txt", SearchOption.AllDirectories)
                    .Where(file => ArchiveConfig.TOCName == Path.GetFileName(file)).FirstOrDefault();
            if (archives.Count < 0) return;
            double percent = 0;
            string[] toc_lines = string.IsNullOrEmpty(toc) ? null : File.ReadAllLines(toc);
            foreach (string archive in archives)
            {
                byte[] bytes = Sqex03DataMessage.ReImport(data, archive);
                File.WriteAllBytes(archive, bytes);
                if (toc_lines != null)
                {
                    for (int i = 0; i < toc_lines.Length; i++)
                    {
                        string[] entry = toc_lines[i].Split((char)32);
                        if (entry[2].Split((char)92).LastOrDefault().ToLower() == Path.GetFileName(archive).ToLower())
                        {
                            entry[0] = $"{bytes.Length}";
                            toc_lines[i] = String.Join(" ", entry);
                        }
                    }
                }
                percent += 100.0 / archives.Count;
                ProgressBar(progressBar, (int)percent);
            }
            if (!string.IsNullOrEmpty(toc)) File.WriteAllLines(toc, toc_lines);
        }
    }
}
