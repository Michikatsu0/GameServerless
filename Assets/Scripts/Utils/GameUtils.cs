using UnityEngine;

internal static class GameUtils
{
    public const string BULLET_LOW_LAYER_NAME = "BulletLow";
    public const string BULLET_MID_LAYER_NAME = "BulletMid";
    public const string BULLET_HARD_LAYER_NAME = "BulletHard";
    public const string KILLVOLUME_LAYER_NAME = "KillVolume";

    public const int BULLET_LOW_PWR = 1;
    public const int BULLET_MID_PWR = 2;
    public const int BULLET_HARD_PWR = 3;

    public const float SCREEN_WIDTH_PERCENT = 0.6F;

    internal static Vector2 GetScreenDimensions() =>
        Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

    internal static float GetUseableScreenWidth(this Vector2 screenDimensions, float percent = 1F) =>
        screenDimensions.x * (System.Math.Clamp(percent, 0F, 1F));

    internal static int GetClampedValue(int index, int max) => System.Math.Clamp(index, 0, max);

    /// <summary>
    /// Converts a seconds amount into a "mm:ss" formatted string (cuts milliseconds)
    /// </summary>
    /// <param name="seconds">The time in seconds to convert</param>
    /// <returns></returns>
    internal static string ToTimeFormatString(this float seconds) => System.TimeSpan.FromSeconds(seconds).ToString(@"mm\:ss");
}