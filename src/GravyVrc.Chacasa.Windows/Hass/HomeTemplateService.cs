using System.Threading;
using System.Threading.Tasks;

namespace GravyVrc.Chacasa.Windows.Hass;

public class HomeTemplateService
{
    private readonly IHassProvider _apiProvider;

    public HomeTemplateService(IHassProvider apiProvider, CancellationToken cancellationToken = default)
    {
        _apiProvider = apiProvider;
    }

    public Task<string> RenderTemplateAsync(ITemplateInput input, CancellationToken cancellationToken = default) => RenderTemplateAsync(input.Template, cancellationToken);

    public async Task<string> RenderTemplateAsync(string input, CancellationToken cancellationToken = default)
    {
        var api = await _apiProvider.GetClientAsync(cancellationToken);
        return await api.RenderTemplateAsync(input, cancellationToken);
    }
}