using System;
using System.Threading;
using System.Threading.Tasks;
using GravyVrc.Chacasa.Windows.Data;
using Simple.HAApi;

namespace GravyVrc.Chacasa.Windows.Hass;

public class HassProvider : IHassProvider
{
    private HassConfiguration? _configuration;
    private readonly SettingsService _settings;
    private Instance _api;

    public HassProvider(SettingsService settings)
    {
        _settings = settings;
    }

    public async Task<Instance> GetClientAsync(CancellationToken cancellationToken = default) =>
        _api ??= await CreateNewClientAsync(cancellationToken);

    public async Task<bool> ValidateConfigurationAsync(HassConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var instance = new Instance(new(configuration.Url), configuration.Token);
            return await instance.CheckRunningAsync();
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

    public async Task<Instance> CreateNewClientAsync(CancellationToken cancellationToken = default)
    {
        if (_configuration is null)
            await LoadConfigurationAsync(cancellationToken);
        return new Instance(new Uri(_configuration!.Url), _configuration.Token);
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