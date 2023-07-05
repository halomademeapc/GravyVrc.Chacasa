using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GravyVrc.Chacasa.Windows.Templates;

namespace GravyVrc.Chacasa.Windows.ViewModels;

public class PageListViewModel : INotifyPropertyChanged
{
    private DisplayPage _selectedPage;
    public ObservableCollection<DisplayPage> Pages { get; set; } = new();

    public DisplayPage? SelectedPage
    {
        get => _selectedPage;
        set
        {
            if (Equals(value, _selectedPage)) return;
            _selectedPage = value;
            OnPropertyChanged();
        }
    }

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