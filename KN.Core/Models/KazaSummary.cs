using KN.Core.Entities;
using KN.Core.Enums;

namespace KN.Core.Models;

public class PrayerSummary
{
    public PrayerType Type { get; set; }
    public int Total { get; set; }
    public int Done { get; set; }
    public int Remaining => Math.Max(0, Total - Done);
}

public class KazaSummary
{
    public int TotalDays { get; set; }
    public List<PrayerSummary> Prayers { get; set; } = new();
    public List<KazaPeriod> Periods { get; set; } = new();
    public List<KazaLog> RecentLogs { get; set; } = new();

    public int TotalPrayers => Prayers.Sum(p => p.Total);
    public int RemainingPrayers => Prayers.Sum(p => p.Remaining);
}