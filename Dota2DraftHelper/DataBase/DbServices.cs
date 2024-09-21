using Dota2DraftHelper.Models;
using Dota2DraftHelper.Services;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace Dota2DraftHelper.DataBase;

public static class DbServices
{
    public static async Task AddHeroesInDBAsync() // Add heroes in DB(OP)
    {
        using (var db = new ApplicationDBContext())
        {
            if (await db.Heroes.AsNoTracking().CountAsync() < 125)
            {
                List<Hero> heroes = await Parsing.ParseHeroesInfoAsync();

                var existingHeroes = await db.Heroes.AsNoTracking()
                                                    .Select(h => new { h.Name, h.Faceit })
                                                    .ToListAsync();

                foreach (var hero in heroes)
                {
                    bool exists = existingHeroes.Any(h => h.Name == hero.Name && h.Faceit == hero.Faceit);

                    if (!exists)
                    {
                        await db.Heroes.AddAsync(hero);
                    }
                }

                await db.SaveChangesAsync();
            }
        }
    }
    public static async Task<List<Hero>> GetHeroesAsync() // Get heroes list(OP)
    {
        using (var db = new ApplicationDBContext())
        {
            return await db.Heroes.AsNoTracking().ToListAsync();
        }
    }
    public static async Task<Hero?> GetHeroAsync(int heroId) // Get hero info(OP)
    {
        using (var db = new ApplicationDBContext())
        {
            try
            {
                return await db.Heroes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == heroId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving hero: {ex.Message}");
                return null;
            }
        }
    }


    public static async Task AddHeroWinRatesInDBAxync()
    {

        using (var db = new ApplicationDBContext())
        {
            var counterPicks = db.CounterPickInfos;

            if (!await counterPicks.AnyAsync(x => x.WinRateDate == DateTime.Now.Date))
            {
                counterPicks.AddRange(await Parsing.ParseCounterPicksInfoAsync());
                await db.SaveChangesAsync();
            }
            else
            {
                if (counterPicks.Where(x => x.WinRateDate == DateTime.Now.Date).Count() < 15500)
                {
                    var existingCounterPicks = counterPicks.Where(x => x.WinRateDate == DateTime.Now.Date).Select(x => x.CounterPickInfoId).ToList();
                    var newCounterPicks = counterPicks.Where(x => !existingCounterPicks.Contains(x.CounterPickInfoId)).ToList();

                    if (newCounterPicks.Any())
                    {
                        await counterPicks.AddRangeAsync(newCounterPicks);
                    }
                    await db.SaveChangesAsync();
                }
            }
        }
    }
    public static async Task<List<CounterPickInfo>> GetWinRatesAsync()
    {
        using (var db = new ApplicationDBContext())
        {
            return await  db.CounterPickInfos.Where(x => x.WinRateDate == DateTime.Now.Date).Include(c => c.PickHero).Include(c => c.CounterPickHero).ToListAsync();
        }
    }

    public static async Task<List<Lane>> GetLanesAsync() // Get lanes list(OP)
    {
        using (var db = new ApplicationDBContext())
        {
            return await db.Lanes.AsNoTracking().ToListAsync();
        }
    }

    public static async Task<List<OwnPick>> GetOwnPicksAsync(uint laneId) // Get own pick heroes(OP)
    {
        using (var db = new ApplicationDBContext())
        {
            return await db.OwnPicks.AsNoTracking().Where(x => x.LaneId == laneId + 1).ToListAsync();
        }
    }

    public static async Task<bool> AddOwnHeroAsync(int heroId, int laneId) // Save own pick in DB(OP)
    {
        using (var db = new ApplicationDBContext())
        {
            bool exist = await db.OwnPicks.AsNoTracking().AnyAsync(p => p.HeroId == heroId && p.LaneId == laneId);

            if (!exist)
            {
                await db.OwnPicks.AddAsync(new OwnPick()
                {
                    HeroId = heroId,
                    LaneId = laneId
                });

                await db.SaveChangesAsync();

                return true;
            }

            return false;
        }
    }

    public static async Task<bool> RemoveOwnHeroAsync(int heroId, int laneId) // Remove hero from own pick in DB(OP)
    {
        using (var db = new ApplicationDBContext())
        {
            OwnPick? ownHero = await db.OwnPicks.FirstOrDefaultAsync(x => x.HeroId == heroId && x.LaneId == laneId);

            if (ownHero != null)
            {
                db.OwnPicks.Remove(ownHero!);

                await db.SaveChangesAsync();

                return true;
            }
        }
        return false;
    }
}
