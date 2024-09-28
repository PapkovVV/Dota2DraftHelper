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
using System.ComponentModel;
using System.Windows.Data;

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
    [ObservableProperty] ObservableCollection<ComboBoxItemPlusWithInfo>? lanes = null;

    [ObservableProperty] ObservableCollection<Hero> bestAlternativeHeroes = null!;
    [ObservableProperty] ObservableCollection<Hero> worstAlternativeHeroes = null!;
    [ObservableProperty] ObservableCollection<Hero> bestHeroCounterList = null!;
    [ObservableProperty] ObservableCollection<Hero> worstHeroCounterList = null!;

    [ObservableProperty] uint selectedLane = 0;

    [ObservableProperty] ComboBoxItemPlusWithInfo? hSPick;
    [ObservableProperty] ComboBoxItemPlusWithInfo? sPick;
    [ObservableProperty] ComboBoxItemPlusWithInfo? offPick;
    [ObservableProperty] ComboBoxItemPlusWithInfo? carPick;
    [ObservableProperty] ComboBoxItemPlusWithInfo? midPick;

    [ObservableProperty] bool isProgressExecuting = true;
    [ObservableProperty] bool isUIAvailable;
    [ObservableProperty] bool isAllHeroes;
    [ObservableProperty] bool canWriteHeroesNames;
    [ObservableProperty] bool isAPAvailable = false;
    [ObservableProperty] bool isLanesAvailable = true;

    [ObservableProperty] string bestPick = "";
    [ObservableProperty] string bestAveragePick = "";
    [ObservableProperty] string bestAveragePickInfo = "";
    [ObservableProperty] string worstAveragePick = "";
    [ObservableProperty] string worstAveragePickInfo = "";
    [ObservableProperty] string hSPickText = "";
    [ObservableProperty] string sPickText = "";
    [ObservableProperty] string offPickText = "";
    [ObservableProperty] string carPickText = "";
    [ObservableProperty] string midPickText = "";
    [ObservableProperty] string bestAlternativePickName = "";
    [ObservableProperty] string worstAlternativePickName = "";

    [ObservableProperty] byte[]? bestAveragePickImage = null;
    [ObservableProperty] byte[]? worstAveragePickImage = null;
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

        await GetJsonSettings();

        IsProgressExecuting = false;
        IsUIAvailable = true;

        if (IsAllHeroes)
        {
            Lanes = null;
            IsLanesAvailable = false;
        }
        else
        {
            Lanes = new ObservableCollection<ComboBoxItemPlusWithInfo>()
            {   new ComboBoxItemPlusWithInfo(){ Content = "Safe Lane (Pos1)"},
                new ComboBoxItemPlusWithInfo(){ Content = "Mid Lane (Pos2)"},
                new ComboBoxItemPlusWithInfo(){ Content = "Hard Lane (Pos3)"},
                new ComboBoxItemPlusWithInfo(){ Content = "Support (Pos4)"},
                new ComboBoxItemPlusWithInfo(){ Content = "Hard Support (Pos5)"}};
        }
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

    private async void GetBestAgainsPick()
    {
        if (HSPick != null || SPick != null || OffPick != null || CarPick != null || MidPick != null)
        {
            List<CounterPickInfo> winRates = await GetAllRequiredWinRatesAsync(); //Get all required winrates

            if (winRates.Count > 0)
            {
                IsAPAvailable = true;
                await SetBestAveragePickUI(winRates);
                await SetWorstAveragePickUI(winRates);
            }
        }
        else
        {
            Refresh();
        }
    }

    private async Task<List<CounterPickInfo>> GetAllRequiredWinRatesAsync()
    {
        var counterPicks = await CacheWinRates.GetCounterPicksAsync(); //Get all winrates

        List<int> enemyIds = new List<int>();

        if (HSPick != null) enemyIds.Add(Convert.ToInt32(HSPick.AdditionalInfo));
        if (SPick != null) enemyIds.Add(Convert.ToInt32(SPick.AdditionalInfo));
        if (OffPick != null) enemyIds.Add(Convert.ToInt32(OffPick.AdditionalInfo));
        if (CarPick != null) enemyIds.Add(Convert.ToInt32(CarPick.AdditionalInfo));
        if (MidPick != null) enemyIds.Add(Convert.ToInt32(MidPick.AdditionalInfo));

        if (enemyIds.Count > 0)
        {
            if (!IsAllHeroes)
            {
                var ownPicks = (await DbServices.GetOwnPicksAsync(SelectedLane)).Select(x => x.HeroId); // Get own picks

                counterPicks = counterPicks.Where(x => ownPicks.Contains(x.PickId) && enemyIds.Contains(x.CounterPickId) && x.CounterPickId != x.PickId).ToList();
            }
            else
            {
                counterPicks = counterPicks.Where(x => enemyIds.Contains(x.CounterPickId) && !enemyIds.Contains(x.PickId) && x.CounterPickId != x.PickId).ToList();
            }
        }

        return counterPicks;
    }

    private async Task SetBestAveragePickUI(List<CounterPickInfo> winRates)
    {
        BestAveragePick = "";
        BestAveragePickImage = null;
        BestAveragePickInfo = "";
        BestAlternativeHeroes = new ObservableCollection<Hero>();
        BestHeroCounterList = new ObservableCollection<Hero>();

        var averageWinRates = winRates.GroupBy(x => x.PickId).Select(g => new
        {
            PickId = g.Key,
            AverageWinRate = g.Sum(x => x.WinRate)
        }).OrderBy(x => x.AverageWinRate).ToList();

        var allHeroes = await CacheHeroes.GetHeroesAsync();

        var bestPicks = averageWinRates
                                .Select(cp => allHeroes.FirstOrDefault(hero => hero.Id == cp.PickId))
                                .Where(hero => hero != null)
                                .ToList();

        for (int i = 1; i < bestPicks.Count; i++)
        {
            bestPicks[i]!.WinRate = $": {averageWinRates.ElementAt(i).AverageWinRate:F2}%";
            bestPicks[i]!.SelectedBestHeroCounterList = GetHeroCounterList(allHeroes, winRates, bestPicks[i]!.Id);
            BestAlternativeHeroes.Add(bestPicks[i]!);
        }

        BestAveragePick = bestPicks.First()!.Name;
        BestAveragePickImage = bestPicks.First()!.ImageData;
        BestAveragePickInfo = $"Faceit: {bestPicks.First()!.Faceit}\n" +
            $"Average WinRate: {averageWinRates.First().AverageWinRate:F2}%\n";
        BestHeroCounterList = GetHeroCounterList(allHeroes, winRates, bestPicks.First()!.Id);
    }

    private async Task SetWorstAveragePickUI(List<CounterPickInfo> winRates)
    {
        WorstAveragePick = "";
        WorstAveragePickImage = null;
        WorstAveragePickInfo = "";
        WorstAlternativeHeroes = new ObservableCollection<Hero>();
        WorstHeroCounterList = new ObservableCollection<Hero>();

        var averageWinRates = winRates.GroupBy(x => x.PickId).Select(g => new
        {
            PickId = g.Key,
            AverageWinRate = g.Sum(x => x.WinRate)
        }).OrderByDescending(x => x.AverageWinRate).ToList();

        var allHeroes = await CacheHeroes.GetHeroesAsync();

        var worstPicks = averageWinRates
                                .Select(cp => allHeroes.FirstOrDefault(hero => hero.Id == cp.PickId))
                                .Where(hero => hero != null)
                                .ToList();

        for (int i = 1; i < worstPicks.Count; i++)
        {
            worstPicks[i]!.WinRate = $": {averageWinRates.ElementAt(i).AverageWinRate:F2}%";
            worstPicks[i]!.SelectedWorstHeroCounterList = GetHeroCounterList(allHeroes, winRates, worstPicks[i]!.Id);
            WorstAlternativeHeroes.Add(worstPicks[i]!);
        }

        WorstAveragePick = worstPicks.First()!.Name;
        WorstAveragePickImage = worstPicks.First()!.ImageData;
        WorstAveragePickInfo = $"Faceit: {worstPicks.First()!.Faceit}\n" +
            $"Average WinRate: {averageWinRates.First().AverageWinRate:F2}%\n";
        WorstHeroCounterList = GetHeroCounterList(allHeroes, winRates, worstPicks.First()!.Id);
    }

    private ObservableCollection<Hero> GetHeroCounterList(List<Hero> allHeroes, List<CounterPickInfo> winRates, int heroId)
    {
        ObservableCollection<Hero> counterHeroList = new ObservableCollection<Hero>();

        var contributingIds = winRates.Where(x => x.PickId == heroId).ToList();

        foreach (var item in contributingIds)
        {
            Hero searchedHero = allHeroes.FirstOrDefault(x => x.Id == item.CounterPickId)!;
            counterHeroList.Add(new Hero
            {
                Name = searchedHero.Name,
                ImageData = searchedHero.ImageData,
                WinRate = $": {item.WinRate:F2}%"
            });
        }
        return counterHeroList;
    }

    private async Task GetJsonSettings()
    {
        var (isAllHeroes, canWriteHeroesNames) = await JSONServices.LoadSettingsAsync();
        IsAllHeroes = isAllHeroes;
        CanWriteHeroesNames = canWriteHeroesNames;
    }

    private async void RefreshBestAlternativePicks()
    {
        await SetBestAveragePickUI(await GetAllRequiredWinRatesAsync());
    }

    private async void RefreshWorstAlternativePicks()
    {
        await SetWorstAveragePickUI(await GetAllRequiredWinRatesAsync());
    }
    #region Events

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
        IsAPAvailable = false;

        HSPick = null;
        SPick= null;
        OffPick= null;
        CarPick= null;
        MidPick= null;

        BestAveragePick = null;
        BestAveragePickImage = null;
        BestAveragePickInfo = null;
        BestAlternativeHeroes = null;
        WorstAveragePick = null;
        WorstAveragePickImage = null;
        WorstAveragePickInfo = null;
        WorstAlternativeHeroes = null;

        HSPickText = "";
        SPickText = "";
        OffPickText = "";
        CarPickText = "";
        MidPickText = "";

        BestAlternativePickName = "";
        WorstAlternativePickName = "";
    }

    [RelayCommand]
    private void ClearHardSupport()
    {
        HSPick = null;
        HSPickText = "";
    }

    [RelayCommand]
    private void ClearSupport()
    {
        SPick = null;
        SPickText = "";
    }

    [RelayCommand]
    private void ClearOfflane()
    {
        OffPick = null;
        OffPickText = "";
    }

    [RelayCommand]
    private void ClearCarry()
    {
        CarPick = null;
        CarPickText = "";
    }

    [RelayCommand]
    private void ClearMid()
    {
        MidPick = null;
        MidPickText = "";
    }

    [RelayCommand]
    private async Task SettingsAsync()
    {
        SettingsDialog settingsDialog = new SettingsDialog();

        if (settingsDialog.ShowDialog() == true)
        {
            await GetJsonSettings();

            if (IsAllHeroes)
            {
                Lanes = null;
                IsLanesAvailable = false;
            }
            else
            {
                IsLanesAvailable = true;

                Lanes = new ObservableCollection<ComboBoxItemPlusWithInfo>()
            {   new ComboBoxItemPlusWithInfo(){ Content = "Safe Lane (Pos1)"},
                new ComboBoxItemPlusWithInfo(){ Content = "Mid Lane (Pos2)"},
                new ComboBoxItemPlusWithInfo(){ Content = "Hard Lane (Pos3)"},
                new ComboBoxItemPlusWithInfo(){ Content = "Support (Pos4)"},
                new ComboBoxItemPlusWithInfo(){ Content = "Hard Support (Pos5)"}};
            }
        }
    }

    partial void OnBestAlternativePickNameChanging(string value)
    {
        RefreshBestAlternativePicks();
        BestAlternativeHeroes = new ObservableCollection<Hero>(BestAlternativeHeroes.Where(hero => hero.Name.ToLower().StartsWith(value.ToLower())));
    }

    partial void OnWorstAlternativePickNameChanging(string value)
    {
        RefreshWorstAlternativePicks();
        WorstAlternativeHeroes = new ObservableCollection<Hero>(WorstAlternativeHeroes.Where(hero => hero.Name.ToLower().StartsWith(value.ToLower())));
    }

    #endregion Events
}
