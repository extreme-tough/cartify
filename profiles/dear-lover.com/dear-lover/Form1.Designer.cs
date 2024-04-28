namespace AllImporterPro
{
    partial class Form1
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.downloadImages = new System.Windows.Forms.Button();
            this.Start = new System.Windows.Forms.Button();
            this.ImportOptButton = new System.Windows.Forms.Button();
            this.importType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Result = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.downloadImages);
            this.panel1.Controls.Add(this.Start);
            this.panel1.Controls.Add(this.ImportOptButton);
            this.panel1.Controls.Add(this.importType);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(853, 33);
            this.panel1.TabIndex = 0;
            // 
            // downloadImages
            // 
            this.downloadImages.Location = new System.Drawing.Point(523, -1);
            this.downloadImages.Name = "downloadImages";
            this.downloadImages.Size = new System.Drawing.Size(105, 28);
            this.downloadImages.TabIndex = 4;
            this.downloadImages.Text = "Download Images";
            this.downloadImages.UseVisualStyleBackColor = true;
            this.downloadImages.Visible = false;
            this.downloadImages.Click += new System.EventHandler(this.downloadImages_Click);
            // 
            // Start
            // 
            this.Start.Location = new System.Drawing.Point(745, 1);
            this.Start.Name = "Start";
            this.Start.Size = new System.Drawing.Size(105, 28);
            this.Start.TabIndex = 3;
            this.Start.Text = "Start Import";
            this.Start.UseVisualStyleBackColor = true;
            this.Start.Click += new System.EventHandler(this.Start_Click);
            // 
            // ImportOptButton
            // 
            this.ImportOptButton.Location = new System.Drawing.Point(352, 5);
            this.ImportOptButton.Name = "ImportOptButton";
            this.ImportOptButton.Size = new System.Drawing.Size(24, 23);
            this.ImportOptButton.TabIndex = 2;
            this.ImportOptButton.Text = "...";
            this.ImportOptButton.UseVisualStyleBackColor = true;
            this.ImportOptButton.Click += new System.EventHandler(this.ImportOptButton_Click);
            // 
            // importType
            // 
            this.importType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.importType.FormattingEnabled = true;
            this.importType.Items.AddRange(new object[] {
            "Import entire products from site",
            "Import from Category URLs",
            "Import from Product URLs"});
            this.importType.Location = new System.Drawing.Point(71, 6);
            this.importType.Name = "importType";
            this.importType.Size = new System.Drawing.Size(276, 21);
            this.importType.TabIndex = 1;
            this.importType.SelectedIndexChanged += new System.EventHandler(this.importType_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Import";
            // 
            // Result
            // 
            this.Result.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Result.Location = new System.Drawing.Point(0, 33);
            this.Result.Multiline = true;
            this.Result.Name = "Result";
            this.Result.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Result.Size = new System.Drawing.Size(853, 414);
            this.Result.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(853, 447);
            this.Controls.Add(this.Result);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "All Importer Pro 1.5";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox importType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ImportOptButton;
        private System.Windows.Forms.Button Start;
        private System.Windows.Forms.TextBox Result;
        private System.Windows.Forms.Button downloadImages;
    }
}

