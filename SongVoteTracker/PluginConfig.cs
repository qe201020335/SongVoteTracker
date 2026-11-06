using System.Runtime.CompilerServices;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]

namespace SongVoteTracker;

internal class PluginConfig
{
    public virtual bool ShowVotes { get; set; } = true;
}