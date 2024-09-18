using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dota2DraftHelper.DataBase;
using Dota2DraftHelper.Expansion_classes;
using Dota2DraftHelper.Models;
using FullControls.Controls;
using System.Collections.ObjectModel;
using System.Windows;

namespace Dota2DraftHelper.ViewModels;

public partial class AddHeroDialogViewModel:ObservableObject
{
    private Window callingWindow = null;

    [ObservableProperty] ObservableCollection<ComboBoxItemPlus> heroes = new ObservableCollection<ComboBoxItemPlus>();
    [ObservableProperty] ObservableCollection<ComboBoxItemPlus> lanes = new ObservableCollection<ComboBoxItemPlus>();
    [ObservableProperty] ComboBoxItemPlusWithInfo selectedHero;
    [ObservableProperty] ComboBoxItemPlusWithInfo selectedLane;
    public AddHeroDialogViewModel(Window window)
    {
        callingWindow = window;
        GetHeroesInComboBox();
        GetLanesInComboBox();
    }

    private void GetHeroesInComboBox()
    {
        IEnumerable<Hero> dbHeroes = DbServices.GetHeroes().OrderBy(h => h.Name);

        foreach (var dbHero in dbHeroes)
        {
            Heroes.Add(new ComboBoxItemPlusWithInfo()
            {
                Content = dbHero.Name,
                AdditionalInfo = dbHero.Id
            });
        }
    }

    private void GetLanesInComboBox()
    {
        IEnumerable<Lane> dbLanes = DbServices.GetLanes();

        foreach (var dbLane in dbLanes)
        {
            Lanes.Add(new ComboBoxItemPlusWithInfo()
            {
                Content = $"{dbLane.Name} ({dbLane.AlternativeName})",
                AdditionalInfo = dbLane.Id
            });
        }

    }

    [RelayCommand]
    private void SaveOwnHero()
    {
        if (SelectedHero != null && SelectedLane != null)
        {
            bool saveResult = DbServices.AddOwnHero(Convert.ToInt32(SelectedHero.AdditionalInfo), Convert.ToInt32(SelectedLane.AdditionalInfo));

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
            MessageBox.Show("Mistake! Select the correct data!","Incorrect data!",MessageBoxButton.OK,MessageBoxImage.Error);
        }
    }
}
