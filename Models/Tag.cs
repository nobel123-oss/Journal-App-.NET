using System.ComponentModel.DataAnnotations;

namespace JournalApp.Models;

/// <summary>
/// Represents a tag that can be applied to journal entries
/// Can be pre-built or user-defined
/// </summary>
public class Tag
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public bool IsPrebuilt { get; set; }

    // Many-to-many relationship with JournalEntry
    public ICollection<EntryTag> EntryTags { get; set; } = new List<EntryTag>();
}
