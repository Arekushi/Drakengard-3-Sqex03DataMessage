using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace D3_Sqex03DataMessage
{
    class Operation
    {
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
                        File.Copy(file, Path.Combine(dest, Path.GetFileName(file)));
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
                result = Sqex03DataMessage.Export(File.ReadAllBytes(archive));
            }
            return result;
        }

        public static void Repack()
        {

        }
    }
}
