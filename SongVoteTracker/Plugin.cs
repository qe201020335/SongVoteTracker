using System.Linq;
using BeatSaberMarkupLanguage.Settings;
using BeatSaberMarkupLanguage.Util;
using Hive.Versioning;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Loader;
using SiraUtil.Zenject;
using SongVoteTracker.Hooks;
using SongVoteTracker.Menu;
using SongVoteTracker.VoteTracker;
using IPALogger = IPA.Logging.Logger;

namespace SongVoteTracker;

[Plugin(RuntimeOptions.SingleStartInit)]
[NoEnableDisable]
internal class Plugin
{
    internal const string HookId = "com.github.qe201020335.SongVoteTracker";
    private const string BeatSaverVotingId = "BeatSaverVoting";
    private const string DiTailsId = "DiTails";
    private const string SongPlayHistoryId = "SongPlayHistory";

    public static Plugin Instance { get; private set; } = null!;
    internal static IPALogger Log { get; private set; } = null!;
    internal static PluginConfig Config { get; private set; } = null!;

    [Init]
    public Plugin(IPALogger logger, Config config, Zenjector zenjector)
    {
        Instance = this;
        Log = logger;
        Config = config.Generated<PluginConfig>();

        var bsvInstalled = PluginManager.EnabledPlugins.Any(metadata => metadata.Id == BeatSaverVotingId);
        var diTailsInstalled = PluginManager.GetPluginFromId(DiTailsId) != null;
        var sphMeta = PluginManager.GetPluginFromId(SongPlayHistoryId);

        var settingsMenu = new SettingsMenu();
        MainMenuAwaiter.MainMenuInitializing += () =>
            BSMLSettings.Instance.AddSettingsMenu("Vote Tracker", "SongVoteTracker.Menu.settings.bsml", settingsMenu);

        zenjector.UseLogger(logger);
        zenjector.Install(Location.App, container =>
        {
            if (bsvInstalled)
            {
                Log.Info("BeatSaverVoting is installed! Binding BeatSaverVotingTracker.");
                container.BindInterfacesTo<BeatSaverVotingTracker>().AsSingle();
            }
            else
            {
                Log.Info("BeatSaverVoting is NOT installed! Binding InternalVoteTracker.");
                container.BindInterfacesTo<InternalVoteTracker>().AsSingle();
            }
        });

        if (sphMeta == null || sphMeta.HVersion >= new Version(2, 4, 0))
        {
            zenjector.Install(Location.Menu, container =>
            {
                if (diTailsInstalled)
                {
                    container.BindInterfacesTo<DiTailsVoteHook>().AsSingle();
                }

                if (Config.ShowVotes && !bsvInstalled) // let BeatSaverVoting show its own icons
                {
                    container.BindInterfacesTo<VoteImageHook>().AsSingle();
                }
            });
        }
        else
        {
            Log.Notice("SongPlayHistory version with vote tracker is installed, not applying hooks.");
        }

        Log.Info("Plugin initialized");
    }
}