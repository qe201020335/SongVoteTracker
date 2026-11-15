using System;
using System.Reflection;
using System.Threading.Tasks;
using DiTails.UI;
using IPA.Logging;
using MonoMod.RuntimeDetour;
using SongVoteTracker.Model;
using SongVoteTracker.VoteTracker;
using Zenject;

namespace SongVoteTracker.Hooks;

internal class DiTailsVoteHook : IInitializable, IDisposable
{
    [Inject]
    private readonly Logger _logger = null!;

    [Inject]
    private readonly InMenuVoteTrackingHelper _voteHelper = null!;

    private Hook? _hook;

    public void Initialize()
    {
        try
        {
            var source = typeof(DetailViewHost).GetMethod(nameof(DetailViewHost.Vote),
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (source == null)
            {
                _logger.Warn("DiTails' Vote method not found, not hooking");
                return;
            }

            _hook = new Hook(source, Detour, new DetourConfig(Plugin.HookId));
        }
        catch (Exception e)
        {
            _logger.Critical("Failed to hook DiTails' Vote method");
            _logger.Critical(e);
        }
    }

    public void Dispose()
    {
        _hook?.Dispose();
    }

    private Task Detour(Func<DetailViewHost, bool, Task> original, DetailViewHost self, bool upvote)
    {
        var beatmap = self._activeBeatmap;
        if (beatmap != null)
        {
            var vote = upvote ? VoteType.Upvote : VoteType.Downvote;
            _logger.Debug($"DiTails voted {vote} to {beatmap.levelID}");
            _voteHelper.Vote(beatmap, vote);
        }

        return original(self, upvote);
    }
}