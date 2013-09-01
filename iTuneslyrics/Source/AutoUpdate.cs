using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
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

    // delegates used to call MainForm functions from
    //  worker thread
    public delegate int DelegateAddRow(String[] row);
    public delegate void DelegateUpdateRow(int index, ResultCodes result);

    public partial class frmResult : Form
    {
        private List<IITFileOrCDTrack> m_selectedTracks;
        private org.lyricwiki.LyricWiki m_lyricsWiki;
        private Boolean m_overwrite = false;

        // Delegate instances used to call user interface
        // functions from worker thread:
        public DelegateAddRow m_DelegateAddRow;
        public DelegateUpdateRow m_DelegateUpdateRow;

        public frmResult(List<IITFileOrCDTrack> selectedTracks, org.lyricwiki.LyricWiki lyricsWiki, Boolean overwrite) : this()
        {
            this.m_selectedTracks = selectedTracks;
            this.m_lyricsWiki = lyricsWiki;
            this.m_overwrite = overwrite;
        }

        public frmResult()
        {
            InitializeComponent();

            // initialize delegates
            m_DelegateAddRow = AddRow;
            m_DelegateUpdateRow = UpdateRow;
        }

        private void frmResult_Load(object sender, EventArgs e)
        {
            var lu = new LyricsUpdater(m_selectedTracks, m_lyricsWiki, m_overwrite, this);
            var threadDelegate = new ThreadStart(lu.UpdateLyrics);
            var newThread = new Thread(threadDelegate);
            newThread.Start();
        }

        private int AddRow(String[] row)
        {
            int index = this.dataGridView1.Rows.Add(row);
            return index;
        }

        private void UpdateRow(int index, ResultCodes result)
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