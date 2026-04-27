namespace iTuneslyrics.Source.Services
{
    public interface IUserSettings
    {
        string ConfigFilePath { get; }
        string GeniusApiToken { get; set; }
        bool AlwaysOnTop { get; set; }
    }
}
