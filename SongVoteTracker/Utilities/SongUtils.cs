using SongCore;

namespace SongVoteTracker.Utilities;

public class SongUtils
{
    internal static string? GetLowerCaseCustomLevelHash(BeatmapLevel level)
    {
        var hash = Collections.GetCustomLevelHash(level.levelID).ToLower();
        return string.IsNullOrWhiteSpace(hash) ? null : hash;
    }
}