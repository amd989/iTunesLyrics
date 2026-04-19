# iTunesLyrics

A small Windows utility that fills in missing lyrics for the tracks in your iTunes library. Select songs (or let it scan the whole library), click a button, and it fetches lyrics and writes them back to the track's metadata so they travel with the file.

Originally a fork of the discontinued [iLyrics](https://code.google.com/p/ilyrics/) that kept the LyricWiki integration alive after the upstream project dropped it. Now that LyricWiki itself is gone, the current version pulls lyrics from the [Genius](https://genius.com/) API instead.

## Requirements

- Windows with **iTunes** installed (the app talks to iTunes through COM, so a working local install is required — it won't run without one).
- .NET Framework **4.7.2**.
- A free **Genius API access token** — create one at <https://genius.com/api-clients> and paste it into **Settings → Genius API Token** on first launch.

## Install

**[ClickOnce Installer (auto-updates)](http://alejandro.md/publish/iTunesLyrics/)**

The ClickOnce channel is the recommended install — it checks for updates in the background and keeps you on the latest release. A legacy NSIS installer script lives in `iTuneslyrics/InstallerScript/` but is no longer part of the release pipeline.

## Using it

1. Launch iTunesLyrics. iTunes will be brought to the foreground.
2. Paste your Genius token under **Settings** (one-time).
3. Either:
   - Select tracks in iTunes and hit the process button, or
   - Check **Fix** to scan the whole library for lyrics containing replacement characters (`�`) and re-fetch them.
4. Pick a mode:
   - **Automatic** — results stream into a grid as each track is looked up (Found / Not Found / Skipped).
   - **Manual** — review each match before accepting it.
5. Check **Overwrite** if you want existing lyrics replaced instead of skipped.

## Building from source

```
nuget restore iTuneslyrics.sln
msbuild iTuneslyrics.sln /p:Configuration=Debug
```

The project targets **x86** in Debug because the iTunes COM interop (`iTunesLib`) is 32-bit only. Open `iTuneslyrics.sln` in Visual Studio 2017 or later.

## Credits

All credit for the original utility goes to the authors of [iLyrics](https://code.google.com/p/ilyrics/). This fork exists only to keep it working against current lyrics sources.

## Feedback

Bugs, requests, or questions: please open an issue on the [GitHub issues page](https://github.com/amd989/iTunesLyrics/issues).

## Changelog

### 1.3.0.0 — 2026
- Migrated from LyricWiki to the Genius API (LyricWiki was discontinued).
- Added a Settings menu for storing your Genius API token in `%APPDATA%\iTunesLyrics\config.txt` instead of hardcoding it.

### 1.2.1.5 — 2015-04-26
- Added HtmlAgilityPack as a reference.
- Changed publish location.

### 1.2.1.4 — 2014-10-31
- Stripped the scripted ad LyricWiki was injecting into lyrics.

### 1.2.1 — 2013-08-31
- Project uploaded to GitHub.

### 1.2 — 2012-07-15
- LyricWiki support restored after the upstream API change.

### Original changelog

**1.1.1 — 2008-09-19**
- 1.1.1.2 fixed crashes while processing deleted files.
- 1.1.1.1 blank artist/name tags are skipped in batch mode.
- 1.1.1.0 ability to overwrite lyrics in automatic mode.
- 1.1.0.1 UTF-8 character support (thanks to davidreis).

**1.1 — 2007-03-07**
- A few bug fixes.
- Update now skips songs that already have lyrics in iTunes.

**1.0 — 2007-02-08**
- Initial release.
