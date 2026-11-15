using System.Diagnostics;
using System.Linq;
using BeatSaberMarkupLanguage.Settings;
using BeatSaberMarkupLanguage.Util;
using Hive.Versioning;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Loader;
using SiraUtil.Zenject;
using SongVoteTracker.Installers;
using SongVoteTracker.Menu;
using IPALogger = IPA.Logging.Logger;

namespace SongVoteTracker;

[Plugin(RuntimeOptions.SingleStartInit)]
internal class Plugin
{
    internal const string HookId = "com.github.qe201020335.SongVoteTracker";
    private const string BeatSaverVotingId = "BeatSaverVoting";
    private const string DiTailsId = "DiTails";
    private const string SongPlayHistoryId = "SongPlayHistory";

    public static Plugin Instance { get; private set; } = null!;
    internal static IPALogger Log { get; private set; } = null!;
    internal static PluginConfig Config { get; private set; } = null!;

    private readonly SettingsMenu _settingsMenu;

    internal bool ShouldApplyHooks { get; }
    internal bool BeatSaverVotingInstalled { get; }
    internal bool DiTailsInstalled { get; }
    internal PluginMetadata? SongPlayHistoryMetadata { get; }


    [Init]
    public Plugin(IPALogger logger, Config config, Zenjector zenjector)
    {
        Instance = this;
        Log = logger;
        Config = config.Generated<PluginConfig>();

        BeatSaverVotingInstalled = PluginManager.EnabledPlugins.Any(metadata => metadata.Id == BeatSaverVotingId);
        DiTailsInstalled = PluginManager.GetPluginFromId(DiTailsId) != null;
        SongPlayHistoryMetadata = PluginManager.GetPluginFromId(SongPlayHistoryId);

        _settingsMenu = new SettingsMenu();
        MainMenuAwaiter.MainMenuInitializing += OnMenuLoad;

        ShouldApplyHooks = SongPlayHistoryMetadata == null || SongPlayHistoryMetadata.HVersion >= new Version(2, 4, 0);
        if (!ShouldApplyHooks)
        {
            Log.Notice("SongPlayHistory version with vote tracker is installed, not applying hooks.");
        }

        zenjector.UseLogger(logger);
        zenjector.Install<MenuInstaller>(Location.Menu);
        zenjector.Install<AppInstaller>(Location.App);
        
        Log.Info("Plugin initialized");
    }
    
    private void OnMenuLoad()
    {
        BSMLSettings.Instance.AddSettingsMenu("Vote Tracker", "SongVoteTracker.Menu.settings.bsml", _settingsMenu);
    }

    [OnStart]
    public void OnApplicationStart()
    {
        Log.Debug("OnApplicationStart");
    }

    [OnExit]
    public void OnApplicationQuit()
    {
        Log.Debug("OnApplicationQuit");
    }

    [Conditional("DEBUG")]
    internal static void DebugLog(string message)
    {
        Log.Debug(message);
    }
}