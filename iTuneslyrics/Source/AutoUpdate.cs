using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Genius.Core;
using iTunesLib;

namespace iTuneslyrics.Source
{
    public enum ResultCodes
    {
        Found,
        NotFound,
        Skipped
    }

    public partial class frmResult : Form
    {
        private readonly List<IITFileOrCDTrack> m_selectedTracks;
        private readonly IGeniusClient m_geniusClient;
        private readonly bool m_overwrite;

        public frmResult(List<IITFileOrCDTrack> selectedTracks, IGeniusClient geniusClient, bool overwrite) : this()
        {
            this.m_selectedTracks = selectedTracks;
            this.m_geniusClient = geniusClient;
            this.m_overwrite = overwrite;
        }

        public frmResult()
        {
            InitializeComponent();
        }

        private async void frmResult_Load(object sender, EventArgs e)
        {
            var lu = new LyricsUpdater(m_selectedTracks, m_geniusClient, m_overwrite, this);
            try
            {
                await lu.UpdateLyricsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public int AddRow(string[] row)
        {
            return this.dataGridView1.Rows.Add(row);
        }

        public void UpdateRow(int index, ResultCodes result)
        {
            switch (result)
            {
                case ResultCodes.Found:
                    this.dataGridView1.Rows[index].Cells[2].Value = "Updated";
                    break;
                case ResultCodes.NotFound:
                    this.dataGridView1.Rows[index].Cells[2].Value = "Not Found";
                    this.dataGridView1.Rows[index].ErrorText = "No matching song found";
                    break;
                case ResultCodes.Skipped:
                    this.dataGridView1.Rows[index].Cells[2].Value = "Skipped";
                    this.dataGridView1.Rows[index].DefaultCellStyle.BackColor = Color.YellowGreen;
                    break;
            }
            dataGridView1.FirstDisplayedScrollingRowIndex = index;
            dataGridView1.FirstDisplayedCell = dataGridView1.Rows[index].Cells[0];
        }
    }
}
