using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dota2DraftHelper.DataBase;
using Dota2DraftHelper.Models;
using System.Windows;

namespace Dota2DraftHelper.ViewModels;

public partial class HeroViewModel : ObservableObject
{
    OwnPick ownPick;

    [ObservableProperty] string heroName = ""; // Hero name
    public HeroViewModel(Window parent, OwnPick ownPick)
    {
        this.ownPick = ownPick;
        HeroName = GetHeroNameFromDb(ownPick.HeroId);
    }

    private string GetHeroNameFromDb(int heroId)
    {
        return DbServices.GetHero(heroId).Name;
    }

    [RelayCommand]
    private void RemoveHeroFromPool()
    {
        if (DbServices.RemoveOwnHero(ownPick.HeroId, ownPick.LaneId))
        {
            MessageBox.Show($"{HeroName} was removed from your pick succesfully!", "Success", MessageBoxButton.OK,MessageBoxImage.Asterisk);
            return;
        }

        MessageBox.Show($"{HeroName} doesnt already contains in your pick!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
