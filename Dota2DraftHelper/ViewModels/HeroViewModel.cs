using CommunityToolkit.Mvvm.ComponentModel;
using Dota2DraftHelper.Models;

namespace Dota2DraftHelper.ViewModels;

public partial class HeroViewModel: ObservableObject
{
    [ObservableProperty] string heroName = ""; // Hero name
    public HeroViewModel(Hero hero)
    {
        HeroName = hero.Name;
    }
}
