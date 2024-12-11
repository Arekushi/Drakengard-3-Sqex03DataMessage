using System;
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
            _Editor.labelFileName.Text = data.Name;
            _Editor.dataGridView.DataSource = null;
            _Editor.dataGridView.Rows.Clear();
            for (int i = 0; i < data.Strings.Count; i++)
            {
                string title = data.Speakers == null ? $"{i}" : data.Speakers[i].Name;
                _Editor.dataGridView.Rows.Add(title, data.Strings[i]);
            }
            return _Editor;
        }
    }
}
