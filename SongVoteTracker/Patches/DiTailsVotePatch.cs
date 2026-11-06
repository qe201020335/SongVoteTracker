using System.Reflection;
using HarmonyLib;
using SongVoteTracker.Model;
using SongVoteTracker.VoteTracker;

namespace SongVoteTracker.Patches;

[HarmonyPatch]
internal class DiTailsVotePatch
{
    private static MethodBase? DiTailsVoteMethod;

    static DiTailsVotePatch()
    {
        DiTailsVoteMethod = Plugin.Instance.DiTailsMetadata?.Assembly
            .GetType("DiTails.UI.DetailViewHost")
            ?.GetMethod("Vote", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        Plugin.DebugLog($"DiTails vote method {(DiTailsVoteMethod == null ? "not found" : "found")}");
    }

    [HarmonyTargetMethod]
    private static MethodBase CalculateMethod() => DiTailsVoteMethod!;

    [HarmonyPrepare]
    private static bool Prepare() => DiTailsVoteMethod != null;

    [HarmonyPrefix]
    public static void Prefix(bool upvote, BeatmapLevel? ____activeBeatmap) //TODO: update when DiTails Update
    {
        if (____activeBeatmap == null) return;
        var vote = upvote ? VoteType.Upvote : VoteType.Downvote;
        Plugin.Log.Debug($"DiTails voted {vote} to {____activeBeatmap.levelID}");
        if (InMenuVoteTrackingHelper.Instance == null)
        {
            Plugin.Log.Warn("InMenuVoteTrackingHelper is null");
            return;
        }

        InMenuVoteTrackingHelper.Instance.Vote(____activeBeatmap, vote);
    }
}