using UnityEngine;

public static class SaveSystem
{
    public static void SaveMaxResource(ResourceType type, int value) => PlayerPrefs.SetInt($"Max{type}", value);

    public static int LoadMaxResource(ResourceType type, int defaultValue) => PlayerPrefs.GetInt($"Max{type}", defaultValue);

    public static void SaveDay(int day) => PlayerPrefs.SetInt("CurrentDay", day);
    public static int LoadDay() => PlayerPrefs.GetInt("CurrentDay", 1);
}
