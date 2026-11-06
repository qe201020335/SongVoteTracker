using JetBrains.Annotations;
using SongVoteTracker.Model;

namespace SongVoteTracker.VoteTracker;

[PublicAPI]
public interface IVoteTracker
{
    public bool TryGetVote(BeatmapLevel level, out VoteType voteType);

    public void Vote(BeatmapLevel level, VoteType voteType);
}