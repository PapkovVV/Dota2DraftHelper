using CommunityToolkit.Mvvm.ComponentModel;
using Dota2DraftHelper.Services;
using System.Windows;

namespace Dota2DraftHelper.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty] bool isAllHeroes = false;
    [ObservableProperty] bool canWriteHeroesNames = false;

    public SettingsViewModel()
    {
        InitAsync();
    }

    private async void InitAsync()
    {
        var (isAllHeroes, canWriteHeroesNames) = await JSONServices.LoadSettingsAsync();
        IsAllHeroes = isAllHeroes;
        CanWriteHeroesNames = canWriteHeroesNames;
    }
    partial void OnCanWriteHeroesNamesChanged(bool oldValue, bool newValue)
    {
        JSONServices.SaveSettings(IsAllHeroes, newValue);
    }

    partial void OnIsAllHeroesChanged(bool oldValue, bool newValue)
    {
        JSONServices.SaveSettings(newValue, CanWriteHeroesNames);
    }
}
