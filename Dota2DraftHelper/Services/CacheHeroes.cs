using Dota2DraftHelper.DataBase;
using Dota2DraftHelper.Models;

namespace Dota2DraftHelper.Services;

public class CacheHeroes
{
    private static List<Hero> _heroes = null!;
    public static async Task<List<Hero>> GetHeroesAsync()
    {
        if (_heroes == null)
        {
            using (var db = new ApplicationDBContext())
            {
                _heroes = await DbServices.GetHeroesAsync();
            }
        }

        return _heroes;
    }
    public static void ClearHeroesCache()
    {
        _heroes = null;
    }
}
