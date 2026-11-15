using IPA.Logging;
using SongVoteTracker.Hooks;
using SongVoteTracker.VoteTracker;
using Zenject;

namespace SongVoteTracker.Installers;

internal class MenuInstaller : Installer<MenuInstaller>
{
    public override void InstallBindings()
    {
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