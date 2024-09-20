using Dota2DraftHelper.DataBase;
using Dota2DraftHelper.Models;

namespace Dota2DraftHelper.Services
{
    public class CacheLanes
    {
        private static List<Lane> _lanes = null!;
        public static async Task<List<Lane>> GetLanesAsync()
        {
            if (_lanes == null)
            {
                using (var db = new ApplicationDBContext())
                {
                    _lanes = await DbServices.GetLanesAsync();
                }
            }

            return _lanes;
        }
        public static void ClearLanesCache()
        {
            _lanes = null;
        }
    }
}
