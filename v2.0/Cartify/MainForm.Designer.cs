namespace Cartify
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCopyItemURL = new System.Windows.Forms.Button();
            this.btnCopyCatURL = new System.Windows.Forms.Button();
            this.lblItemURL = new System.Windows.Forms.Label();
            this.lblCatURL = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblFeedFile = new System.Windows.Forms.Label();
            this.cboProfiles = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ImportOptButton = new System.Windows.Forms.Button();
            this.importType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Result = new System.Windows.Forms.TextBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cmdAbout = new System.Windows.Forms.Button();
            this.ManageProducts = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.UploadButton = new System.Windows.Forms.Button();
            this.btnCatMapping = new System.Windows.Forms.Button();
            this.butView = new System.Windows.Forms.Button();
            this.Start = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.downloadImages = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lblTotalNow = new System.Windows.Forms.Label();
            this.lblTotalHistory = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblSource = new System.Windows.Forms.Label();
            this.lblPrice = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.imgProduct = new System.Windows.Forms.PictureBox();
            this.label6 = new System.Windows.Forms.Label();
            this.chromium = new CefSharp.WinForms.ChromiumWebBrowser();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgProduct)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnCopyItemURL);
            this.panel1.Controls.Add(this.btnCopyCatURL);
            this.panel1.Controls.Add(this.lblItemURL);
            this.panel1.Controls.Add(this.lblCatURL);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.lblFeedFile);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(155, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(721, 101);
            this.panel1.TabIndex = 0;
            // 
            // btnCopyItemURL
            // 
            this.btnCopyItemURL.Image = global::Cartify.Properties.Resources.copypathmenu;
            this.btnCopyItemURL.Location = new System.Drawing.Point(519, 65);
            this.btnCopyItemURL.Name = "btnCopyItemURL";
            this.btnCopyItemURL.Size = new System.Drawing.Size(21, 20);
            this.btnCopyItemURL.TabIndex = 20;
            this.btnCopyItemURL.UseVisualStyleBackColor = true;
            this.btnCopyItemURL.Click += new System.EventHandler(this.btnCopyItemURL_Click);
            // 
            // btnCopyCatURL
            // 
            this.btnCopyCatURL.Image = global::Cartify.Properties.Resources.copypathmenu;
            this.btnCopyCatURL.Location = new System.Drawing.Point(519, 36);
            this.btnCopyCatURL.Name = "btnCopyCatURL";
            this.btnCopyCatURL.Size = new System.Drawing.Size(21, 20);
            this.btnCopyCatURL.TabIndex = 19;
            this.btnCopyCatURL.UseVisualStyleBackColor = true;
            this.btnCopyCatURL.Click += new System.EventHandler(this.btnCopyCatURL_Click);
            // 
            // lblItemURL
            // 
            this.lblItemURL.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblItemURL.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblItemURL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblItemURL.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblItemURL.Location = new System.Drawing.Point(93, 64);
            this.lblItemURL.Name = "lblItemURL";
            this.lblItemURL.Size = new System.Drawing.Size(420, 23);
            this.lblItemURL.TabIndex = 18;
            this.lblItemURL.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblItemURL.Click += new System.EventHandler(this.lblItemURL_Click);
            // 
            // lblCatURL
            // 
            this.lblCatURL.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblCatURL.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblCatURL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCatURL.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblCatURL.Location = new System.Drawing.Point(93, 35);
            this.lblCatURL.Name = "lblCatURL";
            this.lblCatURL.Size = new System.Drawing.Size(417, 23);
            this.lblCatURL.TabIndex = 17;
            this.lblCatURL.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblCatURL.Click += new System.EventHandler(this.lblCatURL_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 64);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(33, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Item :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 37);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Category :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Last imported file :";
            // 
            // lblFeedFile
            // 
            this.lblFeedFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblFeedFile.Location = new System.Drawing.Point(96, 5);
            this.lblFeedFile.Name = "lblFeedFile";
            this.lblFeedFile.Size = new System.Drawing.Size(444, 23);
            this.lblFeedFile.TabIndex = 8;
            this.lblFeedFile.Text = "(None)";
            this.lblFeedFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cboProfiles
            // 
            this.cboProfiles.BackColor = System.Drawing.Color.White;
            this.cboProfiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProfiles.FormattingEnabled = true;
            this.cboProfiles.Location = new System.Drawing.Point(7, 27);
            this.cboProfiles.Name = "cboProfiles";
            this.cboProfiles.Size = new System.Drawing.Size(142, 21);
            this.cboProfiles.Sorted = true;
            this.cboProfiles.TabIndex = 6;
            this.cboProfiles.SelectedIndexChanged += new System.EventHandler(this.cboProfiles_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Profile";
            // 
            // ImportOptButton
            // 
            this.ImportOptButton.Location = new System.Drawing.Point(128, 75);
            this.ImportOptButton.Name = "ImportOptButton";
            this.ImportOptButton.Size = new System.Drawing.Size(21, 22);
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
            "Import Entire Site",
            "Import Categories",
            "Import Products",
            "Import Feed",
            "Quick update"});
            this.importType.Location = new System.Drawing.Point(7, 76);
            this.importType.Name = "importType";
            this.importType.Size = new System.Drawing.Size(120, 21);
            this.importType.TabIndex = 1;
            this.importType.SelectedIndexChanged += new System.EventHandler(this.importType_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Action";
            // 
            // Result
            // 
            this.Result.BackColor = System.Drawing.Color.White;
            this.Result.Location = new System.Drawing.Point(184, 302);
            this.Result.Multiline = true;
            this.Result.Name = "Result";
            this.Result.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Result.Size = new System.Drawing.Size(181, 46);
            this.Result.TabIndex = 1;
            this.Result.TextChanged += new System.EventHandler(this.Result_TextChanged);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog1";
            // 
            // webBrowser
            // 
            this.webBrowser.Location = new System.Drawing.Point(203, 138);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(94, 101);
            this.webBrowser.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.cmdAbout);
            this.panel2.Controls.Add(this.ManageProducts);
            this.panel2.Controls.Add(this.btnSettings);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.cboProfiles);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.importType);
            this.panel2.Controls.Add(this.UploadButton);
            this.panel2.Controls.Add(this.btnCatMapping);
            this.panel2.Controls.Add(this.butView);
            this.panel2.Controls.Add(this.ImportOptButton);
            this.panel2.Controls.Add(this.Start);
            this.panel2.Controls.Add(this.buttonStop);
            this.panel2.Controls.Add(this.downloadImages);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(155, 589);
            this.panel2.TabIndex = 3;
            // 
            // cmdAbout
            // 
            this.cmdAbout.Image = ((System.Drawing.Image)(resources.GetObject("cmdAbout.Image")));
            this.cmdAbout.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.cmdAbout.Location = new System.Drawing.Point(6, 406);
            this.cmdAbout.Name = "cmdAbout";
            this.cmdAbout.Size = new System.Drawing.Size(141, 24);
            this.cmdAbout.TabIndex = 15;
            this.cmdAbout.Text = "About Cartify";
            this.cmdAbout.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdAbout.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.cmdAbout.UseVisualStyleBackColor = true;
            this.cmdAbout.Click += new System.EventHandler(this.cmdAbout_Click);
            // 
            // ManageProducts
            // 
            this.ManageProducts.Image = global::Cartify.Properties.Resources.paper_dark_hollow_16x16;
            this.ManageProducts.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.ManageProducts.Location = new System.Drawing.Point(6, 353);
            this.ManageProducts.Name = "ManageProducts";
            this.ManageProducts.Size = new System.Drawing.Size(141, 24);
            this.ManageProducts.TabIndex = 14;
            this.ManageProducts.Text = "Manage Products";
            this.ManageProducts.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ManageProducts.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ManageProducts.UseVisualStyleBackColor = true;
            this.ManageProducts.Click += new System.EventHandler(this.ManageProducts_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Image = global::Cartify.Properties.Resources.Office_365_Settings_Icon_No_Background_16x16;
            this.btnSettings.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSettings.Location = new System.Drawing.Point(6, 286);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(142, 24);
            this.btnSettings.TabIndex = 13;
            this.btnSettings.Text = "Settings";
            this.btnSettings.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSettings.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // UploadButton
            // 
            this.UploadButton.Image = global::Cartify.Properties.Resources.upload;
            this.UploadButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.UploadButton.Location = new System.Drawing.Point(7, 227);
            this.UploadButton.Name = "UploadButton";
            this.UploadButton.Size = new System.Drawing.Size(142, 24);
            this.UploadButton.TabIndex = 7;
            this.UploadButton.Text = "Upload Images";
            this.UploadButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.UploadButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.UploadButton.UseVisualStyleBackColor = true;
            this.UploadButton.Click += new System.EventHandler(this.UploadButton_Click);
            // 
            // btnCatMapping
            // 
            this.btnCatMapping.Image = global::Cartify.Properties.Resources._22;
            this.btnCatMapping.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCatMapping.Location = new System.Drawing.Point(7, 259);
            this.btnCatMapping.Name = "btnCatMapping";
            this.btnCatMapping.Size = new System.Drawing.Size(142, 24);
            this.btnCatMapping.TabIndex = 11;
            this.btnCatMapping.Text = "Category Mapping";
            this.btnCatMapping.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCatMapping.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCatMapping.UseVisualStyleBackColor = true;
            this.btnCatMapping.Click += new System.EventHandler(this.btnCatMapping_Click);
            // 
            // butView
            // 
            this.butView.Image = global::Cartify.Properties.Resources.sockets;
            this.butView.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.butView.Location = new System.Drawing.Point(7, 323);
            this.butView.Name = "butView";
            this.butView.Size = new System.Drawing.Size(141, 24);
            this.butView.TabIndex = 12;
            this.butView.Text = "View Browser";
            this.butView.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.butView.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.butView.UseVisualStyleBackColor = true;
            this.butView.Click += new System.EventHandler(this.butView_Click);
            // 
            // Start
            // 
            this.Start.Enabled = false;
            this.Start.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Start.Image = global::Cartify.Properties.Resources.Start;
            this.Start.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Start.Location = new System.Drawing.Point(7, 137);
            this.Start.Name = "Start";
            this.Start.Size = new System.Drawing.Size(142, 24);
            this.Start.TabIndex = 3;
            this.Start.Text = "Start";
            this.Start.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Start.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.Start.UseVisualStyleBackColor = true;
            this.Start.Click += new System.EventHandler(this.Start_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Enabled = false;
            this.buttonStop.Image = global::Cartify.Properties.Resources.process_stop;
            this.buttonStop.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonStop.Location = new System.Drawing.Point(7, 163);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(142, 25);
            this.buttonStop.TabIndex = 10;
            this.buttonStop.Text = "Stop";
            this.buttonStop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonStop.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // downloadImages
            // 
            this.downloadImages.Image = global::Cartify.Properties.Resources.glyph_getbin;
            this.downloadImages.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.downloadImages.Location = new System.Drawing.Point(7, 199);
            this.downloadImages.Name = "downloadImages";
            this.downloadImages.Size = new System.Drawing.Size(142, 24);
            this.downloadImages.TabIndex = 4;
            this.downloadImages.Text = "Download Images";
            this.downloadImages.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.downloadImages.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.downloadImages.UseVisualStyleBackColor = true;
            this.downloadImages.Click += new System.EventHandler(this.downloadImages_Click);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.lblTotalNow);
            this.panel3.Controls.Add(this.lblTotalHistory);
            this.panel3.Controls.Add(this.label9);
            this.panel3.Controls.Add(this.label8);
            this.panel3.Controls.Add(this.label7);
            this.panel3.Controls.Add(this.lblSource);
            this.panel3.Controls.Add(this.lblPrice);
            this.panel3.Controls.Add(this.lblTitle);
            this.panel3.Controls.Add(this.imgProduct);
            this.panel3.Controls.Add(this.label6);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(690, 101);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(186, 488);
            this.panel3.TabIndex = 4;
            // 
            // lblTotalNow
            // 
            this.lblTotalNow.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalNow.Location = new System.Drawing.Point(15, 394);
            this.lblTotalNow.Name = "lblTotalNow";
            this.lblTotalNow.Size = new System.Drawing.Size(154, 36);
            this.lblTotalNow.TabIndex = 23;
            this.lblTotalNow.Text = "0";
            this.lblTotalNow.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTotalHistory
            // 
            this.lblTotalHistory.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalHistory.Location = new System.Drawing.Point(15, 325);
            this.lblTotalHistory.Name = "lblTotalHistory";
            this.lblTotalHistory.Size = new System.Drawing.Size(154, 36);
            this.lblTotalHistory.TabIndex = 22;
            this.lblTotalHistory.Text = "0";
            this.lblTotalHistory.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(15, 372);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(143, 13);
            this.label9.TabIndex = 21;
            this.label9.Text = "Total Products Imported now";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(13, 305);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(154, 13);
            this.label8.TabIndex = 20;
            this.label8.Text = "Total Products Imported So Far";
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(0, 271);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(184, 20);
            this.label7.TabIndex = 19;
            this.label7.Text = "Summary";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSource
            // 
            this.lblSource.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblSource.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblSource.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSource.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblSource.Location = new System.Drawing.Point(5, 246);
            this.lblSource.Name = "lblSource";
            this.lblSource.Size = new System.Drawing.Size(176, 23);
            this.lblSource.TabIndex = 18;
            this.lblSource.Text = "View Imported From Link";
            this.lblSource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblSource.Click += new System.EventHandler(this.lblSource_Click);
            // 
            // lblPrice
            // 
            this.lblPrice.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblPrice.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPrice.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblPrice.Location = new System.Drawing.Point(3, 222);
            this.lblPrice.Name = "lblPrice";
            this.lblPrice.Size = new System.Drawing.Size(178, 24);
            this.lblPrice.TabIndex = 3;
            this.lblPrice.Text = "$0.00";
            this.lblPrice.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTitle
            // 
            this.lblTitle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblTitle.Location = new System.Drawing.Point(5, 176);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(176, 46);
            this.lblTitle.TabIndex = 2;
            this.lblTitle.Text = "Title";
            this.lblTitle.Click += new System.EventHandler(this.lblTitle_Click);
            // 
            // imgProduct
            // 
            this.imgProduct.Image = global::Cartify.Properties.Resources.image_not_available;
            this.imgProduct.Location = new System.Drawing.Point(3, 23);
            this.imgProduct.Name = "imgProduct";
            this.imgProduct.Size = new System.Drawing.Size(178, 151);
            this.imgProduct.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imgProduct.TabIndex = 1;
            this.imgProduct.TabStop = false;
            this.imgProduct.Click += new System.EventHandler(this.imgProduct_Click);
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.label6.Dock = System.Windows.Forms.DockStyle.Top;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(0, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(184, 20);
            this.label6.TabIndex = 0;
            this.label6.Text = "Last Imported Item ";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chromium
            // 
            this.chromium.ActivateBrowserOnCreation = false;
            this.chromium.Location = new System.Drawing.Point(360, 243);
            this.chromium.Name = "chromium";
            this.chromium.Size = new System.Drawing.Size(179, 135);
            this.chromium.TabIndex = 5;
            this.chromium.LoadError += new System.EventHandler<CefSharp.LoadErrorEventArgs>(this.chromium_LoadError);
            this.chromium.LoadingStateChanged += new System.EventHandler<CefSharp.LoadingStateChangedEventArgs>(this.chromium_LoadingStateChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(876, 589);
            this.Controls.Add(this.Result);
            this.Controls.Add(this.webBrowser);
            this.Controls.Add(this.chromium);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cartify 6.7";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgProduct)).EndInit();
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
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboProfiles;
        private System.Windows.Forms.Button UploadButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Label lblFeedFile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.WebBrowser webBrowser;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button btnCatMapping;
        private System.Windows.Forms.Button butView;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblItemURL;
        private System.Windows.Forms.Label lblCatURL;
        private System.Windows.Forms.Button btnCopyCatURL;
        private System.Windows.Forms.Button btnCopyItemURL;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblPrice;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.PictureBox imgProduct;
        private System.Windows.Forms.Label lblSource;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblTotalNow;
        private System.Windows.Forms.Label lblTotalHistory;
        private System.Windows.Forms.Button ManageProducts;
        private System.Windows.Forms.Button cmdAbout;
        private CefSharp.WinForms.ChromiumWebBrowser chromium;
    }
}

