using System;
using System.IO;

namespace iTuneslyrics.Source.Services
{
    public class UserSettings : IUserSettings
    {
        private static string SettingsDir => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "iTunesLyrics");

        public string ConfigFilePath => Path.Combine(SettingsDir, "config.txt");

        private string AlwaysOnTopPath => Path.Combine(SettingsDir, "alwaysOnTop.txt");

        public string GeniusApiToken
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
                Directory.CreateDirectory(SettingsDir);
                File.WriteAllText(ConfigFilePath, value ?? string.Empty);
            }
        }

        public bool AlwaysOnTop
        {
            get
            {
                try
                {
                    return File.Exists(AlwaysOnTopPath)
                        && bool.TryParse(File.ReadAllText(AlwaysOnTopPath).Trim(), out var v)
                        && v;
                }
                catch
                {
                    return false;
                }
            }
            set
            {
                Directory.CreateDirectory(SettingsDir);
                File.WriteAllText(AlwaysOnTopPath, value ? "true" : "false");
            }
        }
    }
}
