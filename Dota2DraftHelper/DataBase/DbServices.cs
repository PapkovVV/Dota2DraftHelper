using Dota2DraftHelper.Models;
using Dota2DraftHelper.Services;

namespace Dota2DraftHelper.DataBase;

public static class DbServices
{
    public static void AddHeroes() // Add heroes in DB
    {
        using (var db = new ApplicationDBContext())
        {
            if (db.Heroes.Count() < 125)
            {
                List<Hero> heroes = Parsing.ParseHeroesInfo().ToList();

                foreach (var hero in heroes)
                {
                    bool exists = db.Heroes.Any(h => h.Name == hero.Name && h.Faceit == hero.Faceit);

                    if (!exists)
                    {
                        db.Heroes.Add(hero);
                    }
                }

                db.SaveChanges();
            }
        }
    }

    public static void AddLanes() // Add lanes in Db
    {

        using (var db = new ApplicationDBContext())
        {
            if (db.Lanes.Count() < 5)
            {
                List<Lane> lanes = [new Lane() { Name = "Safe Lane", AlternativeName = "Pos 1" },
                                    new Lane() { Name = "Mid Lane", AlternativeName = "Pos 2" },
                                    new Lane() { Name = "Hard Lane", AlternativeName = "Pos 3" },
                                    new Lane() { Name = "Support", AlternativeName = "Pos 4" },
                                    new Lane() { Name = "Hard Support", AlternativeName = "Pos 5" }];
                db.Lanes.AddRange(lanes);
            }

            db.SaveChanges();
        }
    }

    public static IEnumerable<Hero> GetHeroes() // Get heroes list
    {
        List<Hero> heroes = new List<Hero>();

        using (var db = new ApplicationDBContext())
        {
            if (db.Heroes.Count() > 0)
            {
                heroes.AddRange(db.Heroes.ToList());
            }
        }

        return heroes;
    }

    public static IEnumerable<Lane> GetLanes() // Get lanes list
    {
        List<Lane> lanes = new List<Lane>();

        using (var db = new ApplicationDBContext())
        {
            if (db.Lanes.Count() > 0)
            {
                lanes.AddRange(db.Lanes.ToList());
            }
        }

        return lanes;
    }

    public static bool AddOwnHero(int heroId, int laneId) // Save own pick in DB
    {
        using (var db = new ApplicationDBContext())
        {
            bool exist = db.OwnPicks.Any(p => p.HeroId == heroId && p.LaneId == laneId);

            if (!exist)
            {
                db.OwnPicks.Add(new OwnPick()
                {
                    HeroId = heroId,
                    LaneId = laneId
                });

                db.SaveChanges();

                return true;
            }

            return false;
        }
    }
}
