using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace D3_Sqex03DataMessage
{
    class Operation
    {
        public static List<DataMessage> Decrypt(string gameDir)
        {
            List<DataMessage> result = new List<DataMessage>();
            string[] archives = Directory
                    .GetFiles(gameDir, "*.XXX", SearchOption.AllDirectories)
                    .Where(file => ArchiveConfig.ArchiveName.Contains(Path.GetFileName(file))).ToArray();

            if (archives.Length > 0)
            {
                foreach (string archive in archives)
                {
                    List<DataMessage> messages = Sqex03DataMessage.Decrypt(File.ReadAllBytes(archive));
                    foreach (DataMessage message in messages)
                    {
                        if (!result.Any(item => item.Name == message.Name)) result.Add(message);
                    }
                }
            }

            return result;
        }

        public static void ExportAllOneFile(string exportDir, List<DataMessage> dataMessage)
        {
            Dictionary<string, int> json = new Dictionary<string, int>();
            List<string> content = new List<string>();
            foreach (DataMessage data in dataMessage)
            {
                List<string> messages = new List<string>();
                if (data.Speakers == null)
                {
                    messages = data.Strings;
                }
                else
                {
                    for (int i = 0; i < data.Strings.Count; i++)
                    {
                        messages.Add($"{data.Strings[i]}{(char)123}#Name={(char)34}{data.Speakers[i].Name}{(char)34}{(char)125}");
                    }
                }
                string dataStrings = String.Join("\r\n", messages.ToArray());
                content.Add(dataStrings);
                json.Add(data.Name, data.Strings.Count);
            }

            File.WriteAllText($"{exportDir}\\Sqex03DataMessage.txt", String.Join("\r\n", content.ToArray()));
            string jsonSerializer = JsonConvert.SerializeObject(json);
            File.WriteAllText(Path.Combine(exportDir, "export.json"), jsonSerializer);
        }

        public static void ExportAll(string exportDir, List<DataMessage> dataMessage)
        {
            Dictionary<string, int> json = new Dictionary<string, int>();

            foreach (DataMessage data in dataMessage)
            {
                string file = $"{data.Name}.txt";
                string filePath = Path.Combine(exportDir, file);
                List<string> messages = new List<string>();

                if (data.Speakers == null)
                {
                    messages = data.Strings;
                }
                else
                {
                    for (int i = 0; i < data.Strings.Count; i++)
                    {
                        messages.Add($"{data.Strings[i]}{(char)123}#Name={(char)34}{data.Speakers[i].Name}{(char)34}{(char)125}");
                    }
                }

                json.Add(data.Name, data.Strings.Count);
                string content = String.Join("\r\n", messages.ToArray());
                File.WriteAllText(filePath, content);
            }

            string jsonSerializer = JsonConvert.SerializeObject(json);
            File.WriteAllText(Path.Combine(exportDir, "export.json"), jsonSerializer);
        }

        public static void ImportFromJSON(string jsonFile, List<DataMessage> dataMessages)
        {
            var exportJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(jsonFile));
            string[] strings = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(jsonFile), "Sqex03DataMessage.txt"));
            int line = 0;

            foreach (KeyValuePair<string, string> entry in exportJson)
            {
                DataMessage data = dataMessages.Find(e => e.Name == entry.Key);
                int start = line;
                int index = 0;
                line += int.Parse($"{entry.Value}");

                for (int i = start; i < line; i++, index++)
                {
                    if (strings[i].Contains("{#Name="))
                    {
                        strings[i] = strings[i].Split(new string[] { "{#Name=" }, StringSplitOptions.None).First();
                    }

                    data.Strings[index] = strings[i];
                }
            }
        }

        public static void Repack(string game_dir, List<DataMessage> data)
        {
            List<string> archives = Directory.GetFiles(game_dir, "*.XXX", SearchOption.AllDirectories)
                    .Where(file => ArchiveConfig.ArchiveName.Contains(Path.GetFileName(file))).ToList();

            List<string> tocs = Directory.GetFiles(game_dir, "*.txt", SearchOption.AllDirectories)
                    .Where(file => ArchiveConfig.TOCName.Contains(Path.GetFileName(file))).ToList();

            if (archives.Count < 0)
            {
                return;
            }

            Dictionary<string, string[]> tocContent = new Dictionary<string, string[]>();
            foreach (string toc in tocs)
            {
                tocContent.Add(Path.GetFileName(toc), File.ReadAllLines(toc));
            }

            foreach (string archive in archives)
            {
                byte[] bytes = Sqex03DataMessage.ReImport(data, archive);
                File.WriteAllBytes(archive, bytes);
                foreach (KeyValuePair<string, string[]> entry in tocContent)
                {
                    for (int i = 0; i < entry.Value.Length; i++)
                    {
                        string[] col = entry.Value[i].Split((char)32);
                        if (col.Length > 2 && col[2].Split((char)92).LastOrDefault().ToLower() == Path.GetFileName(archive).ToLower())
                        {
                            col[0] = $"{bytes.Length}";
                            entry.Value[i] = string.Join(" ", col);
                        }
                    }
                }
            }

            foreach (string toc in tocs)
            {
                if (tocContent.TryGetValue(Path.GetFileName(toc), out string[] content))
                {
                    File.WriteAllLines(toc, content);
                }
            }
        }
    }
}
