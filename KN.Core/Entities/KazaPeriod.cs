namespace KN.Core.Entities;

public class KazaPeriod
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int Days => EndDate.DayNumber - StartDate.DayNumber + 1;
}