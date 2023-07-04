using System.Collections.ObjectModel;

namespace GravyVrc.Chacasa.Windows.ViewModels;

public class HomeViewModel
{
    public ObservableCollection<string> Messages { get; set; } = new();
}