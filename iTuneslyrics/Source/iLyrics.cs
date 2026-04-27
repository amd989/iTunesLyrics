using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using iTunesLib;
using iTuneslyrics.Properties;

namespace iTuneslyrics.Source
{
    public partial class iLyrics : Form
    {
        readonly IiTunes _iTunesApp;
        IGeniusService _geniusService;
        public iLyrics()
        {
            InitializeComponent();
            try
            {
                _iTunesApp = new iTunesApp();
                _iTunesApp.BrowserWindow.Visible = true;
                _iTunesApp.BrowserWindow.Minimized = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not connect to iTunes. Make sure iTunes is installed and running.\r\n\r\n" + ex.Message,
                    "iTunes Unavailable",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Application.Exit();
                return;
            }
            alwaysOnTopItem.Checked = UserSettings.AlwaysOnTop;
        }

        private void btnAlbums_Click(object sender, EventArgs e)
        {
            var token = UserSettings.GeniusApiToken;
            if (string.IsNullOrWhiteSpace(token))
            {
                token = PromptForApiToken();
                if (string.IsNullOrWhiteSpace(token))
                {
                    MessageBox.Show(this, "A Genius API token is required. Set one via Settings \u2192 Genius API Token\u2026", "API Token Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            var selectedTracks = new List<IITFileOrCDTrack>();
            if(chkFix.Checked)
            {
                var tracks = _iTunesApp.LibraryPlaylist.Tracks;
                selectedTracks = tracks.OfType<IITFileOrCDTrack>().Where(track => track.Lyrics != null && track.Lyrics.Contains("\uFFFD")).ToList();
            }
            else
            {
                selectedTracks = (_iTunesApp.SelectedTracks?.OfType<IITFileOrCDTrack>() ?? Enumerable.Empty<IITFileOrCDTrack>()).ToList();
            }

            if (selectedTracks.Count == 0)
            {
                MessageBox.Show(this, Resources.iLyrics_btnAlbums_Click_Nothing_seems_to_be_selected);
                return;
            }

            _geniusService = new GeniusService(token);

            if (chkAuto.Checked == true)
            {
                var fr = new frmResult(selectedTracks, _geniusService, chkOverwrite.Checked);
                fr.TopMost = this.TopMost;
                fr.ShowDialog(this);
            }
            else
            {
                var updatedSongsCount = 0;
                foreach (var currentTrack in selectedTracks)
                {
                    updatedSongsCount++;
                    var ab = new ManualUpdate(_geniusService, currentTrack);
                    ab.TopMost = this.TopMost;
                    var dr = ab.ShowDialog(this);
                    if (dr == DialogResult.Abort)
                        break;
                }
                MessageBox.Show(this, updatedSongsCount == 0 ? "All selected songs seems to have lyrics" : "Update completed", "Complete");
            }
        }

        private void apiTokenItem_Click(object sender, EventArgs e)
        {
            PromptForApiToken();
        }

        private void alwaysOnTopItem_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = alwaysOnTopItem.Checked;
            UserSettings.AlwaysOnTop = alwaysOnTopItem.Checked;
        }

        private string PromptForApiToken()
        {
            using (var dialog = new Form())
            {
                dialog.Text = "Genius API Token";
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.MinimizeBox = false;
                dialog.MaximizeBox = false;
                dialog.ClientSize = new Size(420, 130);

                var label = new Label
                {
                    Text = "Paste your Genius API access token.\r\nGet one at https://genius.com/api-clients\r\nStored at: " + UserSettings.ConfigFilePath,
                    Location = new Point(12, 12),
                    Size = new Size(396, 48),
                    AutoSize = false
                };

                var textBox = new TextBox
                {
                    Location = new Point(12, 66),
                    Size = new Size(396, 20),
                    Text = UserSettings.GeniusApiToken,
                    UseSystemPasswordChar = true
                };

                var okButton = new Button
                {
                    Text = "Save",
                    DialogResult = DialogResult.OK,
                    Location = new Point(252, 104),
                    Size = new Size(75, 23)
                };

                var cancelButton = new Button
                {
                    Text = "Cancel",
                    DialogResult = DialogResult.Cancel,
                    Location = new Point(333, 104),
                    Size = new Size(75, 23)
                };

                dialog.ClientSize = new Size(420, 140);
                dialog.Controls.Add(label);
                dialog.Controls.Add(textBox);
                dialog.Controls.Add(okButton);
                dialog.Controls.Add(cancelButton);
                dialog.AcceptButton = okButton;
                dialog.CancelButton = cancelButton;
                dialog.TopMost = this.TopMost;

                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return UserSettings.GeniusApiToken;

                var entered = (textBox.Text ?? string.Empty).Trim();
                try
                {
                    UserSettings.GeniusApiToken = entered;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Could not save token: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return entered;
            }
        }
    }
}
