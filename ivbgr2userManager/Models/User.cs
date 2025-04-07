using System.Dynamic;

namespace ivbgr2userManager.Models;

public class User
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? NotificationPreference { get; set; }
    public string? AccountType { get; set; }
    public bool isTermsAccepted { get; set; }
}