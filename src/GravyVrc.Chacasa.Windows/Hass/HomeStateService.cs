using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HassClient.Models;
using Microsoft.Extensions.Caching.Memory;

namespace GravyVrc.Chacasa.Windows.Hass;

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