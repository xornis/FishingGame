using UnityEngine;

public static class SaveSystem
{
    public static void SaveMaxResource(ResourceType type, int value) => PlayerPrefs.SetInt($"Max{type}Resource", value);
    public static int LoadMaxResource(ResourceType type, int value) => PlayerPrefs.GetInt($"Max{type}Resource", value);

    public static void SaveCurrentResource(ResourceType type, int value) => PlayerPrefs.SetInt($"Current{type}Resource", value);
    public static int LoadCurrentResource(ResourceType type, int value) => PlayerPrefs.GetInt($"Current{type}Resource", value);

    public static void SaveMaxStat(StatType type, float value) => PlayerPrefs.SetFloat($"Max{type}Stat", value);
    public static float LoadMaxStat(StatType type, float value) => PlayerPrefs.GetFloat($"Max{type}Stat", value);

    public static void SaveUpgradeLevel(string id, int level) => PlayerPrefs.SetInt($"Upgrade{id}", level);
    public static int LoadUpgradeLevel(string id) => PlayerPrefs.GetInt($"Upgrade{id}", 0);

    public static void SaveDay(int day) => PlayerPrefs.SetInt("CurrentDay", day);
    public static int LoadDay() => PlayerPrefs.GetInt("CurrentDay", 1);
}
