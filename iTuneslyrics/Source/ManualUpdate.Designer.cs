namespace iTuneslyrics.Source
{
    partial class ManualUpdate
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManualUpdate));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.lblArtist = new System.Windows.Forms.Label();
            this.lblSong = new System.Windows.Forms.Label();
            this.artPictureBox = new System.Windows.Forms.PictureBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.button1 = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.lyricsBox = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.artPictureBox)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 67F));
            this.tableLayoutPanel.Controls.Add(this.lblArtist, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.lblSong, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.flowLayoutPanel1, 1, 3);
            this.tableLayoutPanel.Controls.Add(this.lyricsBox, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.panel1, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.artPictureBox, 0, 2);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(9, 9);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 4;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 65F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(674, 344);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // lblArtist
            // 
            this.lblArtist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblArtist.Font = new System.Drawing.Font("Georgia", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblArtist.Location = new System.Drawing.Point(6, 34);
            this.lblArtist.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.lblArtist.MaximumSize = new System.Drawing.Size(0, 17);
            this.lblArtist.Name = "lblArtist";
            this.lblArtist.Size = new System.Drawing.Size(213, 17);
            this.lblArtist.TabIndex = 0;
            this.lblArtist.Text = "Artist";
            this.lblArtist.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSong
            // 
            this.lblSong.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSong.Font = new System.Drawing.Font("Georgia", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSong.Location = new System.Drawing.Point(6, 0);
            this.lblSong.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.lblSong.MaximumSize = new System.Drawing.Size(0, 17);
            this.lblSong.Name = "lblSong";
            this.lblSong.Size = new System.Drawing.Size(213, 17);
            this.lblSong.TabIndex = 19;
            this.lblSong.Text = "Song";
            this.lblSong.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // artPictureBox
            // 
            this.artPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.artPictureBox.Location = new System.Drawing.Point(3, 71);
            this.artPictureBox.Name = "artPictureBox";
            this.artPictureBox.Size = new System.Drawing.Size(216, 217);
            this.artPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.artPictureBox.TabIndex = 12;
            this.artPictureBox.TabStop = false;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.button1);
            this.flowLayoutPanel1.Controls.Add(this.btnCancel);
            this.flowLayoutPanel1.Controls.Add(this.btnUpdate);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(225, 294);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(446, 47);
            this.flowLayoutPanel1.TabIndex = 24;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(350, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(93, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Cancel All";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(269, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(188, 3);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnUpdate.TabIndex = 1;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // lyricsBox
            // 
            this.lyricsBox.BackColor = System.Drawing.SystemColors.Info;
            this.lyricsBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lyricsBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lyricsBox.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lyricsBox.Location = new System.Drawing.Point(225, 3);
            this.lyricsBox.Name = "lyricsBox";
            this.tableLayoutPanel.SetRowSpan(this.lyricsBox, 3);
            this.lyricsBox.Size = new System.Drawing.Size(446, 285);
            this.lyricsBox.TabIndex = 25;
            this.lyricsBox.Text = "";
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 294);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(216, 47);
            this.panel1.TabIndex = 26;
            // 
            // ManualUpdate
            // 
            this.AcceptButton = this.btnUpdate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(692, 362);
            this.Controls.Add(this.tableLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ManualUpdate";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Update Lyrics";
            this.Load += new System.EventHandler(this.AboutBox1_Load);
            this.tableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.artPictureBox)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.PictureBox artPictureBox;
        private System.Windows.Forms.Label lblSong;
        private System.Windows.Forms.Label lblArtist;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.RichTextBox lyricsBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1;
    }
}
