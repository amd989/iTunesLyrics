using System;
using System.IO;

namespace iTuneslyrics.Source
{
    internal static class UserSettings
    {
        private static string SettingsDir => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "iTunesLyrics");

        public static string ConfigFilePath => Path.Combine(SettingsDir, "config.txt");

        private static string AlwaysOnTopPath => Path.Combine(SettingsDir, "alwaysOnTop.txt");

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
                Directory.CreateDirectory(SettingsDir);
                File.WriteAllText(ConfigFilePath, value ?? string.Empty);
            }
        }

        public static bool AlwaysOnTop
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
