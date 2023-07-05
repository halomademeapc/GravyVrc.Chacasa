using GravyVrc.Chacasa.Windows.Helpers;
using GravyVrc.Chacasa.Windows.Templates;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GravyVrc.Chacasa.Windows;

public sealed partial class PageEditorControl : UserControl
{
    private static readonly TimeSpan DefaultRefresh = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan DefaultDuration = TimeSpan.FromSeconds(5);
    private bool IsChanging = false;
    private DisplayPage Page => DataContext as DisplayPage;

    public event PageRemovedHandler Removed;

    public PageEditorControl()
    {
        this.InitializeComponent();
        DataContextChanged += (_, args) => UpdateSliders(args);
    }

    private void Refresh_Checked(object sender, RoutedEventArgs e)
    {
        if (IsChanging)
            return;
        RefreshSlider.IsEnabled = true;
        Page.RefreshPeriod = DefaultRefresh;
    }

    private void Refresh_Unchecked(object sender, RoutedEventArgs e)
    {
        if (IsChanging)
            return;
        RefreshSlider.IsEnabled = false;
        Page.RefreshPeriod = null;
    }

    private void RefreshSlider_OnValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        if (IsChanging)
            return;
        Page.RefreshPeriod = TimeSpan.FromMilliseconds(e.NewValue);
    }

    private void DurationSlider_OnValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        if (IsChanging)
            return;
        Page.Duration = TimeSpan.FromMilliseconds(e.NewValue);
    }

    private void UpdateSliders(DataContextChangedEventArgs args)
    {
        IsChanging = true;
        var vm = args.NewValue as DisplayPage;
        if (vm is null)
            return;
        RefreshSlider.IsEnabled = vm.RefreshPeriod.HasValue;
        RefreshCheckbox.IsChecked = vm.RefreshPeriod.HasValue;
        RefreshSlider.Value = (vm.RefreshPeriod ?? DefaultRefresh).TotalMilliseconds;
        DurationSlider.Value = vm.Duration.TotalMilliseconds;
        this.RunOnUiThread(() => IsChanging = false);
    }

    private void RemoveButton_OnClick(object sender, RoutedEventArgs e)
    {
        Removed?.Invoke(Page);
    }

    public delegate void PageRemovedHandler(DisplayPage page);
}