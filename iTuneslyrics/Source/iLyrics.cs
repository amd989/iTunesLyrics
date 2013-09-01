using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using iTunesLib;
using iTuneslyrics.Properties;

namespace iTuneslyrics.Source
{
    public partial class iLyrics : Form
    {
        readonly IiTunes _iTunesApp;
        org.lyricwiki.LyricWiki _lyricsWiki;
        public iLyrics()
        {
            InitializeComponent();
            _iTunesApp = new iTunesAppClass();
            _iTunesApp.BrowserWindow.Visible = true;
            _iTunesApp.BrowserWindow.Minimized = false;
        }

        private void btnAlbums_Click(object sender, EventArgs e)
        {
            var selectedTracks = new List<IITFileOrCDTrack>();
            if(chkFix.Checked)
            {
                var tracks = _iTunesApp.LibraryPlaylist.Tracks;
                selectedTracks = tracks.Cast<IITFileOrCDTrack>().Where(track => track.Lyrics != null && track.Lyrics.Contains("�")).ToList();
            }
            else
            {
                selectedTracks = _iTunesApp.SelectedTracks.Cast<IITFileOrCDTrack>().ToList();    
            }

            if (selectedTracks.Count == 0)
            {
                MessageBox.Show(Resources.iLyrics_btnAlbums_Click_Nothing_seems_to_be_selected);
                return;
            }

            _lyricsWiki = new org.lyricwiki.LyricWiki();

            if (chkAuto.Checked == true)
            {
                var fr = new frmResult(selectedTracks, _lyricsWiki, chkOverwrite.Checked);
                fr.ShowDialog();
            }
            else
            {
                var updatedSongsCount = 0;
                foreach (var currentTrack in selectedTracks)
                {
                    //if (currentTrack.Lyrics != null)
                    //    continue;

                    updatedSongsCount++;
                    var ab = new ManualUpdate {currentTrack = currentTrack, lyricsWiki = _lyricsWiki};
                    var dr = ab.ShowDialog();
                    if (dr == DialogResult.Abort)
                        break;
                }
                MessageBox.Show(updatedSongsCount == 0 ? "All selected songs seems to have lyrics" : "Update completed", "Complete");
            }
        }
    }
}