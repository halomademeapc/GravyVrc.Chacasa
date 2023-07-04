using GravyVrc.Chacasa.Windows.Hass;
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
        if (page.RefreshPeriod is null || page.Duration is null)
        {
            await ShowPageAsync(page, cancellationToken);
            await Task.Delay(page.Duration ?? _defaultDuration, cancellationToken);
            return;
        }

        var iterations = Math.Floor(page.Duration.Value / page.RefreshPeriod.Value);
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