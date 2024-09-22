using System.Net.Http;
using System.Windows;

namespace Dota2DraftHelper.Services;

public static class ImageServices
{
    public static async Task<byte[]> DownloadImageAsync(string heroName)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                var imageBytes = await client.GetByteArrayAsync($"https://www.dotafire.com/images/hero/icon/{heroName}.png");
                return imageBytes;
            }
            catch (Exception ex)
            {
                MessageBox.Show(heroName +" not found!");
                return new byte[0];
            }
        }
    }
}
