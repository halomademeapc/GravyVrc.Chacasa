using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using GravyVrc.Chacasa.Windows.Templates;
using GravyVrc.Chacasa.Windows.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GravyVrc.Chacasa.Windows
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PageListWindow : Window
    {
        private PageListViewModel ViewModel => Root.DataContext as PageListViewModel;

        public PageListWindow()
        {
            this.InitializeComponent();
            LoadData();
        }

        private async Task LoadData()
        {
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
            foreach (var page in testPages)
            {
                ViewModel.Pages.Add(page);
            }
        }

        private void PageList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var page = e.AddedItems.FirstOrDefault();
            Editor.DataContext = page;
        }

        private void Editor_OnRemoved(DisplayPage page)
        {
            ViewModel.Pages.Remove(page);
            ViewModel.SelectedPage = null;
        }

        private void Add_OnClick(object sender, RoutedEventArgs e)
        {
            var page = new DisplayPage();
            ViewModel.Pages.Add(page);
            PageList.SelectedItem = page;
        }
    }
}