using System;
using System.Collections.Generic;
using System.Threading;
using GravyVrc.Chacasa.Windows.Data;
using GravyVrc.Chacasa.Windows.Templates;
using GravyVrc.Chacasa.Windows.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GravyVrc.Chacasa.Windows
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly PageService _pageService;
        private CancellationTokenSource? _cancellationTokenSource;
        private const int MaxScrollback = 50;
        private bool IsRunning = false;

        public MainWindow()
        {
            InitializeComponent();
            var db = App.Services.GetRequiredService<SettingsContext>();
            db.Database.EnsureCreated();
            _pageService = App.Services.GetRequiredService<PageService>();

            ChatService.MessageSent += OnMessageSend;
        }

        private void OnMessageSend(string message)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                var model = Log.DataContext as HomeViewModel;
                model!.Messages.Add(message);
                if (model.Messages.Count > MaxScrollback)
                    model.Messages.RemoveAt(0);
            });
        }

        private void ToggleButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!IsRunning)
                StartButton_OnClick(sender, e);
            else
                StopButton_OnClick(sender, e);
            IsRunning = !IsRunning;
        }

        private async void StartButton_OnClick(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var testPages = new List<DisplayPage>
            {
                new()
                {
                    Id = 1,
                    RefreshPeriod = null,
                    Duration = TimeSpan.FromSeconds(10),
                    Label = "CO2",
                    Order = 1,
                    Template = @"Testing something :) CO2 level: {{states('sensor.senseair_co2_value')}}ppm"
                },
                new()
                {
                    Id = 0,
                    RefreshPeriod = TimeSpan.FromSeconds(2),
                    Duration = TimeSpan.FromSeconds(10),
                    Label = "Power",
                    Order = 0,
                    Template = @"Hello from home assistant! Grid consumption: {{states('sensor.total_power')}}W"
                },
                new()
                {
                    Id = 3,
                    Duration = TimeSpan.FromSeconds(10),
                    Label = "Power",
                    Order = 2,
                    Template = @"More testing! Kitchen luminance: {{states('sensor.kitchen_sensor_illuminance')}}lm"
                },
                new()
                {
                    Id = 4,
                    Duration = TimeSpan.FromSeconds(10),
                    Label = "PM25",
                    Order = 4,
                    Template =
                        @"It's working :D Garage PM25: {{states('sensor.particulate_matter_2_5um_concentration_2')}}µg/m³"
                },
                new()
                {
                    Id = 4,
                    Duration = TimeSpan.FromSeconds(10),
                    Label = "Office",
                    Order = 4,
                    Template = @"Office power draw: {{states('sensor.office_power')}}W"
                },
                new()
                {
                    Id = 4,
                    Duration = TimeSpan.FromSeconds(10),
                    Label = "Office",
                    Order = 4,
                    Template =
                        @"🎵 {{state_attr('media_player.spotify_alex_griffith', 'media_title')}} - {{state_attr('media_player.spotify_alex_griffith', 'media_artist')}}"
                }
            };
            await _pageService.ShowPages(testPages, _cancellationTokenSource.Token).ConfigureAwait(false);
        }

        private void StopButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_cancellationTokenSource is null)
                return;
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = null;
        }

        private void ConnectionButton_OnClick(object sender, RoutedEventArgs e)
        {
            OpenConnectionSettings();
        }

        private void OpenConnectionSettings()
        {
            var control = new ConnectionSettingsControl();
            var dialog = new ContentDialog()
            {
                XamlRoot = Content.XamlRoot,
                Content = control
            };
            control.Initialize(dialog);
            dialog.ShowAsync();
        }
    }
}