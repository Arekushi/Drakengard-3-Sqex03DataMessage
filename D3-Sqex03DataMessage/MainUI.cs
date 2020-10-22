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
using System.Runtime.InteropServices;

namespace D3_Sqex03DataMessage
{
    public partial class MainUI : Form
    {
        string _AppDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string _ConfigFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "config.json");
        string _MessageBoxTitle = "D3 Sqex03DataMessage";
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
                        MessageBox.Show($"An error occurred:\n\n{err.Message}", _MessageBoxTitle);
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
                ViewUI view = new ViewUI();
                DataMessage data = _DataMessage[index];
                view.labelFileName.Text = data.Name;
                view.labelIndex.Text = $"{index}";
                for (int i = 0; i < data.Strings.Count; i++)
                {
                    view.dataGridView.Rows.Add($"{i}", data.Strings[i]);
                }
                view.Show();
            }
            catch (Exception err)
            {
                MessageBox.Show($"An error occurred:\n\n{err.Message}", _MessageBoxTitle);
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_DataMessage.Count() < 0 || _IsBusy || listFiles.SelectedIndex <= -1) return;
            _IsBusy = true;
            int index = listFiles.SelectedIndex;
            Task.Run(() =>
            {
                try
                {
                    Operation.Export(_DataMessage[index], this.progressBar);
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
            if (_DataMessage.Count() < 0 || _IsBusy || listFiles.SelectedIndex <= -1) return;
            int index = listFiles.SelectedIndex;
            string file_name = $"[${_DataMessage[index].Index}] {_DataMessage[index].Name}";
            string file_import = DiaglogManager.FileBrowser(file_name, "Text files (*.txt)|*.txt|All files (*.*)|*.*");
            if (string.IsNullOrEmpty(file_import)) return;
            _IsBusy = true;
            Task.Run(() =>
            {
                try
                {
                    double percent = 100.0 / _DataMessage[index].Strings.Count;
                    string[] lines = File.ReadAllLines(file_import);
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
            if (_DataMessage.Count() < 0 || _IsBusy) return;
            string export_dir = DiaglogManager.FolderBrowser("Export (Directory)");
            if (string.IsNullOrEmpty(export_dir)) return;
            _IsBusy = true;
            Task.Run(() =>
            {
                try
                {
                    Operation.ExportAll(export_dir, _DataMessage, this.progressBar);
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
            if (_DataMessage.Count() < 0 || _IsBusy ) return;
            string json_file = DiaglogManager.FileBrowser("export.json", "JSON files (*.json)|*.json");
            if (string.IsNullOrEmpty(json_file)) return;
            _IsBusy = true;
            Task.Run(() =>
            {
                try
                {
                    Operation.ImportAll(json_file, this.progressBar);
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
