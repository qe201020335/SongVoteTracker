using BeatSaberMarkupLanguage.Attributes;

namespace SongVoteTracker.Menu;

internal class SettingsMenu
{
    [UIValue("show-votes")]
    public bool ShowVotes
    {
        get => Plugin.Config.ShowVotes;
        set => Plugin.Config.ShowVotes = value;
    }
}