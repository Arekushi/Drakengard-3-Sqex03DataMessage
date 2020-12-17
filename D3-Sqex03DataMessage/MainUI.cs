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
using System.Diagnostics;

namespace D3_Sqex03DataMessage
{
    public partial class MainUI : Form
    {
        string _AppDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string _ConfigFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "config.json");
        string _MessageBoxTitle = "D3 Sqex03DataMessage";
        Editor _Editor = Editor.Instance();
        bool _IsBusy = false;
        public static List<DataMessage> _DataMessage = new List<DataMessage>();
        Dictionary<string, string> _JsonConfig = new Dictionary<string, string>();
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
                string msg = _IsBusy ? "Another task is already in progress." : "Please select your game directory.";
                MessageBox.Show(msg, _MessageBoxTitle);
            }
            else
            {
                _IsBusy = true;
                Task.Run(() =>
                {
                    try
                    {
                        List<DataMessage> data = Operation.Decrypt(_AppDirectory, _JsonConfig["GameLocation"]);
                        if (data.Count > 0)
                        {
                            _DataMessage = data;
                            this.listFiles.BeginInvoke((MethodInvoker)delegate ()
                            {
                                listFiles.Items.Clear();
                                foreach (DataMessage entry in _DataMessage)
                                {
                                    listFiles.Items.Add($"[{entry.Index}] - {entry.Name}");
                                }
                            });
                        }
                    }
                    catch (Exception err)
                    {
                        _IsBusy = false;
                        _DataMessage.Clear();
                        MessageBox.Show($"An error occurred:\n\n{err}", _MessageBoxTitle);
                    }
                }).GetAwaiter().OnCompleted(() => { _IsBusy = false; });
            } 
        }


        private void btnReimport_Click(object sender, EventArgs e)
        {
            if (!_JsonConfig.ContainsKey("GameLocation") || _IsBusy)
            {
                string msg = _IsBusy ? "Another task is already in progress." : "Please select your game directory.";
                MessageBox.Show(msg, _MessageBoxTitle);
            }
            else
            {
                _IsBusy = true;
                Task.Run(() =>
                {
                    try
                    {
                        Operation.Repack(_AppDirectory, _JsonConfig["GameLocation"], _DataMessage, this.progressBar);
                    }
                    catch (Exception err)
                    {
                        _IsBusy = false;
                        MessageBox.Show($"An error occurred:\n\n{err.Message}", _MessageBoxTitle);
                    }
                }).GetAwaiter().OnCompleted(() => { _IsBusy = false; });
            }
        }

        private void btnSelectGameLocation_Click(object sender, EventArgs e)
        {
            string folderPath = DiaglogManager.FolderBrowser("BLUS31197");
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

        private void listFiles_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listFiles.SelectedIndex < 0 || _IsBusy ) return;
            int index = listFiles.SelectedIndex;
            try
            {
                DataMessage data = _DataMessage[index];
                ViewUI editor = _Editor.TransferData(data);
                if (!editor.Visible) editor.Show();
            }
            catch (Exception err)
            {
                MessageBox.Show($"An error occurred:\n\n{err.Message}", _MessageBoxTitle);
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_DataMessage.Count() <= 0 || _IsBusy || listFiles.SelectedIndex <= -1) return;
            _IsBusy = true;
            int index = listFiles.SelectedIndex;
            Task.Run(() =>
            {
                try
                {
                    Operation.Export(_DataMessage[index]);
                }
                catch (Exception err)
                {
                    _IsBusy = false;
                    MessageBox.Show($"An error occurred:\n\n{err.Message}", _MessageBoxTitle);
                }
            }).GetAwaiter().OnCompleted(() => { _IsBusy = false; });
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_DataMessage.Count() <= 0 || _IsBusy || listFiles.SelectedIndex <= -1) return;
            int index = listFiles.SelectedIndex;
            string fileName = _DataMessage[index].Name;
            string fileImport = DiaglogManager.FileBrowser(fileName, "Text files (*.txt)|*.txt|All files (*.*)|*.*");
            if (string.IsNullOrEmpty(fileImport)) return;
            _IsBusy = true;
            Task.Run(() =>
            {
                try
                {
                    double percent = 100.0 / _DataMessage[index].Strings.Count;
                    string[] lines = File.ReadAllLines(fileImport);
                    for (int i = 0; i < _DataMessage[index].Strings.Count; i++)
                    {
                        _DataMessage[index].Strings[i] = lines[i];
                        percent += 100.0 / _DataMessage[index].Strings.Count;
                        Operation.ProgressBar(this.progressBar, (int)percent);
                    }
                }
                catch (Exception err)
                {
                    _IsBusy = false;
                    MessageBox.Show($"An error occurred:\n\n{err.Message}", _MessageBoxTitle);
                }
            }).GetAwaiter().OnCompleted(() => { _IsBusy = false; });
        }

        private void exportAllStripMenuItem_Click(object sender, EventArgs e)
        {
            Extract(false);
            
        }
        private void Extract(bool oneFile)
        {
            if (_DataMessage.Count() <= 0 || _IsBusy) return;
            string fileName = DiaglogManager.SaveFile("Sqex03DataMessage.txt", "Text files (*.txt)|*.txt|All files (*.*)|*.*");
            if (string.IsNullOrEmpty(fileName)) return;
            _IsBusy = true;
            Task.Run(() =>
            {
                try
                {
                    Operation.ExportAll(fileName, _DataMessage, oneFile, this.progressBar);
                }
                catch (Exception err)
                {
                    _IsBusy = false;
                    MessageBox.Show($"An error occurred:\n\n{err.Message}", _MessageBoxTitle);
                }
            }).GetAwaiter().OnCompleted(() => { _IsBusy = false; });
        }

        private void importAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_DataMessage.Count() <= 0 || _IsBusy ) return;
            string dir = DiaglogManager.FolderBrowser("Export");
            if (string.IsNullOrEmpty(dir)) return;
            _IsBusy = true;
            Task.Run(() =>
            {
                try
                {
                    Operation.ImportDirectory(dir, this.progressBar);
                }
                catch (Exception err)
                {
                    _IsBusy = false;
                    MessageBox.Show($"An error occurred:\n\n{err.Message}", _MessageBoxTitle);
                }
            }).GetAwaiter().OnCompleted(() => { _IsBusy = false; });
        }

        private void linkLabelGit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/lehieugch68/Drakengard-3-Sqex03DataMessage");
        }
        private void linkLabelVHG_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://viethoagame.com/");
        }

        private void exportAllOneFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Extract(true);
        }

        private void importAllOneFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_DataMessage.Count() <= 0 || _IsBusy) return;
            string jsonFile = DiaglogManager.FileBrowser("export.json", "JSON files (*.json)|*.json");
            if (string.IsNullOrEmpty(jsonFile)) return;
            _IsBusy = true;
            Task.Run(() =>
            {
                try
                {
                    Operation.ImportJSON(jsonFile, this.progressBar);
                }
                catch (Exception err)
                {
                    _IsBusy = false;
                    MessageBox.Show($"An error occurred:\n\n{err.Message}", _MessageBoxTitle);
                }
            }).GetAwaiter().OnCompleted(() => { _IsBusy = false; });
        }
    }
}
