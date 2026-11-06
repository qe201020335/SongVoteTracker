using IPA.Logging;
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
    }
}