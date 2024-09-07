using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dota2DraftHelper.DataBase;
using Dota2DraftHelper.Models;
using Dota2DraftHelper.UserControls;
using System.Collections.ObjectModel;

namespace Dota2DraftHelper.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty] ObservableCollection<HeroControl> heroesFromPool;// Heroes from own hero pool
    public MainWindowViewModel()
    {
        using (var db = new ApplicationDBContext())
        {
            
        }

        HeroesFromPool = new ObservableCollection<HeroControl>();
    }

    [RelayCommand]
    private void AddHeroInPool()
    {

    }
}
