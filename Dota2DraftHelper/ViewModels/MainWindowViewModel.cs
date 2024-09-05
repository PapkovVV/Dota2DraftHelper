using CommunityToolkit.Mvvm.ComponentModel;
using Dota2DraftHelper.UserControls;
using System.Collections.ObjectModel;

namespace Dota2DraftHelper.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty] ObservableCollection<HeroControl> heroesFromPool;// Heroes from own hero pool
    public MainWindowViewModel()
    {
        HeroesFromPool = new ObservableCollection<HeroControl>();
    }

}
