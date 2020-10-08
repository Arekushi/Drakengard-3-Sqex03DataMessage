namespace D3_Sqex03DataMessage
{
    partial class ViewUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listView = new System.Windows.Forms.ListView();
            this.columnLine = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnEnglish = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnTranslation = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.labelFileNameTitle = new System.Windows.Forms.Label();
            this.labelFileName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listView
            // 
            this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnLine,
            this.columnEnglish,
            this.columnTranslation});
            this.listView.HideSelection = false;
            this.listView.Location = new System.Drawing.Point(12, 34);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(560, 415);
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            // 
            // columnLine
            // 
            this.columnLine.Text = "Line";
            this.columnLine.Width = 55;
            // 
            // columnEnglish
            // 
            this.columnEnglish.Text = "English / Japanese";
            this.columnEnglish.Width = 250;
            // 
            // columnTranslation
            // 
            this.columnTranslation.Text = "Translation";
            this.columnTranslation.Width = 250;
            // 
            // labelFileNameTitle
            // 
            this.labelFileNameTitle.AutoSize = true;
            this.labelFileNameTitle.Location = new System.Drawing.Point(12, 9);
            this.labelFileNameTitle.Name = "labelFileNameTitle";
            this.labelFileNameTitle.Size = new System.Drawing.Size(57, 13);
            this.labelFileNameTitle.TabIndex = 1;
            this.labelFileNameTitle.Text = "File Name:";
            // 
            // labelFileName
            // 
            this.labelFileName.AutoSize = true;
            this.labelFileName.Location = new System.Drawing.Point(75, 9);
            this.labelFileName.Name = "labelFileName";
            this.labelFileName.Size = new System.Drawing.Size(22, 13);
            this.labelFileName.TabIndex = 2;
            this.labelFileName.Text = "NA";
            // 
            // ViewUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 461);
            this.Controls.Add(this.labelFileName);
            this.Controls.Add(this.labelFileNameTitle);
            this.Controls.Add(this.listView);
            this.MinimumSize = new System.Drawing.Size(600, 200);
            this.Name = "ViewUI";
            this.Text = "Editor";
            this.Load += new System.EventHandler(this.ViewUI_Load);
            this.Resize += new System.EventHandler(this.View_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ColumnHeader columnLine;
        private System.Windows.Forms.ColumnHeader columnEnglish;
        private System.Windows.Forms.ColumnHeader columnTranslation;
        private System.Windows.Forms.Label labelFileNameTitle;
        public System.Windows.Forms.ListView listView;
        public System.Windows.Forms.Label labelFileName;
    }
}