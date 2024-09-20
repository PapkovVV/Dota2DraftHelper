using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dota2DraftHelper.DataBase;
using Dota2DraftHelper.Expansion_classes;
using Dota2DraftHelper.Models;
using FullControls.Controls;
using System.Collections.ObjectModel;
using System.Windows;

namespace Dota2DraftHelper.ViewModels;

public partial class AddHeroDialogViewModel : ObservableObject
{
    private Window callingWindow = null!;

    [ObservableProperty] ObservableCollection<ComboBoxItemPlus> heroes = new ObservableCollection<ComboBoxItemPlus>();
    [ObservableProperty] ObservableCollection<ComboBoxItemPlus> lanes = new ObservableCollection<ComboBoxItemPlus>();
    [ObservableProperty] ComboBoxItemPlusWithInfo selectedHero = null!;
    [ObservableProperty] ComboBoxItemPlusWithInfo selectedLane = null!;
    public AddHeroDialogViewModel(Window window)
    {
        callingWindow = window;
        GetHeroesInComboBoxAsync();
        GetLanesInComboBoxAsync();
    }

    private async void GetHeroesInComboBoxAsync()// (OP)
    {
        IEnumerable<Hero> dbHeroes = (await DbServices.GetHeroesAsync()).OrderBy(h => h.Name);

        if (dbHeroes.Any())
        {
            foreach (var dbHero in dbHeroes)
            {
                Heroes.Add(new ComboBoxItemPlusWithInfo()
                {
                    Content = dbHero.Name,
                    AdditionalInfo = dbHero.Id
                });
            }
        }
    }

    private async void GetLanesInComboBoxAsync()// (OP)
    {
        IEnumerable<Lane> dbLanes = await DbServices.GetLanesAsync();

        if (dbLanes.Any())
        {
            foreach (var dbLane in dbLanes)
            {
                Lanes.Add(new ComboBoxItemPlusWithInfo()
                {
                    Content = $"{dbLane.Name} ({dbLane.AlternativeName})",
                    AdditionalInfo = dbLane.Id
                });
            }
        }
    }

    [RelayCommand]
    private async Task SaveOwnHeroAsync() // (OP)
    {
        if (SelectedHero != null && SelectedLane != null)
        {
            bool saveResult = await DbServices.AddOwnHeroAsync(Convert.ToInt32(SelectedHero.AdditionalInfo), Convert.ToInt32(SelectedLane.AdditionalInfo));

            if (saveResult)
            {
                MessageBox.Show("Saved!", "Correct!", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                callingWindow.DialogResult = true;
            }
            else
            {
                MessageBox.Show("Current pick already exists!", "InCorrect data!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        else
        {
            MessageBox.Show("Mistake! Select the correct data!", "Incorrect data!", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
