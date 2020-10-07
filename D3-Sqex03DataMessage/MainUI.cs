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
        ArchiveConfig _Config = new ArchiveConfig();
        string _AppDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public MainUI()
        {
            InitializeComponent();
        }

        private void MainUI_Load(object sender, EventArgs e)
        {

        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            
        }

        private void btnReimport_Click(object sender, EventArgs e)
        {
            
        }
    }
}
