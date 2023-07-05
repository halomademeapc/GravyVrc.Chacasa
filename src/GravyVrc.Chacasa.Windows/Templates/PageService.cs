using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GravyVrc.Chacasa.Windows.Hass;

namespace GravyVrc.Chacasa.Windows.Templates;

public class PageService
{
    private readonly TimeSpan _defaultDuration = TimeSpan.FromSeconds(10);
    private readonly HomeTemplateService _templateService;

    public PageService(HomeTemplateService templateService)
    {
        _templateService = templateService;
    }

    public async Task ShowPages(IReadOnlyList<DisplayPage> pages, CancellationToken cancellationToken)
    {
        try
        {
            var currentIndex = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                var currentPage = pages[currentIndex];
                await ShowRefreshingPageAsync(currentPage, cancellationToken);

                currentIndex++;
                currentIndex %= pages.Count;
            }
        }
        catch (TaskCanceledException)
        { //expected
        }
    }

    public async Task ShowRefreshingPageAsync(DisplayPage page, CancellationToken cancellationToken)
    {
        if (page.RefreshPeriod is null)
        {
            await ShowPageAsync(page, cancellationToken);
            await Task.Delay(page.Duration, cancellationToken);
            return;
        }

        var iterations = Math.Floor(page.Duration / page.RefreshPeriod.Value);
        for (var i = 0; i < iterations; i++)
        {
            await ShowPageAsync(page, cancellationToken);
            await Task.Delay(page.RefreshPeriod.Value, cancellationToken);
        }
    }

    private async Task ShowPageAsync(ITemplateInput page, CancellationToken cancellationToken)
    {
        try
        {
            var renderTask = _templateService.RenderTemplateAsync(page.Template, cancellationToken).ContinueWith(r => ChatService.SendMessage(r.Result), cancellationToken);
            var timeoutTask = Task.Delay(5000, cancellationToken);
            await Task.WhenAny(renderTask, timeoutTask);
            if (renderTask.IsCompleted)
                return;

            ChatService.SendMessage("Template took too long to render...");
        }
        catch (Exception ex)
        {
            ChatService.SendMessage("Unable to render template.");
        }
    }
}