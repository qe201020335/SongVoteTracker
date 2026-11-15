using IPA.Logging;
using SongVoteTracker.Hooks;
using SongVoteTracker.VoteTracker;
using Zenject;

namespace SongVoteTracker.Installers;

internal class MenuInstaller : Installer<MenuInstaller>
{
    [Inject]
    private readonly Logger _logger = null!;

    public override void InstallBindings()
    {
        _logger.Debug("Binding InMenuVoteTrackingHelper");
        Container.BindInterfacesAndSelfTo<InMenuVoteTrackingHelper>().AsSingle();

        if (!Plugin.Instance.ShouldApplyHooks) return;

        if (Plugin.Instance.DiTailsInstalled)
        {
            Container.BindInterfacesTo<DiTailsVoteHook>().AsSingle();
        }

        if (!Plugin.Instance.BeatSaverVotingInstalled) // let BeatSaverVoting show its own icons
        {
            Container.BindInterfacesTo<VoteImageHook>().AsSingle();
        }
    }
}