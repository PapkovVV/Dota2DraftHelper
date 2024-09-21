using Dota2DraftHelper.DataBase;
using Dota2DraftHelper.Models;

namespace Dota2DraftHelper.Services;

public class CacheWinRates
{
    private static List<CounterPickInfo> _counterPicks = null!;
    public static async Task<List<CounterPickInfo>> GetCounterPicksAsync()
    {
        if (_counterPicks == null)
        {
            using (var db = new ApplicationDBContext())
            {
                _counterPicks = await DbServices.GetWinRatesAsync();
            }
        }

        return _counterPicks;
    }
}
