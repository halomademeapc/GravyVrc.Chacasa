using GravyVrc.Chacasa.Windows.Hass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GravyVrc.Chacasa.Windows.Templates;

public class DisplayPage : ITemplateInput, INotifyPropertyChanged
{
    public int Id { get; set; }

    private TimeSpan _duration;
    private TimeSpan? _refreshPeriod;
    private string _template;
    private string _label;

    public string Label
    {
        get => _label;
        set
        {
            if (value == _label) return;
            _label = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(DisplayLabel));
        }
    }

    public string Template
    {
        get => _template;
        set
        {
            if (value == _template) return;
            _template = value;
            OnPropertyChanged();
        }
    }

    public TimeSpan Duration
    {
        get => _duration;
        set
        {
            if (Nullable.Equals(value, _duration)) return;
            _duration = value;
            OnPropertyChanged();
            OnPropertyChanged();
        }
    }

    public TimeSpan? RefreshPeriod
    {
        get => _refreshPeriod;
        set
        {
            if (Nullable.Equals(value, _refreshPeriod)) return;
            _refreshPeriod = value;
            OnPropertyChanged();
            OnPropertyChanged();
        }
    }

    public int Order { get; set; }

    public string DisplayLabel => Label ?? "New Page";
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}