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
            int width = dataGridView.Width;
            //dataGridView.Columns[0].Width = (int)(width * 0.25);
            //dataGridView.Columns[1].Width = (int)(width * 0.75);
            dataGridView.Columns[1].Width = width - 170;
        }

        private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string fileName = labelFileName.Text;
            int index = Array.FindIndex(MainUI._DataMessage.ToArray(), element => element.Name == fileName);
            if (e.RowIndex > MainUI._DataMessage[index].Strings.Count-1) return;
            MainUI._DataMessage[index].Strings[e.RowIndex] = $"{dataGridView.Rows[e.RowIndex].Cells[1].Value}";
        }

        private void ViewUI_FromClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
