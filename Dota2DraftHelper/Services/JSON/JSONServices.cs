using Newtonsoft.Json;
using System.IO;

namespace Dota2DraftHelper.Services.JSON;

public class JSONServices
{

    private static string jsonFilePath = "settings.json";
    public static void SaveSettings(bool isFeatureEnabled)
    {

        Settings settings = new Settings
        {
            IsAllHeroes = isFeatureEnabled
        };

        string json = JsonConvert.SerializeObject(settings, Formatting.Indented);

        File.WriteAllText(jsonFilePath, json);
    }

    public static bool LoadSettings()
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
