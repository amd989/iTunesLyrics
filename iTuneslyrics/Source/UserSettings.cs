using System;
using System.IO;

namespace iTuneslyrics.Source
{
    internal static class UserSettings
    {
        public static string ConfigFilePath
        {
            get
            {
                var dir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "iTunesLyrics");
                return Path.Combine(dir, "config.txt");
            }
        }

        public static string GeniusApiToken
        {
            get
            {
                try
                {
                    return File.Exists(ConfigFilePath)
                        ? File.ReadAllText(ConfigFilePath).Trim()
                        : string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }
            set
            {
                var path = ConfigFilePath;
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, value ?? string.Empty);
            }
        }
    }
}
