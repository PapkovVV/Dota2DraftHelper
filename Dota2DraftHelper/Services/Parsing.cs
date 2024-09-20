using Dota2DraftHelper.Models;
using HtmlAgilityPack;
using System;
using System.Net.Http;
using System.Security.Policy;
using System.Windows;

namespace Dota2DraftHelper.Services;

public static class Parsing
{
    private static HtmlWeb htmlWeb = new HtmlWeb(); 

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

                if (heroInfo != null)
                {
                    Hero newHero = new Hero();

                    var heroName = heroInfo.SelectSingleNode(".//div");
                    var heroFaceit = heroInfo.SelectSingleNode(".//div[@class='tw-text-xs tw-text-secondary']");
                    
                    if (heroName != null)
                    {
                        newHero.Name = heroName.InnerText.Replace("&#x27;","");
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

        return heroes;
    }
}
