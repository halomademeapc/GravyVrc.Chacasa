using Microsoft.UI.Xaml;
using System;
using GravyVrc.Chacasa.Windows.Data;
using GravyVrc.Chacasa.Windows.Hass;
using GravyVrc.Chacasa.Windows.Templates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GravyVrc.Chacasa.Windows
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private static IHost? AppHost;
        public static IServiceProvider Services => AppHost!.Services;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            AppHost ??= BuildHost();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();
        }

        private Window m_window;

        private IHost BuildHost()
        {
            var builder = Host.CreateApplicationBuilder();

            builder.Services.AddMemoryCache();
            builder.Services.AddSingleton<IHassProvider, HassProvider>();
            builder.Services.AddDbContext<SettingsContext>(opts => opts.UseSqlite("Data Source=chacasa.db"));
            builder.Services.AddTransient<SettingsService>();
            builder.Services.AddSingleton<HomeTemplateService>();
            builder.Services.AddSingleton<PageService>();

            AppHost = builder.Build();
            AppHost.StartAsync();
            return AppHost;
        }
    }
}