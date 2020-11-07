using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web.Script.Serialization;
using System.Text;
using System.Windows.Forms;

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
            string bak_dir = Path.Combine(app_dir, "Backup");
            Backup(game_dir, bak_dir);
            string archive = Directory.GetFiles(game_dir, "*.XXX", SearchOption.AllDirectories)
                    .Where(file => ArchiveConfig.ArchiveName.Contains(Path.GetFileName(file))).FirstOrDefault();
            if (!string.IsNullOrEmpty(archive))
            {
                result = Sqex03DataMessage.Decrypt(File.ReadAllBytes(archive));
            }
            return result;
        }
        struct ExportJson
        {
            public bool Merge;
            public string Archive;
            public Dictionary<string, string> Files;
        }
        public static void ExportAll(string export_dir, List<DataMessage> data_message, bool merge, ProgressBar progressBar)
        {
            double percent = 100.0 / data_message.Count;
            ExportJson json = new ExportJson();
            json.Merge = merge;
            json.Files = new Dictionary<string, string>();
            json.Archive = merge ? "Sqex03DataMessage.txt" : null;
            if (merge)
            {
                List<string> content = new List<string>();
                foreach (DataMessage data in data_message)
                {
                    string data_strings = String.Join("\r\n", data.Strings.ToArray());
                    content.Add(data_strings);
                    json.Files.Add($"{data.Index}", $"{data.Strings.Count}");
                    percent += 100.0 / data_message.Count;
                    ProgressBar(progressBar, (int)percent);
                }
                File.WriteAllText(Path.Combine(export_dir, json.Archive), String.Join("\r\n", content.ToArray()));
            }
            else
            {
                foreach (DataMessage data in data_message)
                {
                    string filename = $"[{data.Index}] {data.Name}.txt";
                    string file = Path.Combine(export_dir, filename);
                    string content = String.Join("\r\n", data.Strings.ToArray());
                    File.WriteAllText(file, content);
                    json.Files.Add($"{data.Index}", filename);
                    percent += 100.0 / data_message.Count;
                    ProgressBar(progressBar, (int)percent);
                }
            }
            string json_content = new JavaScriptSerializer().Serialize(json);
            File.WriteAllText(Path.Combine(export_dir, "export.json"), json_content);
        }

        public static void Export(DataMessage data_message)
        {
            byte[] data = Encoding.UTF8.GetBytes(String.Join("\r\n", data_message.Strings.ToArray()));
            DiaglogManager.SaveFile($"[{data_message.Index}] {data_message.Name}", data, "Text files (*.txt)|*.txt|All files (*.*)|*.*");
        }

        public static void ImportAll(string json_file, ProgressBar progressBar)
        {
            dynamic export_config = new JavaScriptSerializer().Deserialize<dynamic>(File.ReadAllText(json_file));
            double percent = 100.0 / export_config["Files"].Count;
            if ((bool)export_config["Merge"] == true)
            {
                string[] strings = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(json_file), export_config["Archive"]));
                int line = 0;
                foreach (KeyValuePair<string, object> entry in export_config["Files"])
                {
                    DataMessage data = MainUI._DataMessage.Find(e => e.Index == int.Parse(entry.Key));
                    int start = line;
                    int index = 0;
                    line += int.Parse($"{entry.Value}");
                    for (int i = start; i < line; i++, index++)
                    {
                        data.Strings[index] = strings[i];
                    }
                    percent += 100.0 / export_config["Files"].Count;
                    ProgressBar(progressBar, (int)percent);
                }
            }
            else
            {
                foreach (KeyValuePair<string, object> entry in export_config["Files"])
                {
                    DataMessage data = MainUI._DataMessage.Find(e => e.Index == int.Parse(entry.Key));
                    percent += 100.0 / export_config["Files"].Count;
                    ProgressBar(progressBar, (int)percent);
                    if (data == null || !File.Exists(Path.Combine(Path.GetDirectoryName(json_file), $"{entry.Value}"))) continue;
                    string[] lines = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(json_file), $"{entry.Value}"));
                    for (int i = 0; i < data.Strings.Count; i++)
                    {
                        data.Strings[i] = lines[i];
                    }
                }
            }
        }

        public static void Repack(string app_dir, string game_dir, List<DataMessage> data, ProgressBar progressBar)
        {
            string bak_dir = Path.Combine(app_dir, "Backup");
            Backup(game_dir, bak_dir);
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
