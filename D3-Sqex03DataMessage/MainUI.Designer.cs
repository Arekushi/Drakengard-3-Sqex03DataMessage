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
            this.progressBarOpen = new System.Windows.Forms.ProgressBar();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // btnSelectOriginalFiles
            // 
            this.btnSelectOriginalFiles.Location = new System.Drawing.Point(12, 41);
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
            this.labelGameLocation.Location = new System.Drawing.Point(12, 22);
            this.labelGameLocation.Name = "labelGameLocation";
            this.labelGameLocation.Size = new System.Drawing.Size(79, 13);
            this.labelGameLocation.TabIndex = 1;
            this.labelGameLocation.Text = "Game Location";
            // 
            // txtBoxGameLocation
            // 
            this.txtBoxGameLocation.Location = new System.Drawing.Point(93, 43);
            this.txtBoxGameLocation.Name = "txtBoxGameLocation";
            this.txtBoxGameLocation.ReadOnly = true;
            this.txtBoxGameLocation.Size = new System.Drawing.Size(379, 20);
            this.txtBoxGameLocation.TabIndex = 2;
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(12, 137);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(460, 23);
            this.btnOpen.TabIndex = 6;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnPreview
            // 
            this.btnPreview.Location = new System.Drawing.Point(12, 218);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(460, 23);
            this.btnPreview.TabIndex = 7;
            this.btnPreview.Text = "Preview / Edit";
            this.btnPreview.UseVisualStyleBackColor = true;
            // 
            // btnReimport
            // 
            this.btnReimport.Location = new System.Drawing.Point(11, 175);
            this.btnReimport.Name = "btnReimport";
            this.btnReimport.Size = new System.Drawing.Size(460, 23);
            this.btnReimport.TabIndex = 8;
            this.btnReimport.Text = "Re-Import";
            this.btnReimport.UseVisualStyleBackColor = true;
            this.btnReimport.Click += new System.EventHandler(this.btnReimport_Click);
            // 
            // progressBarOpen
            // 
            this.progressBarOpen.Location = new System.Drawing.Point(13, 161);
            this.progressBarOpen.Name = "progressBarOpen";
            this.progressBarOpen.Size = new System.Drawing.Size(458, 5);
            this.progressBarOpen.TabIndex = 10;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 200);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(458, 5);
            this.progressBar1.TabIndex = 11;
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(12, 260);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(460, 212);
            this.listBox1.TabIndex = 9;
            // 
            // MainUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 511);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.progressBarOpen);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.btnReimport);
            this.Controls.Add(this.btnPreview);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.txtBoxGameLocation);
            this.Controls.Add(this.labelGameLocation);
            this.Controls.Add(this.btnSelectOriginalFiles);
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
        private System.Windows.Forms.ProgressBar progressBarOpen;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ListBox listBox1;
    }
}

