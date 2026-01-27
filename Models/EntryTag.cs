using System.ComponentModel.DataAnnotations.Schema;

namespace JournalApp.Models;

/// <summary>
/// Junction table for the many-to-many relationship between JournalEntry and Tag
/// </summary>
public class EntryTag
{
    public int EntryId { get; set; }
    
    [ForeignKey(nameof(EntryId))]
    public JournalEntry? Entry { get; set; }

    public int TagId { get; set; }
    
    [ForeignKey(nameof(TagId))]
    public Tag? Tag { get; set; }
}
