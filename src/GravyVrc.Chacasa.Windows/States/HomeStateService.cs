using HassClient.Models;
using HassClient.WS;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vogen;

namespace GravyVrc.Chacasa.Windows.States;

public class HomeStateService
{
    private readonly IHassProvider _apiProvider;
    private readonly IMemoryCache _cache;

    public HomeStateService(IHassProvider apiProvider, IMemoryCache cache)
    {
        _apiProvider = apiProvider;
        _cache = cache;
    }

    public async Task<IEnumerable<StateModel>> GetStatesAsync(CancellationToken cancellationToken = default) =>
        await _cache.GetOrCreateAsync("hass:states",
            async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                var api = await _apiProvider.GetClientAsync(cancellationToken);
                return await api.GetStatesAsync(cancellationToken);
            });
}

public interface IHassProvider
{
    Task<HassWSApi> GetClientAsync(CancellationToken cancellationToken = default);

    bool IsConfigured { get; }
}

public class HassProvider : IHassProvider
{
    private readonly HassConfiguration _configuration;
    private HassWSApi _api;

    public HassProvider(IOptionsSnapshot<HassConfiguration> options)
    {
        _configuration = options.Value;
    }

    public async Task<HassWSApi> GetClientAsync(CancellationToken cancellationToken = default) =>
        _api ??= await CreateNewClientAsync(cancellationToken);

    public async Task<HassWSApi> CreateNewClientAsync(CancellationToken cancellationToken = default)
    {
        HassWSApi api = new();
        var connectionParameters =
            ConnectionParameters.CreateFromInstanceBaseUrl(_configuration.Url, _configuration.Token);
        await api.ConnectAsync(connectionParameters, cancellationToken: cancellationToken);
        return api;
    }

    public bool IsConfigured => true; // TODO: this lol
}

public record struct StateChange(HomeEntity Entity, string NewValue, string OldValue);

public class HassConfiguration
{
    public string Url { get; set; }
    public string Token { get; set; }
}

public record struct HomeEntity(EntityId Id, string Name);

[ValueObject(typeof(string))]
public partial class EntityId
{
}