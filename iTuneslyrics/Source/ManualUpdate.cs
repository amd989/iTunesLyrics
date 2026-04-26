using System;
using System.Windows.Forms;
using System.Reflection;
using iTunesLib;
using iTuneslyrics.Properties;
using Genius;
using Genius.Core;

namespace iTuneslyrics.Source
{
    partial class ManualUpdate : Form
    {
        private readonly GeniusClient geniusClient;
        private readonly IITFileOrCDTrack currentTrack;

        public ManualUpdate(IGeniusClient geniusClient, IITFileOrCDTrack currentTrack)
        {
            this.geniusClient = (GeniusClient)(geniusClient ?? throw new ArgumentNullException(nameof(geniusClient)));
            this.currentTrack = currentTrack ?? throw new ArgumentNullException(nameof(currentTrack));
            InitializeComponent();
        }

        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                        return titleAttribute.Title;
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);
            }
        }

        public string AssemblyVersion
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0) return "";
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0) return "";
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0) return "";
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0) return "";
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion

        private async void AboutBox1_Load(object sender, EventArgs e)
        {
            try
            {
                var artist = currentTrack.Artist;
                var song = currentTrack.Name;

                lblArtist.Text = artist;
                lblSong.Text = song;

                var tempPath = string.Empty;
                if (currentTrack.Artwork != null && currentTrack.Artwork.Count > 0)
                {
                    tempPath = System.IO.Path.GetTempPath() + "\\itunesart";
                    currentTrack.Artwork[1].SaveArtworkToFile(tempPath);
                }
                artPictureBox.ImageLocation = tempPath;

                var searchArtist = TitleNormalizer.NormalizeForQuery(artist);
                var searchSong = TitleNormalizer.NormalizeForQuery(song);
                var query = await geniusClient.SearchClient.Search(searchArtist + " " + searchSong);
                var hits = query?.Response?.Hits;
                var match = TitleNormalizer.PickBest(hits, artist, song);

                if (match != null)
                {
                    lyricsBox.Text = await LyricsDecoder.DecodeLyricsAsync(match.Result.Url);
                }
                else
                {
                    lyricsBox.Text = Resources.ManualUpdate_AboutBox1_Load_No_Result;
                    btnUpdate.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                lyricsBox.Text = string.Empty;
                btnUpdate.Enabled = false;
                MessageBox.Show(this, "Could not load lyrics: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            currentTrack.Lyrics = lyricsBox.Text;
            this.Close();
            this.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Abort;
            this.Close();
            this.Dispose();
        }
    }
}
