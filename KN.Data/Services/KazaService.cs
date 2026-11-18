using KN.Core.Entities;
using KN.Core.Enums;
using KN.Core.Interfaces;
using KN.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace KN.Data.Services;

public class KazaService(AppDbContext db) : IKazaService
{
    private readonly AppDbContext _db = db;

    public async Task<KazaSummary> GetSummaryAsync(string userId)
    {
        var periods = await _db.KazaPeriods.AsNoTracking()
            .Where(p => p.UserId == userId)
            .OrderBy(p => p.StartDate)
            .ToListAsync();

        var doneByType = await _db.KazaLogs.AsNoTracking()
            .Where(l => l.UserId == userId)
            .GroupBy(l => l.PrayerType)
            .Select(g => new { Type = g.Key, Sum = g.Sum(x => x.Count) })
            .ToDictionaryAsync(x => x.Type, x => x.Sum);

        var recent = await _db.KazaLogs.AsNoTracking()
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.CreatedAt)
            .Take(10)
            .ToListAsync();

        var totalDays = periods.Sum(p => p.Days);

        return new KazaSummary
        {
            TotalDays = totalDays,
            Periods = periods,
            RecentLogs = recent,
            Prayers = Enum.GetValues<PrayerType>().Select(t => new PrayerSummary
            {
                Type = t,
                Total = totalDays,
                Done = doneByType.GetValueOrDefault(t)
            }).ToList()
        };
    }

    public async Task<OperationResult> AddPeriodAsync(string userId, DateOnly start, DateOnly end)
    {
        if (start > end)
            return OperationResult.Fail("Дата начала не может быть позже даты окончания.");

        if (end > DateOnly.FromDateTime(DateTime.Today))
            return OperationResult.Fail("Дата оканчания не может быть будущем");

        var overlaps = await _db.KazaPeriods
            .AnyAsync(p => p.UserId == userId && p.StartDate <= end && p.EndDate >= start);
        if (overlaps)
            return OperationResult.Fail("Этот период пересекается с уже добавденным");

        _db.KazaPeriods.Add(new KazaPeriod { UserId = userId, StartDate = start, EndDate = end });
        await _db.SaveChangesAsync();
        return OperationResult.Success();
    }

    public async Task<OperationResult> DeletePeriodAsync(string userId, int periodId)
    {
        var period = await _db.KazaPeriods.FirstOrDefaultAsync(p => p.Id == periodId && p.UserId == userId);
        if (period is null) return OperationResult.Fail("Период не найден.");

        _db.KazaPeriods.Remove(period);
        await _db.SaveChangesAsync();
        return OperationResult.Success();
    }

    public async Task<OperationResult> AddReadAsync(string userId, PrayerType type, int count)
    {
        if (!Enum.IsDefined(type))
            return OperationResult.Fail("Неизвестный вид намаза.");
        if (count < 1)
            return OperationResult.Fail("Количество должно быть не меньше 1.");

        var periods = await _db.KazaPeriods.AsNoTracking().Where(p => p.UserId == userId).ToListAsync();
        var total = periods.Sum(p => p.Days);
        var done = await _db.KazaLogs
            .Where(l => l.UserId == userId && l.PrayerType == type)
            .SumAsync(l => l.Count);
        
        var remaining = total - done;
        if (count > remaining)
            return OperationResult.Fail($"{type.ToDisplayName()}: осталось только {Math.Max(0, remaining)}.");
        
        _db.KazaLogs.Add(new KazaLog { UserId = userId, PrayerType = type, Count = count });
        await _db.SaveChangesAsync();
        return OperationResult.Success();
    }

    public async Task<OperationResult> DeleteLogAsync(string userId, int logId)
    {
        var log = await _db.KazaLogs.FirstOrDefaultAsync(l => l.Id == logId && l.UserId == userId);
        if (log is null) return OperationResult.Fail("Запись не найден.");
        
        _db.KazaLogs.Remove(log);
        await _db.SaveChangesAsync();
        return OperationResult.Success();
    }
}

