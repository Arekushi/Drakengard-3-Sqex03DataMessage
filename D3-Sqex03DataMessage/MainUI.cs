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
        Dictionary<string, string> _JsonConfig = new Dictionary<string, string>();
        List<ViewUI> _ViewUI = new List<ViewUI>();
        public MainUI()
        {
            InitializeComponent();
        }

        private void MainUI_Load(object sender, EventArgs e)
        {
            ArchiveConfig.Initialization();
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
            string bakPath = Path.Combine(_AppDirectory, "Backup");
            if (!Directory.Exists(bakPath)) Directory.CreateDirectory(bakPath);

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
