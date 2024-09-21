using Newtonsoft.Json;
using System.IO;

namespace Dota2DraftHelper.Services;

public class JSONServices
{
    private static string jsonFilePath = "Settings.json";
    public static void SaveSettings(bool IsAllHeroesChecked) //Save settings in JSON(OP)
    {
        Settings settings = new Settings
        {
            IsAllHeroes = IsAllHeroesChecked
        };

        string json = JsonConvert.SerializeObject(settings, Formatting.Indented);

        File.WriteAllTextAsync(jsonFilePath, json);
    }
    public static async Task<bool> LoadSettingsAsync() //Load settings from JSON
    {
        if (File.Exists(jsonFilePath))
        {
            string json = await File.ReadAllTextAsync(jsonFilePath);

            Settings settings = JsonConvert.DeserializeObject<Settings>(json)!;

            return settings.IsAllHeroes;
        }

        return false;
    }
}
