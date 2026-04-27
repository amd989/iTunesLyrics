using System;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Abstractions;

namespace iTuneslyrics.Source.Services
{
    public class ViewPageProvider : INavigationViewPageProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public ViewPageProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object GetPage(Type pageType)
        {
            return _serviceProvider.GetService(pageType);
        }
    }
}
