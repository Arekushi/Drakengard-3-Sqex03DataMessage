using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace D3_Sqex03DataMessage
{
    class Editor
    {
        private static Editor _Instance;
        private ViewUI _Editor;
        protected Editor()
        {
            _Editor = new ViewUI();
            _Editor.dataGridView.DefaultCellStyle.Font = new Font("Consolas", 8.5F);
        }
        public static Editor Instance()
        {
            if (_Instance == null)
            {
                _Instance = new Editor();
            }
            return _Instance;
        }
        public ViewUI TransferData(DataMessage data)
        {
            if (_Editor.labelIndex.Text == $"{data.Index}") return _Editor;
            _Editor.labelFileName.Text = data.Name;
            _Editor.labelIndex.Text = $"{data.Index}";
            _Editor.dataGridView.DataSource = null;
            _Editor.dataGridView.Rows.Clear();
            for (int i = 0; i < data.Strings.Count; i++)
            {
                _Editor.dataGridView.Rows.Add($"{i}", data.Strings[i]);
            }
            return _Editor;
        }
    }
}
