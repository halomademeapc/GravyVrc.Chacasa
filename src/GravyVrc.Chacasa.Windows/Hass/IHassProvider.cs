using System.Threading;
using System.Threading.Tasks;
using HassClient.WS;
using Simple.HAApi;

namespace GravyVrc.Chacasa.Windows.Hass;

public interface IHassProvider
{
    Task<Instance> GetClientAsync(CancellationToken cancellationToken = default);

    Task<bool> ValidateConfigurationAsync(HassConfiguration configuration, CancellationToken cancellationToken = default);

    Task SaveConfigurationAsync(HassConfiguration configuration, CancellationToken cancellationToken = default);

    bool IsConfigured { get; }
}