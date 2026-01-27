using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JournalApp.Models;

/// <summary>
/// Represents a single journal entry for a specific date
/// Only one entry is allowed per day
/// </summary>
public class JournalEntry
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    [MaxLength(500)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; }

    public int WordCount { get; set; }

    [MaxLength(200)]
    public string? Category { get; set; }

    // Primary mood (required)
    [Required]
    public int PrimaryMoodId { get; set; }
    
    [ForeignKey(nameof(PrimaryMoodId))]
    public Mood? PrimaryMood { get; set; }

    // Secondary moods (optional, up to 2)
    public int? SecondaryMood1Id { get; set; }
    
    [ForeignKey(nameof(SecondaryMood1Id))]
    public Mood? SecondaryMood1 { get; set; }

    public int? SecondaryMood2Id { get; set; }
    
    [ForeignKey(nameof(SecondaryMood2Id))]
    public Mood? SecondaryMood2 { get; set; }

    // Many-to-many relationship with Tags
    public ICollection<EntryTag> EntryTags { get; set; } = new List<EntryTag>();
}
