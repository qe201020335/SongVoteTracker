using BeatSaberMarkupLanguage.Settings;
using BeatSaberMarkupLanguage.Util;
using IPA;
using IPA.Config.Stores;
using IPALogger = IPA.Logging.Logger;
using IPA.Config;
using SongVoteTracker.Menu;

namespace SongVoteTracker;

[Plugin(RuntimeOptions.SingleStartInit)]
internal class Plugin
{
    internal static IPALogger Log { get; private set; } = null!;
    internal static PluginConfig Config { get; private set; } = null!;
    
    private readonly SettingsMenu _settingsMenu;

    [Init]
    public Plugin(IPALogger logger, Config config)
    {
        Log = logger;
        Config = config.Generated<PluginConfig>();
        
        _settingsMenu = new SettingsMenu();
        MainMenuAwaiter.MainMenuInitializing += OnMenuLoad;

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
}