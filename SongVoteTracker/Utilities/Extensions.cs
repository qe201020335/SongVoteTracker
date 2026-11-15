using SongCore;

namespace SongVoteTracker.Utilities;

internal static class Extensions
{
    public static bool TryGetLowerCaseLevelHash(this BeatmapLevel level, out string hash)
    {
        hash = Collections.GetCustomLevelHash(level.levelID).ToLower();
        return !string.IsNullOrWhiteSpace(hash);
    }
}