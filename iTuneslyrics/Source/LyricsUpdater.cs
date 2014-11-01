using System;
using System.Collections.Generic;
using System.Windows.Forms;
using iTunesLib;
using iTuneslyrics.Properties;
using iTuneslyrics.org.lyricwiki;

namespace iTuneslyrics.Source
{
    class LyricsUpdater
    {
        private readonly List<IITFileOrCDTrack> mSelectedTracks;
        private readonly LyricWiki mLyricsWiki;
        private readonly frmResult mForm;
        private readonly Boolean mOverwrite;

        public LyricsUpdater(List<IITFileOrCDTrack> selectedTracks, LyricWiki lyricsWiki, Boolean overwrite, frmResult form)
        {
            this.mOverwrite = false;
            this.mSelectedTracks = selectedTracks;
            this.mLyricsWiki = lyricsWiki;
            this.mOverwrite = overwrite;
            this.mForm = form;
        }

        public void UpdateLyrics()
        {
            foreach (var currentTrack in this.mSelectedTracks)
            {
                var artist = currentTrack.Artist;
                var song = currentTrack.Name;

                if (string.IsNullOrEmpty(currentTrack.Location) || string.IsNullOrEmpty(artist) ||
                    string.IsNullOrEmpty(song)) continue;

                String[] row = { song, artist, "Processing..." };
                var index = (int)this.mForm.Invoke(this.mForm.m_DelegateAddRow, new Object[] { row });

                if (currentTrack.Lyrics != null && !this.mOverwrite)
                {
                    this.mForm.Invoke(this.mForm.m_DelegateUpdateRow, new Object[] { index, ResultCodes.Skipped });
                    continue;
                }

                try
                {
                    var result = new LyricsResult { lyrics = string.Empty};
                    if (this.mLyricsWiki.checkSongExists(artist, song))
                    {
                        result = this.mLyricsWiki.getSong(artist, song);
                        if (this.mOverwrite || currentTrack.Lyrics == null)
                        {
                            SetLyrics(currentTrack, result, index);
                        }
                    }
                    else
                    {
                        result.url = string.Format("http://lyrics.wikia.com/{0}:{1}", artist, song.Replace(" ", "_"));
                        SetLyrics(currentTrack, result, index);
                    }
                }
                catch (Exception e)
                {
                    //throw;
                    MessageBox.Show(e.Message);
                    this.mForm.Invoke(this.mForm.m_DelegateUpdateRow, new Object[] { index, ResultCodes.NotFound });
                }
            }
            MessageBox.Show(Resources.LyricsUpdater_UpdateLyrics_Completed, Resources.LyricsUpdater_UpdateLyrics_Completed, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Sets the lyrics taking into account varios factors
        /// </summary>
        /// <param name="currentTrack"></param>
        /// <param name="result"></param>
        /// <param name="index"></param>
        private void SetLyrics(IITFileOrCDTrack currentTrack, LyricsResult result, int index)
        {
            string lyrics;
            var isFound = ResultCodes.Found;
            if (result.lyrics.Equals("instrumental", StringComparison.OrdinalIgnoreCase))
            {
                lyrics = result.lyrics;
            }
            else
            {
                lyrics = LyricsDecoder.DecodeLyrics(result.url);
                if (string.IsNullOrEmpty(lyrics))
                    isFound = ResultCodes.NotFound;
            }
            if (isFound == ResultCodes.Found)
                currentTrack.Lyrics = lyrics;
            this.mForm.Invoke(this.mForm.m_DelegateUpdateRow, new Object[] { index, isFound });
        }

    }


}
