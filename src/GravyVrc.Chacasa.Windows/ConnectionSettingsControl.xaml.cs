using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Threading;
using System.Threading.Tasks;
using GravyVrc.Chacasa.Windows.Hass;
using GravyVrc.Chacasa.Windows.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace GravyVrc.Chacasa.Windows;

public sealed partial class ConnectionSettingsControl : UserControl
{
    private ContentDialog? _dialogRef;
    private readonly IHassProvider _hassProvider;
    private readonly CancellationTokenSource _cancellationSource = new();

    public ConnectionSettingsControl()
    {
        this.InitializeComponent();
        _hassProvider = App.Services.GetRequiredService<IHassProvider>();
    }

    public async void Initialize(ContentDialog dialogRef)
    {
        _dialogRef = dialogRef;
        var config = await _hassProvider.GetConfigurationAsync(_cancellationSource.Token);
        this.RunOnUiThread(() => SetConfigurationModel(config));
    }

    private async void Test_OnClick(object sender, RoutedEventArgs e)
    {
        SetEnabled(false);
        try
        {
            SetLoadingMessage(LoadingState.Progressing);
            var result = await _hassProvider
                .ValidateConfigurationAsync(GetConfigurationModel(), _cancellationSource.Token).ConfigureAwait(false);
            this.RunOnUiThread(() => SetLoadingMessage(result ? LoadingState.Success : LoadingState.Failure));
        }
        catch (TaskCanceledException) //expected
        {
        }

        this.RunOnUiThread(() => SetEnabled(true));
    }

    private HassConfiguration GetConfigurationModel() => new()
    {
        Token = TokenTextBox.Text,
        Url = UrlTextBox.Text
    };

    private void SetConfigurationModel(HassConfiguration config)
    {
        TokenTextBox.Text = config.Token;
        UrlTextBox.Text = config.Url;
    }

    private void Cancel_OnClick(object sender, RoutedEventArgs e)
    {
        _cancellationSource.Cancel();
        _dialogRef?.Hide();
    }

    private async void Save_OnClick(object sender, RoutedEventArgs e)
    {
        await _hassProvider.SaveConfigurationAsync(GetConfigurationModel());
        _dialogRef?.Hide();
    }

    private void SetLoadingMessage(LoadingState? state)
    {
        LoadingMessage.Visibility = state == LoadingState.Progressing ? Visibility.Visible : Visibility.Collapsed;
        SuccessMessage.Visibility = state == LoadingState.Success ? Visibility.Visible : Visibility.Collapsed;
        FailureMessage.Visibility = state == LoadingState.Failure ? Visibility.Visible : Visibility.Collapsed;
        IdleMessage.Visibility = state == null ? Visibility.Visible : Visibility.Collapsed;
    }

    private void SetEnabled(bool isEnabled)
    {
        UrlTextBox.IsEnabled = isEnabled;
        TokenTextBox.IsEnabled = isEnabled;
        SaveButton.IsEnabled = isEnabled;
        TestButton.IsEnabled = isEnabled;
    }

    private enum LoadingState
    {
        Success,
        Failure,
        Progressing
    }
}