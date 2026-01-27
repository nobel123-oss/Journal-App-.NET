using System.ComponentModel.DataAnnotations;

namespace JournalApp.Models;

/// <summary>
/// Represents a mood that can be associated with journal entries
/// Categorized as Positive, Neutral, or Negative
/// </summary>
public class Mood
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Category { get; set; } = string.Empty; // Positive, Neutral, Negative

    [MaxLength(10)]
    public string? Emoji { get; set; }
}
