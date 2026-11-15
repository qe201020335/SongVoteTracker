# SongVoteTracker

[![release](https://img.shields.io/github/v/release/qe201020335/SongVoteTracker?color=Green&style=for-the-badge)](https://github.com/qe201020335/SongVoteTracker/releases/latest)

Track all your song votes.

> [!NOTE]
> For information on the Vote API, see [For Modders](#for-modders).

## Features
- Track your votes (👍/👎) for all songs
- Sync your votes from DiTails with BeatSaverVoting 
- Visualize your song preferences (👍/👎) (if you don't have BeatSaverVoting installed)

## Requirements
- BSIPA
- BeatSaberMarkupLanguage
- SiraUtil
- SongCore

## For Modders
- To access the vote data, use `Zenject` and inject `IVoteTracker`.
- `IVoteTracker` is installed to `SiraUtil`'s `Location.App`.