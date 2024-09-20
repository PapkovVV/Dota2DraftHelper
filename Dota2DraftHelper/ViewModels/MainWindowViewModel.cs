using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dota2DraftHelper.DataBase;
using Dota2DraftHelper.Models;
using Dota2DraftHelper.UserControls;
using Dota2DraftHelper.Views;
using System.Collections.ObjectModel;

namespace Dota2DraftHelper.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty] ObservableCollection<OwnPick> heroesFromPool = null!;// Heroes from own hero pool
    [ObservableProperty] ObservableCollection<HeroControl> heroesFromPoolUI = null!;
    [ObservableProperty] uint selectedLane = 0;
    public MainWindowViewModel()
    {
        DbServices.AddHeroesAsync();
        SetUIAsync(SelectedLane);
    }

    public async void SetUIAsync(uint laneId) // Set UI (OP)
    {
        HeroesFromPool = new ObservableCollection<OwnPick>(await DbServices.GetOwnPicksAsync(laneId));

        HeroesFromPoolUI = new ObservableCollection<HeroControl>();

        if (HeroesFromPool.Count > 0)
        {
            switch (SelectedLane)
            {
                case 0:
                    {
                        var carryHeroes = HeroesFromPool.Where(x => x.LaneId == 1);

                        foreach (var carry in carryHeroes)
                        {
                            HeroesFromPoolUI.Add(new HeroControl(carry));
                        }

                        break;
                    }
                case 1:
                    {
                        {
                            var midders = HeroesFromPool.Where(x => x.LaneId == 2);

                            foreach (var midder in midders)
                            {
                                HeroesFromPoolUI.Add(new HeroControl(midder));
                            }

                            break;
                        }
                    }
                case 2:
                    {
                        var offlaners = HeroesFromPool.Where(x => x.LaneId == 3);

                        foreach (var offlaner in offlaners)
                        {
                            HeroesFromPoolUI.Add(new HeroControl(offlaner));
                        }

                        break;
                    }
                case 3:
                    {
                        var supports = HeroesFromPool.Where(x => x.LaneId == 4);

                        foreach (var support in supports)
                        {
                            HeroesFromPoolUI.Add(new HeroControl(support));
                        }

                        break;
                    }
                case 4:
                    {
                        var hardSupports = HeroesFromPool.Where(x => x.LaneId == 5);

                        foreach (var hardSupport in hardSupports)
                        {
                            HeroesFromPoolUI.Add(new HeroControl(hardSupport));
                        }

                        break;
                    }
            }
        }
    }

    [RelayCommand]
    private void AddHeroInPool() // (OP)
    {
        AddHeroDialog addHeroDialog = new AddHeroDialog();

        if (addHeroDialog.ShowDialog() == true)
        {
            SetUIAsync(SelectedLane);
        }
    }

    partial void OnSelectedLaneChanged(uint oldValue, uint newValue) // (OP)
    {
        SetUIAsync(newValue);
    }
}
