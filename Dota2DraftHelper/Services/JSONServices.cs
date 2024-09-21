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

        File.WriteAllText(jsonFilePath, json);
    }
    public static bool LoadSettings() //Load settings from JSON
    {
        if (File.Exists(jsonFilePath))
        {
            string json = File.ReadAllText(jsonFilePath);

            Settings settings = JsonConvert.DeserializeObject<Settings>(json);

            return settings.IsAllHeroes;
        }

        return false;
    }
}
