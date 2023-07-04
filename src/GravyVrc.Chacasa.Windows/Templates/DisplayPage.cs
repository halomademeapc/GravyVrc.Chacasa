using BuildSoft.VRChat.Osc.Chatbox;
using GravyVrc.Chacasa.Windows.Hass;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GravyVrc.Chacasa.Windows.Templates;

public class DisplayPage : ITemplateInput
{
    public int Id { get; set; }
    public string Label { get; set; }
    public string Template { get; set; }
    public TimeSpan? Duration { get; set; }
    public TimeSpan? RefreshPeriod { get; set; }
    public int Order { get; set; }
}

public class PageService
{
    private readonly TimeSpan _defaultDuration = TimeSpan.FromSeconds(10);

    public PageService()
    {
    }

    public async Task ShowPages(IReadOnlyList<DisplayPage> pages, CancellationToken cancellationToken)
    {
        var currentIndex = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            var currentPage = pages[currentIndex];
            await ShowRefreshingPageAsync(currentPage, cancellationToken);

            currentIndex++;
            currentIndex %= pages.Count - 1;
        }
    }

    public async Task ShowRefreshingPageAsync(DisplayPage page, CancellationToken cancellationToken)
    {
        if (page.RefreshPeriod is null || page.Duration is null)
        {
            await ShowPageAsync(page, cancellationToken).ConfigureAwait(false);
            await Task.Delay(page.Duration ?? _defaultDuration, cancellationToken).ConfigureAwait(false);
            return;
        }

        var iterations = Math.Floor(page.Duration.Value / page.RefreshPeriod.Value);
        for (var i = 0; i < iterations; i++)
        {
            await ShowPageAsync(page, cancellationToken).ConfigureAwait(false);
            await Task.Delay(page.RefreshPeriod.Value, cancellationToken).ConfigureAwait(false);
        }
    }

    private static async Task ShowPageAsync(ITemplateInput page, CancellationToken cancellationToken)
    {
        try
        {
            var templateService = App.Services.GetRequiredService<HomeTemplateService>();
            var rendered = await templateService.RenderTemplateAsync(page, cancellationToken);
            ChatService.SendMessage(rendered);
        }
        catch (Exception ex)
        {
            ChatService.SendMessage("Unable to render template.");
        }
    }
}