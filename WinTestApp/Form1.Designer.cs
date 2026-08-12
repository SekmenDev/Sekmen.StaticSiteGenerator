namespace WinTestApp;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
        tabControl1 = new System.Windows.Forms.TabControl();
        tabExporter = new System.Windows.Forms.TabPage();
        tabTester = new System.Windows.Forms.TabPage();
        
        // Exporter controls
        label1 = new System.Windows.Forms.Label();
        label2 = new System.Windows.Forms.Label();
        label3 = new System.Windows.Forms.Label();
        label4 = new System.Windows.Forms.Label();
        label5 = new System.Windows.Forms.Label();
        label6 = new System.Windows.Forms.Label();
        
        txtSiteUrl = new System.Windows.Forms.TextBox();
        txtTargetUrl = new System.Windows.Forms.TextBox();
        txtOutputFolder = new System.Windows.Forms.TextBox();
        txtAdditionalUrls = new System.Windows.Forms.TextBox();
        btnBrowseFolder = new System.Windows.Forms.Button();
        
        dgvReplacements = new System.Windows.Forms.DataGridView();
        colOldValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
        colNewValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
        btnAddReplacement = new System.Windows.Forms.Button();
        btnRemoveReplacement = new System.Windows.Forms.Button();
        
        btnExport = new System.Windows.Forms.Button();
        btnOpenFolder = new System.Windows.Forms.Button();
        btnClearLog = new System.Windows.Forms.Button();
        
        progressBar1 = new System.Windows.Forms.ProgressBar();
        txtLog = new System.Windows.Forms.TextBox();
        
        // Tester controls
        grpUrlValidation = new System.Windows.Forms.GroupBox();
        lblInputUrl = new System.Windows.Forms.Label();
        txtValidationInputUrl = new System.Windows.Forms.TextBox();
        lblBaseUri = new System.Windows.Forms.Label();
        txtValidationBaseUri = new System.Windows.Forms.TextBox();
        btnRunValidationTest = new System.Windows.Forms.Button();
        txtValidationOutput = new System.Windows.Forms.TextBox();
        
        grpHtmlRewriter = new System.Windows.Forms.GroupBox();
        lblRewriterSource = new System.Windows.Forms.Label();
        txtRewriterSource = new System.Windows.Forms.TextBox();
        lblRewriterTarget = new System.Windows.Forms.Label();
        txtRewriterTarget = new System.Windows.Forms.TextBox();
        lblHtmlInput = new System.Windows.Forms.Label();
        txtHtmlInput = new System.Windows.Forms.TextBox();
        btnRunRewriterTest = new System.Windows.Forms.Button();
        txtHtmlOutput = new System.Windows.Forms.TextBox();
        
        grpExtractor = new System.Windows.Forms.GroupBox();
        lblExtractorBase = new System.Windows.Forms.Label();
        txtExtractorBase = new System.Windows.Forms.TextBox();
        lblExtractorHtml = new System.Windows.Forms.Label();
        txtExtractorHtml = new System.Windows.Forms.TextBox();
        btnRunExtractorTest = new System.Windows.Forms.Button();
        txtExtractorOutput = new System.Windows.Forms.TextBox();

        tabControl1.SuspendLayout();
        tabExporter.SuspendLayout();
        tabTester.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvReplacements).BeginInit();
        grpUrlValidation.SuspendLayout();
        grpHtmlRewriter.SuspendLayout();
        grpExtractor.SuspendLayout();
        SuspendLayout();

        // 
        // tabControl1
        // 
        tabControl1.Controls.Add(tabExporter);
        tabControl1.Controls.Add(tabTester);
        tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
        tabControl1.Location = new System.Drawing.Point(0, 0);
        tabControl1.Name = "tabControl1";
        tabControl1.SelectedIndex = 0;
        tabControl1.Size = new System.Drawing.Size(960, 720);
        tabControl1.TabIndex = 0;

        // 
        // tabExporter
        // 
        tabExporter.Controls.Add(label1);
        tabExporter.Controls.Add(txtSiteUrl);
        tabExporter.Controls.Add(label2);
        tabExporter.Controls.Add(txtTargetUrl);
        tabExporter.Controls.Add(label3);
        tabExporter.Controls.Add(txtOutputFolder);
        tabExporter.Controls.Add(btnBrowseFolder);
        tabExporter.Controls.Add(label4);
        tabExporter.Controls.Add(txtAdditionalUrls);
        tabExporter.Controls.Add(label5);
        tabExporter.Controls.Add(dgvReplacements);
        tabExporter.Controls.Add(btnAddReplacement);
        tabExporter.Controls.Add(btnRemoveReplacement);
        tabExporter.Controls.Add(btnExport);
        tabExporter.Controls.Add(btnOpenFolder);
        tabExporter.Controls.Add(btnClearLog);
        tabExporter.Controls.Add(progressBar1);
        tabExporter.Controls.Add(label6);
        tabExporter.Controls.Add(txtLog);
        tabExporter.Location = new System.Drawing.Point(4, 24);
        tabExporter.Name = "tabExporter";
        tabExporter.Padding = new System.Windows.Forms.Padding(10);
        tabExporter.Size = new System.Drawing.Size(952, 692);
        tabExporter.TabIndex = 0;
        tabExporter.Text = "Site Exporter";
        tabExporter.UseVisualStyleBackColor = true;

        // 
        // label1: Site URL
        // 
        label1.AutoSize = true;
        label1.Location = new System.Drawing.Point(15, 18);
        label1.Name = "label1";
        label1.Size = new System.Drawing.Size(91, 15);
        label1.Text = "Source Site URL:";
        // 
        // txtSiteUrl
        // 
        txtSiteUrl.Location = new System.Drawing.Point(130, 15);
        txtSiteUrl.Name = "txtSiteUrl";
        txtSiteUrl.Size = new System.Drawing.Size(320, 23);
        txtSiteUrl.TabIndex = 1;

        // 
        // label2: Target URL
        // 
        label2.AutoSize = true;
        label2.Location = new System.Drawing.Point(470, 18);
        label2.Name = "label2";
        label2.Size = new System.Drawing.Size(87, 15);
        label2.Text = "Target Base URL:";
        // 
        // txtTargetUrl
        // 
        txtTargetUrl.Location = new System.Drawing.Point(575, 15);
        txtTargetUrl.Name = "txtTargetUrl";
        txtTargetUrl.Size = new System.Drawing.Size(350, 23);
        txtTargetUrl.TabIndex = 2;

        // 
        // label3: Output Folder
        // 
        label3.AutoSize = true;
        label3.Location = new System.Drawing.Point(15, 53);
        label3.Name = "label3";
        label3.Size = new System.Drawing.Size(84, 15);
        label3.Text = "Output Directory:";
        // 
        // txtOutputFolder
        // 
        txtOutputFolder.Location = new System.Drawing.Point(130, 50);
        txtOutputFolder.Name = "txtOutputFolder";
        txtOutputFolder.Size = new System.Drawing.Size(695, 23);
        txtOutputFolder.TabIndex = 3;
        // 
        // btnBrowseFolder
        // 
        btnBrowseFolder.Location = new System.Drawing.Point(835, 49);
        btnBrowseFolder.Name = "btnBrowseFolder";
        btnBrowseFolder.Size = new System.Drawing.Size(90, 25);
        btnBrowseFolder.TabIndex = 4;
        btnBrowseFolder.Text = "Browse...";
        btnBrowseFolder.UseVisualStyleBackColor = true;
        btnBrowseFolder.Click += btnBrowseFolder_Click;

        // 
        // label4: Additional URLs
        // 
        label4.AutoSize = true;
        label4.Location = new System.Drawing.Point(15, 88);
        label4.Name = "label4";
        label4.Size = new System.Drawing.Size(107, 15);
        label4.Text = "Additional URLs (1/line):";
        // 
        // txtAdditionalUrls
        // 
        txtAdditionalUrls.Location = new System.Drawing.Point(130, 85);
        txtAdditionalUrls.Multiline = true;
        txtAdditionalUrls.Name = "txtAdditionalUrls";
        txtAdditionalUrls.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        txtAdditionalUrls.Size = new System.Drawing.Size(320, 100);
        txtAdditionalUrls.TabIndex = 5;

        // 
        // label5: String Replacements
        // 
        label5.AutoSize = true;
        label5.Location = new System.Drawing.Point(470, 88);
        label5.Name = "label5";
        label5.Size = new System.Drawing.Size(103, 15);
        label5.Text = "String Replacements:";
        // 
        // dgvReplacements
        // 
        dgvReplacements.AllowUserToAddRows = false;
        dgvReplacements.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
        dgvReplacements.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvReplacements.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { colOldValue, colNewValue });
        dgvReplacements.Location = new System.Drawing.Point(575, 85);
        dgvReplacements.Name = "dgvReplacements";
        dgvReplacements.RowHeadersWidth = 25;
        dgvReplacements.Size = new System.Drawing.Size(350, 72);
        dgvReplacements.TabIndex = 6;
        // 
        // colOldValue
        // 
        colOldValue.HeaderText = "Old Value";
        colOldValue.Name = "colOldValue";
        // 
        // colNewValue
        // 
        colNewValue.HeaderText = "New Value";
        colNewValue.Name = "colNewValue";
        // 
        // btnAddReplacement
        // 
        btnAddReplacement.Location = new System.Drawing.Point(575, 161);
        btnAddReplacement.Name = "btnAddReplacement";
        btnAddReplacement.Size = new System.Drawing.Size(120, 24);
        btnAddReplacement.TabIndex = 7;
        btnAddReplacement.Text = "+ Add Row";
        btnAddReplacement.UseVisualStyleBackColor = true;
        btnAddReplacement.Click += btnAddReplacement_Click;
        // 
        // btnRemoveReplacement
        // 
        btnRemoveReplacement.Location = new System.Drawing.Point(705, 161);
        btnRemoveReplacement.Name = "btnRemoveReplacement";
        btnRemoveReplacement.Size = new System.Drawing.Size(120, 24);
        btnRemoveReplacement.TabIndex = 8;
        btnRemoveReplacement.Text = "- Remove Selected";
        btnRemoveReplacement.UseVisualStyleBackColor = true;
        btnRemoveReplacement.Click += btnRemoveReplacement_Click;

        // 
        // btnExport
        // 
        btnExport.BackColor = System.Drawing.Color.FromArgb(0, 122, 204);
        btnExport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        btnExport.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
        btnExport.ForeColor = System.Drawing.Color.White;
        btnExport.Location = new System.Drawing.Point(130, 200);
        btnExport.Name = "btnExport";
        btnExport.Size = new System.Drawing.Size(220, 40);
        btnExport.TabIndex = 9;
        btnExport.Text = "🚀 Export Static Site";
        btnExport.UseVisualStyleBackColor = false;
        btnExport.Click += btnExport_Click;

        // 
        // btnOpenFolder
        // 
        btnOpenFolder.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        btnOpenFolder.Location = new System.Drawing.Point(365, 200);
        btnOpenFolder.Name = "btnOpenFolder";
        btnOpenFolder.Size = new System.Drawing.Size(160, 40);
        btnOpenFolder.TabIndex = 10;
        btnOpenFolder.Text = "📁 Open Output Folder";
        btnOpenFolder.UseVisualStyleBackColor = true;
        btnOpenFolder.Click += btnOpenFolder_Click;

        // 
        // btnClearLog
        // 
        btnClearLog.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        btnClearLog.Location = new System.Drawing.Point(535, 200);
        btnClearLog.Name = "btnClearLog";
        btnClearLog.Size = new System.Drawing.Size(120, 40);
        btnClearLog.TabIndex = 11;
        btnClearLog.Text = "🧹 Clear Log";
        btnClearLog.UseVisualStyleBackColor = true;
        btnClearLog.Click += btnClearLog_Click;

        // 
        // progressBar1
        // 
        progressBar1.Location = new System.Drawing.Point(15, 250);
        progressBar1.Name = "progressBar1";
        progressBar1.Size = new System.Drawing.Size(910, 15);
        progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
        progressBar1.TabIndex = 12;
        progressBar1.Visible = false;

        // 
        // label6: Console Output Log
        // 
        label6.AutoSize = true;
        label6.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
        label6.Location = new System.Drawing.Point(15, 275);
        label6.Name = "label6";
        label6.Size = new System.Drawing.Size(130, 17);
        label6.Text = "Execution & Event Log:";
        // 
        // txtLog
        // 
        txtLog.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
        txtLog.Font = new System.Drawing.Font("Consolas", 9.75F);
        txtLog.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
        txtLog.Location = new System.Drawing.Point(15, 298);
        txtLog.Multiline = true;
        txtLog.Name = "txtLog";
        txtLog.ReadOnly = true;
        txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
        txtLog.Size = new System.Drawing.Size(910, 370);
        txtLog.TabIndex = 13;

        // 
        // tabTester
        // 
        tabTester.Controls.Add(grpUrlValidation);
        tabTester.Controls.Add(grpHtmlRewriter);
        tabTester.Controls.Add(grpExtractor);
        tabTester.Location = new System.Drawing.Point(4, 24);
        tabTester.Name = "tabTester";
        tabTester.Padding = new System.Windows.Forms.Padding(10);
        tabTester.Size = new System.Drawing.Size(952, 692);
        tabTester.TabIndex = 1;
        tabTester.Text = "Package Feature Tester";
        tabTester.UseVisualStyleBackColor = true;

        // 
        // grpUrlValidation
        // 
        grpUrlValidation.Controls.Add(lblInputUrl);
        grpUrlValidation.Controls.Add(txtValidationInputUrl);
        grpUrlValidation.Controls.Add(lblBaseUri);
        grpUrlValidation.Controls.Add(txtValidationBaseUri);
        grpUrlValidation.Controls.Add(btnRunValidationTest);
        grpUrlValidation.Controls.Add(txtValidationOutput);
        grpUrlValidation.Location = new System.Drawing.Point(15, 10);
        grpUrlValidation.Name = "grpUrlValidation";
        grpUrlValidation.Size = new System.Drawing.Size(915, 190);
        grpUrlValidation.TabIndex = 0;
        grpUrlValidation.TabStop = false;
        grpUrlValidation.Text = "1. UrlValidation Component Tester";

        // lblInputUrl
        lblInputUrl.AutoSize = true;
        lblInputUrl.Location = new System.Drawing.Point(15, 25);
        lblInputUrl.Name = "lblInputUrl";
        lblInputUrl.Size = new System.Drawing.Size(100, 15);
        lblInputUrl.Text = "Test URL / Path:";
        // txtValidationInputUrl
        txtValidationInputUrl.Location = new System.Drawing.Point(120, 22);
        txtValidationInputUrl.Name = "txtValidationInputUrl";
        txtValidationInputUrl.Size = new System.Drawing.Size(320, 23);
        txtValidationInputUrl.TabIndex = 1;
        txtValidationInputUrl.Text = "/assets/styles.css";

        // lblBaseUri
        lblBaseUri.AutoSize = true;
        lblBaseUri.Location = new System.Drawing.Point(460, 25);
        lblBaseUri.Name = "lblBaseUri";
        lblBaseUri.Size = new System.Drawing.Size(56, 15);
        lblBaseUri.Text = "Base URI:";
        // txtValidationBaseUri
        txtValidationBaseUri.Location = new System.Drawing.Point(530, 22);
        txtValidationBaseUri.Name = "txtValidationBaseUri";
        txtValidationBaseUri.Size = new System.Drawing.Size(365, 23);
        txtValidationBaseUri.TabIndex = 2;
        txtValidationBaseUri.Text = "https://example.com/";

        // btnRunValidationTest
        btnRunValidationTest.Location = new System.Drawing.Point(120, 52);
        btnRunValidationTest.Name = "btnRunValidationTest";
        btnRunValidationTest.Size = new System.Drawing.Size(180, 26);
        btnRunValidationTest.TabIndex = 3;
        btnRunValidationTest.Text = "Evaluate UrlValidation";
        btnRunValidationTest.UseVisualStyleBackColor = true;
        btnRunValidationTest.Click += btnRunValidationTest_Click;

        // txtValidationOutput
        txtValidationOutput.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
        txtValidationOutput.Font = new System.Drawing.Font("Consolas", 9.25F);
        txtValidationOutput.Location = new System.Drawing.Point(120, 85);
        txtValidationOutput.Multiline = true;
        txtValidationOutput.Name = "txtValidationOutput";
        txtValidationOutput.ReadOnly = true;
        txtValidationOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        txtValidationOutput.Size = new System.Drawing.Size(775, 95);
        txtValidationOutput.TabIndex = 4;

        // 
        // grpHtmlRewriter
        // 
        grpHtmlRewriter.Controls.Add(lblRewriterSource);
        grpHtmlRewriter.Controls.Add(txtRewriterSource);
        grpHtmlRewriter.Controls.Add(lblRewriterTarget);
        grpHtmlRewriter.Controls.Add(txtRewriterTarget);
        grpHtmlRewriter.Controls.Add(lblHtmlInput);
        grpHtmlRewriter.Controls.Add(txtHtmlInput);
        grpHtmlRewriter.Controls.Add(btnRunRewriterTest);
        grpHtmlRewriter.Controls.Add(txtHtmlOutput);
        grpHtmlRewriter.Location = new System.Drawing.Point(15, 210);
        grpHtmlRewriter.Name = "grpHtmlRewriter";
        grpHtmlRewriter.Size = new System.Drawing.Size(915, 220);
        grpHtmlRewriter.TabIndex = 1;
        grpHtmlRewriter.TabStop = false;
        grpHtmlRewriter.Text = "2. UrlHelpers.UpdateHtmlUrls Rewriter Tester";

        // lblRewriterSource
        lblRewriterSource.AutoSize = true;
        lblRewriterSource.Location = new System.Drawing.Point(15, 25);
        lblRewriterSource.Name = "lblRewriterSource";
        lblRewriterSource.Size = new System.Drawing.Size(66, 15);
        lblRewriterSource.Text = "Source URL:";
        // txtRewriterSource
        txtRewriterSource.Location = new System.Drawing.Point(90, 22);
        txtRewriterSource.Name = "txtRewriterSource";
        txtRewriterSource.Size = new System.Drawing.Size(350, 23);
        txtRewriterSource.TabIndex = 1;
        txtRewriterSource.Text = "https://mysite.com/";

        // lblRewriterTarget
        lblRewriterTarget.AutoSize = true;
        lblRewriterTarget.Location = new System.Drawing.Point(460, 25);
        lblRewriterTarget.Name = "lblRewriterTarget";
        lblRewriterTarget.Size = new System.Drawing.Size(63, 15);
        lblRewriterTarget.Text = "Target URL:";
        // txtRewriterTarget
        txtRewriterTarget.Location = new System.Drawing.Point(530, 22);
        txtRewriterTarget.Name = "txtRewriterTarget";
        txtRewriterTarget.Size = new System.Drawing.Size(365, 23);
        txtRewriterTarget.TabIndex = 2;
        txtRewriterTarget.Text = "https://static.mysite.com/";

        // lblHtmlInput
        lblHtmlInput.AutoSize = true;
        lblHtmlInput.Location = new System.Drawing.Point(15, 55);
        lblHtmlInput.Name = "lblHtmlInput";
        lblHtmlInput.Size = new System.Drawing.Size(68, 15);
        lblHtmlInput.Text = "Input HTML:";
        // txtHtmlInput
        txtHtmlInput.Font = new System.Drawing.Font("Consolas", 9F);
        txtHtmlInput.Location = new System.Drawing.Point(90, 52);
        txtHtmlInput.Multiline = true;
        txtHtmlInput.Name = "txtHtmlInput";
        txtHtmlInput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        txtHtmlInput.Size = new System.Drawing.Size(350, 120);
        txtHtmlInput.TabIndex = 3;
        txtHtmlInput.Text = "<a href=\"https://mysite.com/about\">About</a>\r\n<img src=\"/images/logo.png\" />\r\n<link rel=\"stylesheet\" href=\"https://mysite.com/css/main.css\" />";

        // btnRunRewriterTest
        btnRunRewriterTest.Location = new System.Drawing.Point(90, 180);
        btnRunRewriterTest.Name = "btnRunRewriterTest";
        btnRunRewriterTest.Size = new System.Drawing.Size(180, 26);
        btnRunRewriterTest.TabIndex = 4;
        btnRunRewriterTest.Text = "Rewrite HTML URLs";
        btnRunRewriterTest.UseVisualStyleBackColor = true;
        btnRunRewriterTest.Click += btnRunRewriterTest_Click;

        // txtHtmlOutput
        txtHtmlOutput.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
        txtHtmlOutput.Font = new System.Drawing.Font("Consolas", 9F);
        txtHtmlOutput.Location = new System.Drawing.Point(460, 52);
        txtHtmlOutput.Multiline = true;
        txtHtmlOutput.Name = "txtHtmlOutput";
        txtHtmlOutput.ReadOnly = true;
        txtHtmlOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        txtHtmlOutput.Size = new System.Drawing.Size(435, 154);
        txtHtmlOutput.TabIndex = 5;

        // 
        // grpExtractor
        // 
        grpExtractor.Controls.Add(lblExtractorBase);
        grpExtractor.Controls.Add(txtExtractorBase);
        grpExtractor.Controls.Add(lblExtractorHtml);
        grpExtractor.Controls.Add(txtExtractorHtml);
        grpExtractor.Controls.Add(btnRunExtractorTest);
        grpExtractor.Controls.Add(txtExtractorOutput);
        grpExtractor.Location = new System.Drawing.Point(15, 440);
        grpExtractor.Name = "grpExtractor";
        grpExtractor.Size = new System.Drawing.Size(915, 235);
        grpExtractor.TabIndex = 2;
        grpExtractor.TabStop = false;
        grpExtractor.Text = "3. Extractor.ExtractResourceUrls Asset Extractor Tester";

        // lblExtractorBase
        lblExtractorBase.AutoSize = true;
        lblExtractorBase.Location = new System.Drawing.Point(15, 25);
        lblExtractorBase.Name = "lblExtractorBase";
        lblExtractorBase.Size = new System.Drawing.Size(56, 15);
        lblExtractorBase.Text = "Base URI:";
        // txtExtractorBase
        txtExtractorBase.Location = new System.Drawing.Point(90, 22);
        txtExtractorBase.Name = "txtExtractorBase";
        txtExtractorBase.Size = new System.Drawing.Size(350, 23);
        txtExtractorBase.TabIndex = 1;
        txtExtractorBase.Text = "https://example.com/pages/home";

        // lblExtractorHtml
        lblExtractorHtml.AutoSize = true;
        lblExtractorHtml.Location = new System.Drawing.Point(15, 55);
        lblExtractorHtml.Name = "lblExtractorHtml";
        lblExtractorHtml.Size = new System.Drawing.Size(68, 15);
        lblExtractorHtml.Text = "Input HTML:";
        // txtExtractorHtml
        txtExtractorHtml.Font = new System.Drawing.Font("Consolas", 9F);
        txtExtractorHtml.Location = new System.Drawing.Point(90, 52);
        txtExtractorHtml.Multiline = true;
        txtExtractorHtml.Name = "txtExtractorHtml";
        txtExtractorHtml.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        txtExtractorHtml.Size = new System.Drawing.Size(350, 135);
        txtExtractorHtml.TabIndex = 2;
        txtExtractorHtml.Text = "<html>\r\n<head>\r\n  <link rel=\"stylesheet\" href=\"/css/app.css\" />\r\n  <script src=\"/js/bundle.js\"></script>\r\n  <style>\r\n    .hero { background: url('/images/hero.png'); }\r\n  </style>\r\n</head>\r\n<body>\r\n  <img src=\"/images/avatar.jpg\" />\r\n  <div style=\"background-image: url('/bg.gif');\"></div>\r\n</body>\r\n</html>";

        // btnRunExtractorTest
        btnRunExtractorTest.Location = new System.Drawing.Point(90, 195);
        btnRunExtractorTest.Name = "btnRunExtractorTest";
        btnRunExtractorTest.Size = new System.Drawing.Size(180, 26);
        btnRunExtractorTest.TabIndex = 3;
        btnRunExtractorTest.Text = "Extract Resource URLs";
        btnRunExtractorTest.UseVisualStyleBackColor = true;
        btnRunExtractorTest.Click += btnRunExtractorTest_Click;

        // txtExtractorOutput
        txtExtractorOutput.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
        txtExtractorOutput.Font = new System.Drawing.Font("Consolas", 9F);
        txtExtractorOutput.Location = new System.Drawing.Point(460, 52);
        txtExtractorOutput.Multiline = true;
        txtExtractorOutput.Name = "txtExtractorOutput";
        txtExtractorOutput.ReadOnly = true;
        txtExtractorOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        txtExtractorOutput.Size = new System.Drawing.Size(435, 169);
        txtExtractorOutput.TabIndex = 4;

        // 
        // Form1
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(960, 720);
        Controls.Add(tabControl1);
        MinimumSize = new System.Drawing.Size(976, 759);
        Name = "Form1";
        StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        Text = "Sekmen.StaticSiteGenerator - WinForms Test & Demo Application";

        tabControl1.ResumeLayout(false);
        tabExporter.ResumeLayout(false);
        tabExporter.PerformLayout();
        tabTester.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvReplacements).EndInit();
        grpUrlValidation.ResumeLayout(false);
        grpUrlValidation.PerformLayout();
        grpHtmlRewriter.ResumeLayout(false);
        grpHtmlRewriter.PerformLayout();
        grpExtractor.ResumeLayout(false);
        grpExtractor.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage tabExporter;
    private System.Windows.Forms.TabPage tabTester;

    // Exporter Controls
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.TextBox txtSiteUrl;
    private System.Windows.Forms.TextBox txtTargetUrl;
    private System.Windows.Forms.TextBox txtOutputFolder;
    private System.Windows.Forms.TextBox txtAdditionalUrls;
    private System.Windows.Forms.Button btnBrowseFolder;
    private System.Windows.Forms.DataGridView dgvReplacements;
    private System.Windows.Forms.DataGridViewTextBoxColumn colOldValue;
    private System.Windows.Forms.DataGridViewTextBoxColumn colNewValue;
    private System.Windows.Forms.Button btnAddReplacement;
    private System.Windows.Forms.Button btnRemoveReplacement;
    private System.Windows.Forms.Button btnExport;
    private System.Windows.Forms.Button btnOpenFolder;
    private System.Windows.Forms.Button btnClearLog;
    private System.Windows.Forms.ProgressBar progressBar1;
    private System.Windows.Forms.TextBox txtLog;

    // Tester Controls
    private System.Windows.Forms.GroupBox grpUrlValidation;
    private System.Windows.Forms.Label lblInputUrl;
    private System.Windows.Forms.TextBox txtValidationInputUrl;
    private System.Windows.Forms.Label lblBaseUri;
    private System.Windows.Forms.TextBox txtValidationBaseUri;
    private System.Windows.Forms.Button btnRunValidationTest;
    private System.Windows.Forms.TextBox txtValidationOutput;

    private System.Windows.Forms.GroupBox grpHtmlRewriter;
    private System.Windows.Forms.Label lblRewriterSource;
    private System.Windows.Forms.TextBox txtRewriterSource;
    private System.Windows.Forms.Label lblRewriterTarget;
    private System.Windows.Forms.TextBox txtRewriterTarget;
    private System.Windows.Forms.Label lblHtmlInput;
    private System.Windows.Forms.TextBox txtHtmlInput;
    private System.Windows.Forms.Button btnRunRewriterTest;
    private System.Windows.Forms.TextBox txtHtmlOutput;

    private System.Windows.Forms.GroupBox grpExtractor;
    private System.Windows.Forms.Label lblExtractorBase;
    private System.Windows.Forms.TextBox txtExtractorBase;
    private System.Windows.Forms.Label lblExtractorHtml;
    private System.Windows.Forms.TextBox txtExtractorHtml;
    private System.Windows.Forms.Button btnRunExtractorTest;
    private System.Windows.Forms.TextBox txtExtractorOutput;
}