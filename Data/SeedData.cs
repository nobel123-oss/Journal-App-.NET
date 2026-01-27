using JournalApp.Models;

namespace JournalApp.Data;

/// <summary>
/// Seeds initial data into the database
/// </summary>
public static class SeedData
{
    /// <summary>
    /// Seeds moods with their categories and emojis
    /// </summary>
    public static List<Mood> GetMoods()
    {
        return new List<Mood>
        {
            // Positive Moods
            new Mood { Name = "Happy", Category = "Positive", Emoji = "😊" },
            new Mood { Name = "Excited", Category = "Positive", Emoji = "🤩" },
            new Mood { Name = "Relaxed", Category = "Positive", Emoji = "😌" },
            new Mood { Name = "Grateful", Category = "Positive", Emoji = "🙏" },
            new Mood { Name = "Confident", Category = "Positive", Emoji = "😎" },

            // Neutral Moods
            new Mood { Name = "Calm", Category = "Neutral", Emoji = "😐" },
            new Mood { Name = "Thoughtful", Category = "Neutral", Emoji = "🤔" },
            new Mood { Name = "Curious", Category = "Neutral", Emoji = "🧐" },
            new Mood { Name = "Nostalgic", Category = "Neutral", Emoji = "🥲" },
            new Mood { Name = "Bored", Category = "Neutral", Emoji = "😑" },

            // Negative Moods
            new Mood { Name = "Sad", Category = "Negative", Emoji = "😢" },
            new Mood { Name = "Angry", Category = "Negative", Emoji = "😠" },
            new Mood { Name = "Stressed", Category = "Negative", Emoji = "😰" },
            new Mood { Name = "Lonely", Category = "Negative", Emoji = "😔" },
            new Mood { Name = "Anxious", Category = "Negative", Emoji = "😟" }
        };
    }

    /// <summary>
    /// Seeds pre-built tags
    /// </summary>
    public static List<Tag> GetTags()
    {
        var tagNames = new[]
        {
            "Work", "Career", "Studies", "Family", "Friends", "Relationships",
            "Health", "Fitness", "Personal Growth", "Self-care", "Hobbies", "Travel",
            "Nature", "Finance", "Spirituality", "Birthday", "Holiday", "Vacation",
            "Celebration", "Exercise", "Reading", "Writing", "Cooking", "Meditation",
            "Yoga", "Music", "Shopping", "Parenting", "Projects", "Planning", "Reflection"
        };

        return tagNames.Select(name => new Tag
        {
            Name = name,
            IsPrebuilt = true
        }).ToList();
    }
}
