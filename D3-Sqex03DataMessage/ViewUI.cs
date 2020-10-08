using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace D3_Sqex03DataMessage
{
    public partial class ViewUI : Form
    {
        public ViewUI()
        {
            InitializeComponent();
        }

        private void ViewUI_Load(object sender, EventArgs e)
        {

        }

        private void View_Resize(object sender, EventArgs e)
        {
            int width = listView.Width;
            if (width < 560 && width > 0) return;
            listView.Columns[0].Width = (width / 10) - 4;
            listView.Columns[1].Width = (width - (width / 10)) / 2;
            listView.Columns[2].Width = (width - (width / 10)) / 2;
        }
    }
}
