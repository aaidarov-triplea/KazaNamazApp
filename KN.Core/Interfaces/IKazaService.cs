using KN.Core.Enums;
using KN.Core.Models;

namespace KN.Core.Interfaces;

public interface IKazaService
{
    Task<KazaSummary> GetSummaryAsync(string userId);
    Task<OperationResult> AddPeriodAsync(string userId, DateOnly start, DateOnly end);
    Task<OperationResult> DeletePeriodAsync(string userId, int periodId);
    Task<OperationResult> AddReadAsync(string userId, PrayerType type, int count);
    Task<OperationResult> DeleteLogAsync(string userId, int logId);
}