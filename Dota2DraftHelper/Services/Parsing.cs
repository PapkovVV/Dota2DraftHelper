using Dota2DraftHelper.Models;
using HtmlAgilityPack;
using System.Windows;
using System.Xml;

namespace Dota2DraftHelper.Services;

public static class Parsing
{
    private static HtmlWeb htmlWeb = new HtmlWeb();
    private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(5);

    public static async Task<List<Hero>> ParseHeroesInfoAsync() // Get the list of heroes (OP)
    {
        var heroes = new List<Hero>();

        var doc = await htmlWeb.LoadFromWebAsync("https://www.dotabuff.com/heroes").ConfigureAwait(false);

        var heroRows = doc.DocumentNode.SelectNodes("//tr[@class='tw-border-b tw-transition-colors hover:tw-bg-muted/50 data-[state=selected]:tw-bg-muted']");

        if (heroRows != null)
        {
            foreach (var heroRow in heroRows)
            {
                var heroInfo = heroRow.SelectSingleNode(".//div[@class='tw-flex tw-flex-col tw-gap-0']");
                var heroImage = heroRow.SelectSingleNode(".//img[@class='tw-w-auto tw-h-6 sm:tw-h-8 tw-shrink-0 " +
                    "tw-rounded-sm tw-shadow-sm tw-shadow-black/20']");

                if (heroInfo != null)
                {
                    Hero newHero = new Hero();

                    var heroName = heroInfo.SelectSingleNode(".//div");
                    var heroFaceit = heroInfo.SelectSingleNode(".//div[@class='tw-text-xs tw-text-secondary']");

                    if (heroName != null)
                    {
                        newHero.Name = heroName.InnerText.Replace("&#x27;", "");
                    }

                    if (heroFaceit != null)
                    {
                        newHero.Faceit = heroFaceit.InnerText.Replace("&#x27;", "");
                    }

                    heroes.Add(newHero);
                }
            }
        }
        else
        {
            MessageBox.Show("Error");
        }

        if (heroes.Count() != 0)
        {
            heroes = heroes.DistinctBy(hero => hero.Name).ToList();
        }

        foreach (var hero in heroes)
        {
            byte[]? imageBytes;
            if (hero.Name.ToLower().Contains("warrunner"))
            {
                imageBytes = await ImageServices.DownloadImageAsync("centaur-warchief");
            }
            else if (hero.Name.ToLower().Equals("io"))
            {
                imageBytes = await ImageServices.DownloadImageAsync("wisp");
            }
            else
            {
                imageBytes = await ImageServices.DownloadImageAsync(hero.Name.ToLower().Replace(" ", "-"));
            }

            hero.ImageData = imageBytes;
        }

        return heroes;
    }

    public static async Task<List<CounterPickInfo>> ParseCounterPicksInfoAsync()
    {
        var heroes = await CacheHeroes.GetHeroesAsync();

        var tasks = heroes.Select(hero => ParseSingleCounterPickInfo(hero));

        var results = await Task.WhenAll(tasks);

        var combinedList = results.SelectMany(list => list).ToList();

        return combinedList;
    }

    private static async Task<List<CounterPickInfo>> ParseSingleCounterPickInfo(Hero hero)
    {
        await semaphore.WaitAsync();

        List<CounterPickInfo> result = new List<CounterPickInfo>();

        try
        {
            string requiredHeroName = hero.Name.ToLower().Replace(" ", "-");
            string heroURL = $"https://www.dotabuff.com/heroes/{requiredHeroName}/counters";

            var doc = await htmlWeb.LoadFromWebAsync(heroURL).ConfigureAwait(false);

            if (doc != null)
            {
                var counterPicksTable = doc.DocumentNode.SelectSingleNode("//table[@class='sortable']");

                if (counterPicksTable != null)
                {
                    var counterPicksInfo = counterPicksTable.SelectNodes("//tr[@data-link-to]");

                    if (counterPicksInfo != null)
                    {
                        foreach (var counterPickInfo in counterPicksInfo)
                        {
                            var name = GetCounterPickName(counterPickInfo);
                            var winRate = GetCounterPickWinRate(counterPickInfo);

                            var enemy = new CounterPickInfo() {PickId = hero.Id,
                                CounterPickId = (await CacheHeroes.GetHeroesAsync()).FirstOrDefault(x => x.Name.Equals(name))!.Id, 
                                WinRate = winRate, WinRateDate = DateTime.Now.Date};

                            result.Add(enemy);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading {ex.Message}");
        }
        finally
        {
            semaphore.Release(); // Освобождаем семафор
        }

        return result;
    }

    private static string GetCounterPickName(HtmlNode counterPickInfo)
    {
        var nameHtml = counterPickInfo!.SelectSingleNode(".//td[2]");

        if (nameHtml != null)
        {
            return nameHtml.InnerText.Replace("&#39;","");
        }

        return "Undefined";
    }

    private static decimal GetCounterPickWinRate(HtmlNode counterPickInfo)
    {
        var winRateHtml = counterPickInfo.SelectSingleNode(".//td[3]");

        if (winRateHtml != null)
        {
            return decimal.Parse(winRateHtml.InnerText.Replace("%", "").Replace(".", ",").Trim());
        }

        return 0.00m;
    }
}
