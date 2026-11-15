using System;
using System.IO;
using System.Reflection;
using MonoMod.RuntimeDetour;
using SongVoteTracker.Model;
using SongVoteTracker.VoteTracker;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static UnityEngine.Object;
using Logger = IPA.Logging.Logger;

namespace SongVoteTracker.Hooks;

internal class VoteImageHook : IInitializable, IDisposable
{
    private static readonly Color UpColor = new(0.455f, 0.824f, 0.455f, 0.8f);
    private static readonly Color DownColor = new(0.824f, 0.498f, 0.455f, 0.8f);

    private static Sprite? _thumbsUp;
    private static Sprite? _thumbsDown;

    private readonly Logger _logger;

    [Inject]
    private readonly IVoteTracker _voteTracker = null!;

    private Hook? _hook;

    public VoteImageHook(Logger logger)
    {
        _logger = logger;
        _thumbsUp ??= LoadSpriteFromResource("SongVoteTracker.Assets.ThumbsUp.png");
        _thumbsDown ??= LoadSpriteFromResource("SongVoteTracker.Assets.ThumbsDown.png");
    }

    public void Initialize()
    {
        if (_thumbsUp == null || _thumbsDown == null)
        {
            _logger.Warn("Vote icons failed to load, not hooking");
            return;
        }

        try
        {
            var source = typeof(LevelListTableCell).GetMethod(nameof(LevelListTableCell.SetDataFromLevelAsync),
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!;
            _hook = new Hook(source, Detour, new DetourConfig(Plugin.HookId));
        }
        catch (Exception e)
        {
            _logger.Critical("Failed to hook LevelListTableCell.SetDataFromLevelAsync");
            _logger.Critical(e);
        }
    }

    public void Dispose()
    {
        _hook?.Dispose();
    }

    private void Detour(Action<LevelListTableCell, BeatmapLevel?, bool, bool, bool, bool> original,
        LevelListTableCell self, BeatmapLevel? beatmapLevel, bool isFavorite, bool isPromoted, bool isUpdated,
        bool interactable)
    {
        original(self, beatmapLevel, isFavorite, isPromoted, isUpdated, interactable);

        if (beatmapLevel == null) return;

        var bpmText = self._songBpmText;
        if (bpmText != null)
        {
            bpmText.text = beatmapLevel.beatsPerMinute.ToString("0");
        }

        var voteIcon = self.transform.Find("Vote")?.GetComponent<Image>();
        if (voteIcon == null)
        {
            voteIcon = Instantiate(self._favoritesBadgeImage, self.transform);
            voteIcon.name = "Vote";
            voteIcon.rectTransform.sizeDelta = new Vector2(2.5f, 2.5f);
        }

        if (!isFavorite && _voteTracker.TryGetVote(beatmapLevel, out var vote))
        {
            if (vote == VoteType.Upvote)
            {
                voteIcon.sprite = _thumbsUp;
                voteIcon.color = UpColor;
            }
            else
            {
                voteIcon.sprite = _thumbsDown;
                voteIcon.color = DownColor;
            }

            voteIcon.enabled = true;
        }
        else
        {
            voteIcon.enabled = false;
        }
    }

    private Sprite? LoadSpriteFromResource(string resourcePath)
    {
        try
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
            using var ms = new MemoryStream();
            stream!.CopyTo(ms);

            var texture = new Texture2D(2, 2);
            texture.LoadImage(ms.ToArray());

            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
        }
        catch (Exception ex)
        {
            _logger.Error($"Error while loading resource {resourcePath}");
            _logger.Error(ex);
            return null;
        }
    }
}
