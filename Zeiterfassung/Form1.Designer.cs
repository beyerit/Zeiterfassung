namespace Zeiterfassung
{
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnOpen = new System.Windows.Forms.Button();
            this.llblPath = new System.Windows.Forms.LinkLabel();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.lblSync = new System.Windows.Forms.Label();
            this.btnReportError = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.btnAbout = new System.Windows.Forms.Button();
            this.btnSync = new System.Windows.Forms.Button();
            this.lblCopyright = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnOpen
            // 
            this.btnOpen.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpen.Image = global::Zeiterfassung.Properties.Resources.Icon241;
            this.btnOpen.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOpen.Location = new System.Drawing.Point(31, 25);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(224, 67);
            this.btnOpen.TabIndex = 0;
            this.btnOpen.Text = "Ö&ffnen";
            this.toolTip.SetToolTip(this.btnOpen, "Zeiterfassungstabelle öffnen");
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // llblPath
            // 
            this.llblPath.AutoSize = true;
            this.llblPath.Location = new System.Drawing.Point(31, 108);
            this.llblPath.Name = "llblPath";
            this.llblPath.Size = new System.Drawing.Size(40, 13);
            this.llblPath.TabIndex = 1;
            this.llblPath.TabStop = true;
            this.llblPath.Text = "C:\\test";
            this.toolTip.SetToolTip(this.llblPath, "Dateipfad der Excel Tabelle ändern");
            this.llblPath.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llblPath_LinkClicked);
            this.llblPath.TextChanged += new System.EventHandler(this.llblPath_TextChanged);
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // lblSync
            // 
            this.lblSync.AutoSize = true;
            this.lblSync.Location = new System.Drawing.Point(30, 143);
            this.lblSync.Name = "lblSync";
            this.lblSync.Size = new System.Drawing.Size(139, 13);
            this.lblSync.TabIndex = 3;
            this.lblSync.Text = "Noch nicht Synchronisiert";
            this.toolTip.SetToolTip(this.lblSync, "Zeitstempel der letzten erfolgreichen Synchronisierung");
            // 
            // btnReportError
            // 
            this.btnReportError.Image = global::Zeiterfassung.Properties.Resources._105_20;
            this.btnReportError.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReportError.Location = new System.Drawing.Point(343, 175);
            this.btnReportError.Name = "btnReportError";
            this.btnReportError.Size = new System.Drawing.Size(143, 33);
            this.btnReportError.TabIndex = 4;
            this.btnReportError.Text = "Einen &Fehler melden";
            this.btnReportError.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.btnReportError, "Einen Fehler an die lokale IT melden\r\n[STRG] + klicken zum anzeigen der Log-Datei" +
        "\r\n");
            this.btnReportError.UseVisualStyleBackColor = true;
            this.btnReportError.Click += new System.EventHandler(this.btnReportError_Click);
            // 
            // toolTip
            // 
            this.toolTip.IsBalloon = true;
            // 
            // btnAbout
            // 
            this.btnAbout.Image = global::Zeiterfassung.Properties.Resources._1001_20;
            this.btnAbout.Location = new System.Drawing.Point(492, 175);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(41, 33);
            this.btnAbout.TabIndex = 6;
            this.btnAbout.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.btnAbout, "Informationen über dieses Programm");
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // btnSync
            // 
            this.btnSync.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSync.Image = global::Zeiterfassung.Properties.Resources._1401_36;
            this.btnSync.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSync.Location = new System.Drawing.Point(348, 25);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(161, 67);
            this.btnSync.TabIndex = 7;
            this.btnSync.Text = "&Sync";
            this.toolTip.SetToolTip(this.btnSync, "Manuelles Synchronisieren");
            this.btnSync.UseVisualStyleBackColor = true;
            this.btnSync.Click += new System.EventHandler(this.btnSync_Click);
            // 
            // lblCopyright
            // 
            this.lblCopyright.AutoSize = true;
            this.lblCopyright.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCopyright.ForeColor = System.Drawing.Color.Gray;
            this.lblCopyright.Location = new System.Drawing.Point(12, 196);
            this.lblCopyright.Name = "lblCopyright";
            this.lblCopyright.Size = new System.Drawing.Size(271, 13);
            this.lblCopyright.TabIndex = 5;
            this.lblCopyright.Text = "Copyright © 2022 - 2023 Christoph Beyer, Schindler AG";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(545, 220);
            this.Controls.Add(this.btnSync);
            this.Controls.Add(this.btnAbout);
            this.Controls.Add(this.lblCopyright);
            this.Controls.Add(this.btnReportError);
            this.Controls.Add(this.lblSync);
            this.Controls.Add(this.llblPath);
            this.Controls.Add(this.btnOpen);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Zeiterfassung - Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button btnOpen;
        private LinkLabel llblPath;
        private System.Windows.Forms.Timer timer;
        private OpenFileDialog openFileDialog;
        private Label lblSync;
        private Button btnReportError;
        private ToolTip toolTip;
        private Label lblCopyright;
        private Button btnAbout;
        private Button btnSync;
    }
}