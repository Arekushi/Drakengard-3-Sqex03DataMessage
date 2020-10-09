namespace D3_Sqex03DataMessage
{
    partial class MainUI
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
            this.btnSelectOriginalFiles = new System.Windows.Forms.Button();
            this.labelGameLocation = new System.Windows.Forms.Label();
            this.txtBoxGameLocation = new System.Windows.Forms.TextBox();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnReimport = new System.Windows.Forms.Button();
            this.listFiles = new System.Windows.Forms.ListBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSelectOriginalFiles
            // 
            this.btnSelectOriginalFiles.Location = new System.Drawing.Point(12, 38);
            this.btnSelectOriginalFiles.Name = "btnSelectOriginalFiles";
            this.btnSelectOriginalFiles.Size = new System.Drawing.Size(75, 23);
            this.btnSelectOriginalFiles.TabIndex = 0;
            this.btnSelectOriginalFiles.Text = "Select";
            this.btnSelectOriginalFiles.UseVisualStyleBackColor = true;
            this.btnSelectOriginalFiles.Click += new System.EventHandler(this.btnSelectGameLocation_Click);
            // 
            // labelGameLocation
            // 
            this.labelGameLocation.AutoSize = true;
            this.labelGameLocation.Location = new System.Drawing.Point(12, 17);
            this.labelGameLocation.Name = "labelGameLocation";
            this.labelGameLocation.Size = new System.Drawing.Size(79, 13);
            this.labelGameLocation.TabIndex = 1;
            this.labelGameLocation.Text = "Game Location";
            // 
            // txtBoxGameLocation
            // 
            this.txtBoxGameLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBoxGameLocation.Location = new System.Drawing.Point(93, 40);
            this.txtBoxGameLocation.Name = "txtBoxGameLocation";
            this.txtBoxGameLocation.ReadOnly = true;
            this.txtBoxGameLocation.Size = new System.Drawing.Size(379, 20);
            this.txtBoxGameLocation.TabIndex = 2;
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(12, 79);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(228, 23);
            this.btnOpen.TabIndex = 6;
            this.btnOpen.Text = "Open / Decrypt";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnPreview
            // 
            this.btnPreview.Location = new System.Drawing.Point(244, 79);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(228, 23);
            this.btnPreview.TabIndex = 7;
            this.btnPreview.Text = "Preview / Edit";
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // btnReimport
            // 
            this.btnReimport.Location = new System.Drawing.Point(244, 106);
            this.btnReimport.Name = "btnReimport";
            this.btnReimport.Size = new System.Drawing.Size(228, 23);
            this.btnReimport.TabIndex = 8;
            this.btnReimport.Text = "Re-Import";
            this.btnReimport.UseVisualStyleBackColor = true;
            this.btnReimport.Click += new System.EventHandler(this.btnReimport_Click);
            // 
            // listFiles
            // 
            this.listFiles.FormattingEnabled = true;
            this.listFiles.Location = new System.Drawing.Point(12, 144);
            this.listFiles.Name = "listFiles";
            this.listFiles.Size = new System.Drawing.Size(456, 329);
            this.listFiles.TabIndex = 9;
            this.listFiles.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listFiles_MouseDoubleClick);
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(12, 106);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(228, 23);
            this.btnExport.TabIndex = 10;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            // 
            // MainUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 511);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.listFiles);
            this.Controls.Add(this.btnReimport);
            this.Controls.Add(this.btnPreview);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.txtBoxGameLocation);
            this.Controls.Add(this.labelGameLocation);
            this.Controls.Add(this.btnSelectOriginalFiles);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(500, 550);
            this.MinimumSize = new System.Drawing.Size(500, 550);
            this.Name = "MainUI";
            this.Text = "Drakengard 3 - Sqex03DataMessage";
            this.Load += new System.EventHandler(this.MainUI_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSelectOriginalFiles;
        private System.Windows.Forms.Label labelGameLocation;
        private System.Windows.Forms.TextBox txtBoxGameLocation;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnReimport;
        private System.Windows.Forms.ListBox listFiles;
        private System.Windows.Forms.Button btnExport;
    }
}

