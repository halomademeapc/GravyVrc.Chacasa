using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using GravyVrc.Chacasa.Windows.Data;
using GravyVrc.Chacasa.Windows.Hass;
using GravyVrc.Chacasa.Windows.Templates;
using GravyVrc.Chacasa.Windows.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GravyVrc.Chacasa.Windows
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly HomeStateService _stateService;
        private readonly PageService _pageService;
        private CancellationTokenSource? _cancellationTokenSource;

        public MainWindow()
        {
            InitializeComponent();
            var db = App.Services.GetRequiredService<SettingsContext>();
            db.Database.EnsureCreated();
            _stateService = App.Services.GetRequiredService<HomeStateService>();
            _pageService = App.Services.GetRequiredService<PageService>();

            ChatService.MessageSent += OnMessageSend;
        }

        private void OnMessageSend(string message)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                (Log.DataContext as HomeViewModel)!.Messages.Add(message);
            });
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
                    Duration = TimeSpan.FromSeconds(16),
                    Label = "CO2",
                    Order = 1,
                    Template = @"Testing something :) CO2 level: {{states('sensor.senseair_co2_value')}}ppm"
                },
                new()
                {
                    Id = 0,
                    RefreshPeriod = TimeSpan.FromSeconds(2),
                    Duration = TimeSpan.FromSeconds(16),
                    Label = "Power",
                    Order = 0,
                    Template = @"Hello from home assistant! Grid consumption: {{states('sensor.total_power')}}W"
                }
            };
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
            await _pageService.ShowPages(testPages, _cancellationTokenSource.Token).ConfigureAwait(false);
        }

        private void StopButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_cancellationTokenSource is null)
                return;
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = null;
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
        }

        private void TestButton_OnClick(object sender, RoutedEventArgs e)
        {
            var ctx = Log.DataContext as HomeViewModel;
            ctx.Messages.Add("this is a test");
        }
    }
}