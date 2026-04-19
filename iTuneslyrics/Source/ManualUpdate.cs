using System;
using System.Windows.Forms;
using System.Reflection;
using iTunesLib;
using iTuneslyrics.Properties;
using Genius.Core;
using Genius;
using System.Linq;

namespace iTuneslyrics.Source
{
    partial class ManualUpdate : Form
    {
        
        public GeniusClient geniusClient = null;
        public IITFileOrCDTrack currentTrack = null;
        public ManualUpdate()
        {
            InitializeComponent();
        }

        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                // Get all Title attributes on this assembly
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                // If there is at least one Title attribute
                if (attributes.Length > 0)
                {
                    // Select the first one
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    // If it is not an empty string, return it
                    if (titleAttribute.Title != "")
                        return titleAttribute.Title;
                }
                // If there was no Title attribute, or if the Title attribute was the empty string, return the .exe name
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                // Get all Description attributes on this assembly
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                // If there aren't any Description attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Description attribute, return its value
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                // Get all Product attributes on this assembly
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                // If there aren't any Product attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Product attribute, return its value
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                // Get all Copyright attributes on this assembly
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                // If there aren't any Copyright attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Copyright attribute, return its value
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                // Get all Company attributes on this assembly
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                // If there aren't any Company attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Company attribute, return its value
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion

        private async void AboutBox1_Load(object sender, EventArgs e)
        {
            try
            {
                var tempPath = "";
                var artist = currentTrack.Artist;
                var song = currentTrack.Name;

                lblArtist.Text = artist;
                lblSong.Text = song;
                if (currentTrack.Artwork[1] != null)
                {
                    tempPath = System.IO.Path.GetTempPath() + "\\itunesart";
                    currentTrack.Artwork[1].SaveArtworkToFile(tempPath);
                }
                artPictureBox.ImageLocation = tempPath;

                var searchArtist = TitleNormalizer.Normalize(artist);
                var searchSong = TitleNormalizer.Normalize(song);
                var query = await geniusClient.SearchClient.Search(searchArtist + " " + searchSong);
                var match = query.Response.Hits.FirstOrDefault(s =>
                    TitleNormalizer.Matches(s.Result.PrimaryArtist.Name, artist) &&
                    TitleNormalizer.Matches(s.Result.Title, song));

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
            catch (Exception)
            {

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
