using System.Threading;
using System.Threading.Tasks;
using HassClient.WS;

namespace GravyVrc.Chacasa.Windows.Hass;

public interface IHassProvider
{
    Task<HassWSApi> GetClientAsync(CancellationToken cancellationToken = default);

    Task<bool> ValidateConfigurationAsync(HassConfiguration configuration, CancellationToken cancellationToken = default);

    Task SaveConfigurationAsync(HassConfiguration configuration, CancellationToken cancellationToken = default);

    bool IsConfigured { get; }
}