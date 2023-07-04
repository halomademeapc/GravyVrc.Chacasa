using System.Threading;
using System.Threading.Tasks;
using GravyVrc.Chacasa.Windows.Data;
using HassClient.WS;

namespace GravyVrc.Chacasa.Windows.Hass;

public class HassProvider : IHassProvider
{
    private HassConfiguration? _configuration;
    private readonly SettingsService _settings;
    private HassWSApi _api;

    public HassProvider(SettingsService settings)
    {
        _settings = settings;
    }

    public async Task<HassWSApi> GetClientAsync(CancellationToken cancellationToken = default) =>
        _api ??= await CreateNewClientAsync(cancellationToken);

    public async Task<bool> ValidateConfigurationAsync(HassConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        try
        {
            HassWSApi api = new();
            var connectionParameters =
                ConnectionParameters.CreateFromInstanceBaseUrl(configuration.Url, configuration.Token);
            await api.ConnectAsync(connectionParameters, cancellationToken: cancellationToken);
            await api.CloseAsync(CancellationToken.None);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task SaveConfigurationAsync(HassConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        await _settings.SetAsync(SettingsService.UrlKey, configuration.Url, cancellationToken);
        await _settings.SetAsync(SettingsService.TokenKey, configuration.Token, cancellationToken);
        _configuration = configuration;
    }

    public async Task<HassWSApi> CreateNewClientAsync(CancellationToken cancellationToken = default)
    {
        if (_configuration is null)
            await LoadConfigurationAsync(cancellationToken);
        HassWSApi api = new();
        var connectionParameters =
            ConnectionParameters.CreateFromInstanceBaseUrl(_configuration.Url, _configuration.Token);
        await api.ConnectAsync(connectionParameters, cancellationToken: cancellationToken);
        return api;
    }

    private async Task LoadConfigurationAsync(CancellationToken cancellationToken = default)
    {
        _configuration = new()
        {
            Url = (await _settings.GetAsync(SettingsService.UrlKey, cancellationToken))?.Value,
            Token = (await _settings.GetAsync(SettingsService.TokenKey, cancellationToken))?.Value
        };
    }

    public bool IsConfigured => true; // TODO: this lol
}