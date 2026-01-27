using Microsoft.EntityFrameworkCore;
using JournalApp.Models;

namespace JournalApp.Data;

/// <summary>
/// Database context for the Journal application
/// </summary>
public class JournalDbContext : DbContext
{
    public JournalDbContext(DbContextOptions<JournalDbContext> options)
        : base(options)
    {
    }

    // DbSets for all entities
    public DbSet<JournalEntry> JournalEntries { get; set; }
    public DbSet<Mood> Moods { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<EntryTag> EntryTags { get; set; }
    public DbSet<AppSettings> AppSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure JournalEntry
        modelBuilder.Entity<JournalEntry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.Date).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.WordCount).IsRequired();
            entity.Property(e => e.Category).HasMaxLength(200);
            
            // Unique constraint: only one entry per date
            entity.HasIndex(e => e.Date).IsUnique();

            // Relationships
            entity.HasOne(e => e.PrimaryMood)
                  .WithMany()
                  .HasForeignKey(e => e.PrimaryMoodId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.SecondaryMood1)
                  .WithMany()
                  .HasForeignKey(e => e.SecondaryMood1Id)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.SecondaryMood2)
                  .WithMany()
                  .HasForeignKey(e => e.SecondaryMood2Id)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Mood
        modelBuilder.Entity<Mood>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Emoji).HasMaxLength(10);
        });

        // Configure Tag
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.IsPrebuilt).IsRequired();
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Configure EntryTag (many-to-many relationship)
        modelBuilder.Entity<EntryTag>(entity =>
        {
            entity.HasKey(e => new { e.EntryId, e.TagId });

            entity.HasOne(et => et.Entry)
                  .WithMany(e => e.EntryTags)
                  .HasForeignKey(et => et.EntryId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(et => et.Tag)
                  .WithMany(t => t.EntryTags)
                  .HasForeignKey(et => et.TagId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure AppSettings
        modelBuilder.Entity<AppSettings>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PasswordHash).HasMaxLength(500);
            entity.Property(e => e.Theme).IsRequired().HasMaxLength(50);
            entity.Property(e => e.IsLocked).IsRequired();
        });
    }
}
