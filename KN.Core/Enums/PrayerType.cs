namespace KN.Core.Enums;

public enum PrayerType
{
    Farj = 1, // Багымдат
    Dhuhr = 2, // Пешим
    Asr = 3,   // Аср
    Maghrib = 4, // Шам
    Isha = 5,  // Куптан
    Witr = 6  // Витр
}

public static class PrayerTypeExtensions
{
    public static string ToDisplayName(this PrayerType type) => type switch
    {
        PrayerType.Farj => "Багымдат",
        PrayerType.Dhuhr => "Пешим",
        PrayerType.Asr => "Аср",
        PrayerType.Maghrib => "Шам",
        PrayerType.Isha => "Куптан",
        PrayerType.Witr => "Витр",
        _ => type.ToString()
    };
}