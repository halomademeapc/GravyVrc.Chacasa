using GravyVrc.Chacasa.Windows.States;
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

        public MainWindow()
        {
            this.InitializeComponent();
            _stateService = App.Services.GetRequiredService<HomeStateService>();
        }

        private async void myButton_Click(object sender, RoutedEventArgs e)
        {
            myButton.Content = "Clicked";
            await _stateService.GetStatesAsync().ConfigureAwait(false);
        }
    }
}
