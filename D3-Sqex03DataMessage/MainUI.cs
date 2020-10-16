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

        public void ProgressBar(int percent)
        {
            this.progressBar.BeginInvoke((MethodInvoker)delegate
            {
                progressBar.Value = percent > 100 ? 100 : percent;
            });
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
                            Show_Archives();
                        }
                        _IsBusy = false;
                    }
                    catch (Exception err)
                    {
                        _IsBusy = false;
                        _DataMessage.Clear();
                        MessageBox.Show($"An error occurred:\n\n{err.Message}", _MessageBoxTitle);
                    }
                });
            } 
        }

        private void Show_Archives()
        {
            this.listFiles.BeginInvoke((MethodInvoker)delegate ()
            {
                listFiles.Items.Clear();
                foreach (DataMessage data in _DataMessage)
                {
                    listFiles.Items.Add($"[{data.Index}] - {data.Name}");
                }
            });
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

        private void listFiles_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Open_Preview();
        }

        private void Open_Preview()
        {
            if (listFiles.SelectedIndex < 0) return;
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

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (_DataMessage.Count() < 1 || _IsBusy)
            {
                string msg = _IsBusy ? "Another task is already in progress." : "There's no data to export.";
                MessageBox.Show(msg, _MessageBoxTitle);
            }
            else
            {
                _IsBusy = true;
                string export_dir = FolderBrowser.FolderBrowserDialog("Export (Directory)");
                if (!string.IsNullOrEmpty(export_dir))
                {
                    Task.Run(() =>
                    {
                        try
                        {
                            Operation.Export(export_dir, _DataMessage);
                            _IsBusy = false;
                        }
                        catch (Exception err)
                        {
                            _IsBusy = false;
                            MessageBox.Show($"An error occurred:\n\n{err.Message}", _MessageBoxTitle);
                        }
                    });
                }
            }
            
        }
    }
}
