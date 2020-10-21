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
            dataGridView.Columns[1].Width = width - 60;
        }

        private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int index = int.Parse(labelIndex.Text);
            if (e.RowIndex > MainUI._DataMessage[index].Strings.Count-1) return;
            MainUI._DataMessage[index].Strings[e.RowIndex] = $"{dataGridView.Rows[e.RowIndex].Cells[1].Value}";
        }
    }
}
