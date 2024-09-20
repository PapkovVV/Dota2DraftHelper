using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dota2DraftHelper.DataBase;
using Dota2DraftHelper.Models;
using System.Windows;

namespace Dota2DraftHelper.ViewModels;

public partial class HeroViewModel : ObservableObject
{
    OwnPick ownPick;
    MainWindowViewModel parentViewModel;

    [ObservableProperty] string heroName = ""; // Hero name
    public HeroViewModel(MainWindowViewModel parentViewModel, OwnPick ownPick)
    {
        this.parentViewModel = parentViewModel;
        this.ownPick = ownPick;
        Init();
    }

    private async void Init() // (OP)
    {
        HeroName = await GetHeroNameFromDbAsync(ownPick.HeroId);
    }
    private async Task<string> GetHeroNameFromDbAsync(int heroId) // (OP)
    {
        Hero? hero = await DbServices.GetHeroAsync(heroId);

        if (hero != null)
        {
            return hero.Name;
        }

        return "";
    }

    [RelayCommand]
    private async Task RemoveHeroFromPoolAsync() // (OP)
    {
        if (await DbServices.RemoveOwnHeroAsync(ownPick.HeroId, ownPick.LaneId))
        {
            MessageBox.Show($"{HeroName} was removed from your pick succesfully!", "Success", MessageBoxButton.OK,MessageBoxImage.Asterisk);
            parentViewModel.SetUIAsync(Convert.ToUInt32(ownPick.LaneId));
            return;
        }

        MessageBox.Show($"{HeroName} doesnt already contains in your pick!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
