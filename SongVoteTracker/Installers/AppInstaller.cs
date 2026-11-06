using SongVoteTracker.VoteTracker;
using Zenject;

namespace SongVoteTracker.Installers;

internal class AppInstaller : Installer<AppInstaller>
{
    private readonly PluginConfig _config;

    public AppInstaller(PluginConfig config) => _config = config;

    public override void InstallBindings()
    {
        Container.BindInstance(_config);

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