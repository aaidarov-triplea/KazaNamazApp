using KN.Core.Enums;

namespace KN.Core.Entities;

public class KazaLog
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    public PrayerType PrayerType { get; set; }
    public int Count { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}