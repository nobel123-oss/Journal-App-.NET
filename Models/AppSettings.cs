using System.ComponentModel.DataAnnotations;

namespace JournalApp.Models;

/// <summary>
/// Stores application-wide settings including security and theme preferences
/// </summary>
public class AppSettings
{
    [Key]
    public int Id { get; set; }

    [MaxLength(500)]
    public string? PasswordHash { get; set; }

    [Required]
    [MaxLength(50)]
    public string Theme { get; set; } = "Light"; // Light or Dark

    public bool IsLocked { get; set; }
}
