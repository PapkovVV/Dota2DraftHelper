using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dota2DraftHelper.DataBase;
using Dota2DraftHelper.UserControls;
using Dota2DraftHelper.Views;
using System.Collections.ObjectModel;

namespace Dota2DraftHelper.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty] ObservableCollection<HeroControl> heroesFromPool;// Heroes from own hero pool
    public MainWindowViewModel()
    {
        DbCalls();

        HeroesFromPool = new ObservableCollection<HeroControl>();
    }

    private static void DbCalls() // Call DB Methods
    {
        DbServices.AddHeroes();
        DbServices.AddLanes();
    }



    [RelayCommand]
    private void AddHeroInPool()
    {
        AddHeroDialog addHeroDialog = new AddHeroDialog();
        addHeroDialog.ShowDialog();
    }
}
