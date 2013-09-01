namespace iTuneslyrics.Source
{
    partial class frmResult
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.clmTrack = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmArtist = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmTrack,
            this.clmArtist,
            this.clmResult});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridView1.Size = new System.Drawing.Size(563, 365);
            this.dataGridView1.TabIndex = 0;
            // 
            // clmTrack
            // 
            this.clmTrack.HeaderText = "Track";
            this.clmTrack.Name = "clmTrack";
            this.clmTrack.ReadOnly = true;
            this.clmTrack.Width = 300;
            // 
            // clmArtist
            // 
            this.clmArtist.HeaderText = "Artist";
            this.clmArtist.Name = "clmArtist";
            this.clmArtist.ReadOnly = true;
            // 
            // clmResult
            // 
            this.clmResult.HeaderText = "Result";
            this.clmResult.Name = "clmResult";
            this.clmResult.ReadOnly = true;
            // 
            // frmResult
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(563, 365);
            this.Controls.Add(this.dataGridView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmResult";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Results";
            this.Load += new System.EventHandler(this.frmResult_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmTrack;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmArtist;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmResult;
    }
}