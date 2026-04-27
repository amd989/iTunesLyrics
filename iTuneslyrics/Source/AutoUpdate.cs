using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
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
        private readonly IGeniusService m_geniusService;
        private readonly bool m_overwrite;

        public frmResult(List<IITFileOrCDTrack> selectedTracks, IGeniusService geniusService, bool overwrite) : this()
        {
            this.m_selectedTracks = selectedTracks;
            this.m_geniusService = geniusService;
            this.m_overwrite = overwrite;
        }

        private frmResult()
        {
            InitializeComponent();
        }

        private async void frmResult_Load(object sender, EventArgs e)
        {
            var lu = new LyricsUpdater(m_selectedTracks, m_geniusService, m_overwrite, this);
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
                    var isDark = this.BackColor.GetBrightness() < 0.5f;
                    this.dataGridView1.Rows[index].DefaultCellStyle.BackColor =
                        isDark ? Color.FromArgb(60, 80, 30) : Color.YellowGreen;
                    break;
            }
            dataGridView1.FirstDisplayedScrollingRowIndex = index;
            dataGridView1.FirstDisplayedCell = dataGridView1.Rows[index].Cells[0];
        }
    }
}
