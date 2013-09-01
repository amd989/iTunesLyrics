using System;
using System.Windows.Forms;
using System.Reflection;
using iTunesLib;
using iTuneslyrics.Properties;

namespace iTuneslyrics.Source
{
    partial class ManualUpdate : Form
    {
        
        public org.lyricwiki.LyricWiki lyricsWiki = null;
        public IITFileOrCDTrack currentTrack = null;
        public ManualUpdate()
        {
            InitializeComponent();

            //  Initialize the AboutBox to display the product information from the assembly information.
            //  Change assembly information settings for your application through either:
            //  - Project->Properties->Application->Assembly Information
            //  - AssemblyInfo.cs
            //this.Text = String.Format("About {0}", AssemblyTitle);
            //this.lblSong.Text = AssemblyProduct;
            //this.lblArtist.Text = String.Format("Version {0}", AssemblyVersion);
            //this.labelCopyright.Text = AssemblyCopyright;
            //this.labelCompanyName.Text = AssemblyCompany;
            //this.textBoxDescription.Text = AssemblyDescription;
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

        private void AboutBox1_Load(object sender, EventArgs e)
        {
            try
            {
                String tempPath = "";
                String artist = currentTrack.Artist;
                String song = currentTrack.Name;

                lblArtist.Text = artist;
                lblSong.Text = song;
                if (currentTrack.Artwork[1] != null)
                {
                    tempPath = System.IO.Path.GetTempPath() + "\\itunesart";
                    currentTrack.Artwork[1].SaveArtworkToFile(tempPath);
                }
                artPictureBox.ImageLocation = tempPath;
                if (lyricsWiki.checkSongExists(artist, song) == true)
                {
                    var result = lyricsWiki.getSong(artist, song);
                    lyricsBox.Text = result.lyrics.Equals("instrumental", StringComparison.OrdinalIgnoreCase) ? result.lyrics : LyricsDecoder.DecodeLyrics(result.url);
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
