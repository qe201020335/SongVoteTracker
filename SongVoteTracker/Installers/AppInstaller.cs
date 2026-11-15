using SongVoteTracker.VoteTracker;
using Zenject;

namespace SongVoteTracker.Installers;

internal class AppInstaller : Installer<AppInstaller>
{
    public override void InstallBindings()
    {
        if (Plugin.Instance.BeatSaverVotingInstalled)
        {
            Plugin.Log.Info("BeatSaverVoting is installed! Binding BeatSaverVotingTracker.");
            Container.BindInterfacesTo<BeatSaverVotingTracker>().AsSingle();
        }
        else
        {
            Plugin.Log.Info("BeatSaverVoting is NOT installed! Binding InternalVoteTracker.");
            Container.BindInterfacesTo<InternalVoteTracker>().AsSingle();
        }
    }
}