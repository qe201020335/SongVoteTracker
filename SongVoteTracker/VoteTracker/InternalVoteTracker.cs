using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using IPA.Logging;
using IPA.Utilities;
using JetBrains.Annotations;
using Newtonsoft.Json;
using SongVoteTracker.Model;
using SongVoteTracker.Utilities;
using Zenject;

namespace SongVoteTracker.VoteTracker;

internal class InternalVoteTracker : IVoteTracker, IDisposable
{
    private static readonly string VoteFile = Path.Combine(UnityGame.UserDataPath, "votedSongs.json");

    [Inject]
    private readonly Logger _logger = null!;

    private readonly Dictionary<string, UserVote> _votes = new();

    private bool _readonly = true;

    [Inject]
    [UsedImplicitly]
    private void Init()
    {
        _logger.Info("Loading votes.");

        _votes.Clear();

        if (!File.Exists(VoteFile))
        {
            _logger.Debug("BeatSaverVoting votedSongs.json doesn't exist.");
            _readonly = false;
            return;
        }

        try
        {
            var text = File.ReadAllText(VoteFile, Encoding.UTF8);
            var votes = JsonConvert.DeserializeObject<Dictionary<string, UserVote>>(text);
            if (votes == null || votes.Count == 0)
            {
                _logger.Warn("votedSongs.json is empty or invalid. Entering readonly mode.");
                _readonly = true;
                return;
            }

            foreach (var userVote in votes)
            {
                _votes[userVote.Key] = userVote.Value;
            }

            _logger.Info("votedSongs.json Loaded");
        }
        catch (Exception ex) // IOException, JsonException
        {
            _readonly = true;
            _logger.Error("Failed to load votedSongs.json. Entering readonly mode.");
            _logger.Error(ex);
            return;
        }

        try
        {
            // backup in case something bad happened.
            File.Copy(VoteFile, VoteFile + ".sph.bak", true);
            _readonly = false;
        }
        catch (Exception e)
        {
            _readonly = true;
            _logger.Error("Failed to backup votedSongs.json. Entering readonly mode.");
            _logger.Error(e);
        }
    }

    private void SaveVotes()
    {
        if (_readonly)
        {
            _logger.Debug("Votes not saved, read only");
            return;
        }

        try
        {
            lock (_votes)
            {
                if (_votes.Count > 0)
                {
                    File.WriteAllText(VoteFile, JsonConvert.SerializeObject(_votes), Encoding.UTF8);
                }
            }
        }
        catch (Exception e)
        {
            _logger.Error($"Failed to save votedSongs.json: {e.Message}");
            _logger.Error(e);
        }
    }

    public void Dispose()
    {
        SaveVotes();
    }

    public bool TryGetVote(BeatmapLevel level, out VoteType voteType)
    {
        voteType = VoteType.Downvote;
        try
        {
            if (level.TryGetLowerCaseLevelHash(out var hash) && _votes.TryGetValue(hash, out var vote))
            {
                voteType = vote.VoteType;
                return true;
            }
        }
        catch (Exception e)
        {
            _logger.Warn($"Failed to get vote: {e.Message}");
            _logger.Warn(e);
        }

        return false;
    }

    public void Vote(BeatmapLevel level, VoteType voteType)
    {
        if (!level.TryGetLowerCaseLevelHash(out var hash))
        {
            return;
        }

        lock (_votes)
        {
            if (_votes.TryGetValue(hash, out var existing) && existing.VoteType == voteType)
            {
                return;
            }

            _votes[hash] = new UserVote
            {
                Hash = hash,
                VoteType = voteType
            };
        }

        // don't block the main thread on writing to disk
        Task.Run(SaveVotes);

        _logger.Info($"Voted {voteType} to {level.levelID}");
    }
}