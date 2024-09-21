using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dota2DraftHelper.DataBase;
using Dota2DraftHelper.Expansion_classes;
using Dota2DraftHelper.Models;
using Dota2DraftHelper.Services;
using Dota2DraftHelper.UserControls;
using Dota2DraftHelper.Views;
using FullControls.Controls;
using System.Collections.ObjectModel;

namespace Dota2DraftHelper.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty] ObservableCollection<OwnPick> heroesFromPool = null!;// Heroes from own hero pool
    [ObservableProperty] ObservableCollection<HeroControl> heroesFromPoolUI = null!;
    [ObservableProperty] ObservableCollection<ComboBoxItemPlus> hardSupports = null!;
    [ObservableProperty] ObservableCollection<ComboBoxItemPlus> supports = null!;
    [ObservableProperty] ObservableCollection<ComboBoxItemPlus> offlaners = null!;
    [ObservableProperty] ObservableCollection<ComboBoxItemPlus> carrys = null!;
    [ObservableProperty] ObservableCollection<ComboBoxItemPlus> midds = null!;
    [ObservableProperty] uint selectedLane = 0;
    [ObservableProperty] string bestPick = "";

    [ObservableProperty] ComboBoxItemPlusWithInfo? hSPick;
    [ObservableProperty] ComboBoxItemPlusWithInfo? sPick;
    [ObservableProperty] ComboBoxItemPlusWithInfo? offPick;
    [ObservableProperty] ComboBoxItemPlusWithInfo? carPick;
    [ObservableProperty] ComboBoxItemPlusWithInfo? midPick;

    [ObservableProperty] bool isProgressExecuting = true;
    [ObservableProperty] bool isUIAvailable;
    [ObservableProperty] bool isAllHeroes = false;

    [ObservableProperty] string bestAveragePick = "";
    [ObservableProperty] string bestAveragePickInfo = "";
    public MainWindowViewModel()
    {
        Init();
        SetUIAsync(SelectedLane);
    }

    private async void Init()// (OP)
    {
        IsUIAvailable = false;

        await DbServices.AddHeroesInDBAsync();
        await CacheHeroes.GetHeroesAsync();

        await DbServices.AddHeroWinRatesInDBAxync();
        await CacheWinRates.GetCounterPicksAsync();

        await CacheLanes.GetLanesAsync();
        await GetHeroesInComboBox();

        IsProgressExecuting = false;
        IsUIAvailable = true;
    }

    private async Task GetHeroesInComboBox()
    {
        IEnumerable<Hero> dbHeroes = (await CacheHeroes.GetHeroesAsync()).OrderBy(h => h.Name);

        HardSupports = new ObservableCollection<ComboBoxItemPlus>();
        Supports = new ObservableCollection<ComboBoxItemPlus>();
        Offlaners = new ObservableCollection<ComboBoxItemPlus>();
        Carrys = new ObservableCollection<ComboBoxItemPlus>();
        Midds = new ObservableCollection<ComboBoxItemPlus>();

        if (dbHeroes.Any())
        {
            foreach (var dbHero in dbHeroes)
            {
                HardSupports.Add(new ComboBoxItemPlusWithInfo()
                {
                    Content = dbHero.Name,
                    AdditionalInfo = dbHero.Id,
                    ToolTip = "You can write a hero name, just start typing..."
                });
                Supports.Add(new ComboBoxItemPlusWithInfo()
                {
                    Content = dbHero.Name,
                    AdditionalInfo = dbHero.Id,
                    ToolTip = "You can write a hero name, just start typing..."
                });
                Offlaners.Add(new ComboBoxItemPlusWithInfo()
                {
                    Content = dbHero.Name,
                    AdditionalInfo = dbHero.Id,
                    ToolTip = "You can write a hero name, just start typing..."
                });
                Carrys.Add(new ComboBoxItemPlusWithInfo()
                {
                    Content = dbHero.Name,
                    AdditionalInfo = dbHero.Id,
                    ToolTip = "You can write a hero name, just start typing..."
                });
                Midds.Add(new ComboBoxItemPlusWithInfo()
                {
                    Content = dbHero.Name,
                    AdditionalInfo = dbHero.Id,
                    ToolTip = "You can write a hero name, just start typing..."
                });
            }
        }
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

    [RelayCommand]
    private void Refresh()
    {
        HSPick = null;
        SPick= null;
        OffPick= null;
        CarPick= null;
        MidPick= null;

        BestAveragePick = null;
        BestAveragePickInfo = null;
        BestLanePickInfo = null;
        BestLanePick = null;
    }

    partial void OnSelectedLaneChanged(uint oldValue, uint newValue) // (OP)
    {
        SetUIAsync(newValue);
    }

    partial void OnHSPickChanged(ComboBoxItemPlusWithInfo? oldValue, ComboBoxItemPlusWithInfo? newValue)
    {
        GetBestAgainsPick();
    }

    partial void OnSPickChanged(ComboBoxItemPlusWithInfo? oldValue, ComboBoxItemPlusWithInfo? newValue)
    {
        GetBestAgainsPick();
    }

    partial void OnOffPickChanged(ComboBoxItemPlusWithInfo? oldValue, ComboBoxItemPlusWithInfo? newValue)
    {
        GetBestAgainsPick();
    }

    partial void OnCarPickChanged(ComboBoxItemPlusWithInfo? oldValue, ComboBoxItemPlusWithInfo? newValue)
    {
        GetBestAgainsPick();
    }

    partial void OnMidPickChanged(ComboBoxItemPlusWithInfo? oldValue, ComboBoxItemPlusWithInfo? newValue)
    {
        GetBestAgainsPick();
    }

    partial void OnIsAllHeroesChanged(bool oldValue, bool newValue)
    {
        Refresh();
    }

    private async void GetBestAgainsPick()
    {
        List<CounterPickInfo> winRates = await GetAllRequiredWinRatesAsync(); //Get all required winrates

        if (winRates.Count > 0)
        {
            await SetBestAveragePickUI(winRates);
        }
    }
    private async Task<List<CounterPickInfo>> GetAllRequiredWinRatesAsync()
    {
        var counterPicks = await CacheWinRates.GetCounterPicksAsync(); //Get all winrates

        var ownPicks = (await DbServices.GetOwnPicksAsync(SelectedLane)).Select(x => x.HeroId); // Get own picks

        List<int> enemyIds = new List<int>();

        if (HSPick != null) enemyIds.Add(Convert.ToInt32(HSPick.AdditionalInfo));
        if (SPick != null) enemyIds.Add(Convert.ToInt32(SPick.AdditionalInfo));
        if (OffPick != null) enemyIds.Add(Convert.ToInt32(OffPick.AdditionalInfo));
        if (CarPick != null) enemyIds.Add(Convert.ToInt32(CarPick.AdditionalInfo));
        if (MidPick != null) enemyIds.Add(Convert.ToInt32(MidPick.AdditionalInfo));

        if (!IsAllHeroes)
        {
            counterPicks = counterPicks.Where(x => ownPicks.Contains(x.PickId) && enemyIds.Contains(x.CounterPickId) &&
            x.WinRateDate == DateTime.Now.Date &&
                            x.CounterPickId != x.PickId).ToList();
        }
        else
        {
            counterPicks = counterPicks.Where(x => enemyIds.Contains(x.CounterPickId) && !enemyIds.Contains(x.PickId) && x.WinRateDate == DateTime.Now.Date &&
                            x.CounterPickId != x.PickId).ToList();
        }

        return counterPicks;
    }

    private async Task SetBestAveragePickUI(List<CounterPickInfo> winRates)
    {
        BestAveragePick = "";
        BestAveragePickInfo = "";

        string alternative = "";

        var averageWinRates = winRates.GroupBy(x => x.PickId).Select(g => new
        {
            PickId = g.Key,
            AverageWinRate = g.Average(x => x.WinRate)
        }).OrderByDescending(x => x.AverageWinRate).Take(10).ToList();

        var allHeroes = await CacheHeroes.GetHeroesAsync();

        var bestPicks = averageWinRates
                                .Select(cp => allHeroes.FirstOrDefault(hero => hero.Id == cp.PickId))
                                .Where(hero => hero != null)
                                .ToList();

        for (int i = 1; i < bestPicks.Count; i++)
        {
            alternative = alternative + $"\n{bestPicks[i]!.Name}: {averageWinRates.ElementAt(i).AverageWinRate:F2}%";
        }

        BestAveragePick = bestPicks.First()!.Name;
        BestAveragePickInfo = $"Faceit: {bestPicks.First()!.Faceit}\n" +
            $"Average WinRate: {averageWinRates.First().AverageWinRate:F2}%\n\n" +
            $"Other:" + alternative;
    }

}
