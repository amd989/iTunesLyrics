using System;
using System.Net;
using System.Net.Http;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using iTuneslyrics.Source.Services;
using iTuneslyrics.Source.ViewModels;
using iTuneslyrics.Source.Views;

namespace iTuneslyrics.Source
{
    public partial class App : Application
    {
        public IServiceProvider Services { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            // Services
            services.AddSingleton<IUserSettings, UserSettings>();
            services.AddSingleton<IITunesService, ITunesService>();
            services.AddHttpClient<IGeniusService, GeniusService>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.UserAgent.ParseAdd(
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
                    "(KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36");
                client.DefaultRequestHeaders.Accept.ParseAdd(
                    "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.9");
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });
            services.AddSingleton<INavigationService, NavigationService>();

            // ViewModels
            services.AddSingleton<MainViewModel>();
            services.AddTransient<HomeViewModel>();
            services.AddTransient<ResultsViewModel>();
            services.AddTransient<ReviewViewModel>();
            services.AddTransient<SettingsViewModel>();

            // Views
            services.AddSingleton<MainWindow>();
            services.AddTransient<HomePage>();
            services.AddTransient<ResultsPage>();
            services.AddTransient<ReviewPage>();
            services.AddTransient<SettingsPage>();

            // Page provider for WPF-UI navigation
            services.AddSingleton<Wpf.Ui.Abstractions.INavigationViewPageProvider, ViewPageProvider>();

            Services = services.BuildServiceProvider();

            // Apply system theme (follows OS dark/light mode)
            var systemTheme = ApplicationThemeManager.GetSystemTheme();
            var appTheme = systemTheme switch
            {
                Wpf.Ui.Appearance.SystemTheme.Dark => Wpf.Ui.Appearance.ApplicationTheme.Dark,
                Wpf.Ui.Appearance.SystemTheme.HC1 or Wpf.Ui.Appearance.SystemTheme.HC2 or
                Wpf.Ui.Appearance.SystemTheme.HCBlack or Wpf.Ui.Appearance.SystemTheme.HCWhite
                    => Wpf.Ui.Appearance.ApplicationTheme.HighContrast,
                _ => Wpf.Ui.Appearance.ApplicationTheme.Light
            };
            ApplicationThemeManager.Apply(appTheme);

            var mainWindow = Services.GetRequiredService<MainWindow>();
            MainWindow = mainWindow;
            mainWindow.Show();
        }
    }
}
