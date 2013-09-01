namespace iTuneslyrics.Source
{
    partial class iLyrics
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(iLyrics));
            this.btnAlbums = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkFix = new System.Windows.Forms.CheckBox();
            this.chkOverwrite = new System.Windows.Forms.CheckBox();
            this.chkAuto = new System.Windows.Forms.CheckBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAlbums
            // 
            this.btnAlbums.Location = new System.Drawing.Point(301, 8);
            this.btnAlbums.Name = "btnAlbums";
            this.btnAlbums.Size = new System.Drawing.Size(75, 23);
            this.btnAlbums.TabIndex = 1;
            this.btnAlbums.Text = "Get Lyrics";
            this.btnAlbums.UseVisualStyleBackColor = true;
            this.btnAlbums.Click += new System.EventHandler(this.btnAlbums_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.chkFix);
            this.panel1.Controls.Add(this.chkOverwrite);
            this.panel1.Controls.Add(this.chkAuto);
            this.panel1.Controls.Add(this.btnAlbums);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(388, 45);
            this.panel1.TabIndex = 3;
            // 
            // chkFix
            // 
            this.chkFix.AutoSize = true;
            this.chkFix.Location = new System.Drawing.Point(222, 12);
            this.chkFix.Name = "chkFix";
            this.chkFix.Size = new System.Drawing.Size(69, 17);
            this.chkFix.TabIndex = 4;
            this.chkFix.Text = "Fix Lyrics";
            this.toolTip.SetToolTip(this.chkFix, "Fix Songs with unknown characters");
            this.chkFix.UseVisualStyleBackColor = true;
            this.chkFix.Visible = false;
            // 
            // chkOverwrite
            // 
            this.chkOverwrite.AutoSize = true;
            this.chkOverwrite.Location = new System.Drawing.Point(144, 12);
            this.chkOverwrite.Name = "chkOverwrite";
            this.chkOverwrite.Size = new System.Drawing.Size(71, 17);
            this.chkOverwrite.TabIndex = 3;
            this.chkOverwrite.Text = "Overwrite";
            this.toolTip.SetToolTip(this.chkOverwrite, "Overwrite the lyrics if there is already lyrics set");
            this.chkOverwrite.UseVisualStyleBackColor = true;
            // 
            // chkAuto
            // 
            this.chkAuto.AutoSize = true;
            this.chkAuto.Location = new System.Drawing.Point(12, 12);
            this.chkAuto.Name = "chkAuto";
            this.chkAuto.Size = new System.Drawing.Size(126, 17);
            this.chkAuto.TabIndex = 2;
            this.chkAuto.Text = "Update Automatically";
            this.toolTip.SetToolTip(this.chkAuto, "Update lyrics without asking for confirmation");
            this.chkAuto.UseVisualStyleBackColor = true;
            // 
            // iLyrics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(388, 45);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "iLyrics";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "iTunes Lyrics Importer";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnAlbums;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox chkAuto;
        private System.Windows.Forms.CheckBox chkOverwrite;
        private System.Windows.Forms.CheckBox chkFix;
        private System.Windows.Forms.ToolTip toolTip;

    }
}

