using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Web.Script.Serialization;
using System.Reflection;

namespace D3_Sqex03DataMessage
{
    public partial class MainUI : Form
    {
        string _AppDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string _ConfigFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "config.json");
        string _MessageBoxTitle = "D3 Sqex03DataMessage";
        bool _IsBusy = false;
        List<DataMessage> _DataMessage = new List<DataMessage>();
        Dictionary<string, string> _JsonConfig = new Dictionary<string, string>();
        List<ViewUI> _ViewUI = new List<ViewUI>();
        public MainUI()
        {
            InitializeComponent();
        }

        private void MainUI_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(Path.GetDirectoryName(_ConfigFile)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_ConfigFile));
            }
            if (!File.Exists(_ConfigFile))
            {
                File.WriteAllText(_ConfigFile, "{}");
            }
            else
            {
                _JsonConfig = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(File.ReadAllText(_ConfigFile));
                if (_JsonConfig.ContainsKey("GameLocation") && !string.IsNullOrEmpty(_JsonConfig["GameLocation"]) && !string.IsNullOrWhiteSpace(_JsonConfig["GameLocation"]))
                    txtBoxGameLocation.Text = _JsonConfig["GameLocation"];
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (!_JsonConfig.ContainsKey("GameLocation")||_IsBusy)
            {
                string msg = _IsBusy ? "" : "Please select the game location.";
                MessageBox.Show(msg, _MessageBoxTitle);
                return;
            }
            _IsBusy = true;
            Task.Run(() =>
            {
                List<DataMessage> data = Open_Archive();
                if (data.Count > 0)
                {
                    _DataMessage = data;
                    
                    Get_Content();
                }
            });
            _IsBusy = false;
        }

        private List<DataMessage> Open_Archive()
        {
            string bakPath = Path.Combine(_AppDirectory, "Backup");
            List<DataMessage> result = new List<DataMessage>();
            try
            {
                if (!Directory.Exists(bakPath))
                {
                    Backup(bakPath);
                }

                string archive = Directory.GetFiles(bakPath, "*.XXX", SearchOption.AllDirectories)
                    .Where(file => ArchiveConfig.ArchiveName.Contains(Path.GetFileName(file))).FirstOrDefault();
                if (!string.IsNullOrEmpty(archive))
                {
                    Directory.Delete(bakPath, true);
                    Backup(bakPath);
                }
                result = Sqex03DataMessage.Export(File.ReadAllBytes(archive));
            }
            catch (Exception err)
            {
                result.Clear();
                MessageBox.Show($"An error occurred:\n\n{err.Message}", _MessageBoxTitle);
            }
            return result;
        }

        private void Get_Content()
        {
            this.listFiles.BeginInvoke((MethodInvoker)delegate ()
            {
                listFiles.Items.Clear();
                foreach (DataMessage data in _DataMessage)
                {
                    listFiles.Items.Add($"[{data.Index}] - {data.Name}");
                    //ViewUI view = new ViewUI();
                    
                }
            });
        }

        private void Backup(string bakPath)
        {
            Directory.CreateDirectory(bakPath);
            List<string> archives = Directory.GetFiles(_JsonConfig["GameLocation"], "*.XXX", SearchOption.AllDirectories)
                .Where(file => ArchiveConfig.ArchiveName.Contains(Path.GetFileName(file))).ToList();
            string toc = Directory.GetFiles(_JsonConfig["GameLocation"], "*.txt", SearchOption.AllDirectories)
                .Where(file => ArchiveConfig.TOCName == Path.GetFileName(file)).FirstOrDefault();
            if (archives.Count > 0)
            {
                foreach (string file in archives)
                {
                    File.Copy(file, Path.Combine(bakPath, Path.GetFileName(file)));
                }
            }
            if (!string.IsNullOrEmpty(toc)) File.Copy(toc, Path.Combine(bakPath, Path.GetFileName(toc)));
        }

        private void btnReimport_Click(object sender, EventArgs e)
        {
            
        }

        private void btnSelectGameLocation_Click(object sender, EventArgs e)
        {
            string folderPath = FolderBrowser.FolderBrowserDialog("BLUS31197");
            if (!string.IsNullOrEmpty(folderPath))
            {
                txtBoxGameLocation.Text = folderPath;
                if (_JsonConfig.ContainsKey("GameLocation"))
                {
                    _JsonConfig["GameLocation"] = folderPath;
                }
                else
                {
                    _JsonConfig.Add("GameLocation", folderPath);
                }
                string configStr = new JavaScriptSerializer().Serialize(_JsonConfig);
                try
                {
                    File.WriteAllText(_ConfigFile, configStr);
                }
                catch (Exception err)
                {
                    MessageBox.Show($"An error occurred:\n\n{err.Message}", _MessageBoxTitle);
                }
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            
        }
    }
}
