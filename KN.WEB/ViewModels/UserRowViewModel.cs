namespace KN.WEB.ViewModels;

public class UserRowViewModel
{
    public string Id { get; set; } = "";
    public string Email { get; set; } = "";
    public string? FullName { get; set; }
    public DateTime CreatedAt { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
    public bool IsAdmin { get; set; }
    public bool IsCurrent { get; set; }
}