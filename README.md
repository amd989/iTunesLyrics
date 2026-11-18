# iTunesLyrics

A small Windows utility that fills in missing lyrics for the tracks in your iTunes library. Select songs in iTunes, click **Get Lyrics**, and it fetches lyrics and writes them back to each track's metadata so they travel with the file.

Originally a fork of the discontinued [iLyrics](https://code.google.com/p/ilyrics/) that kept the LyricWiki integration alive after the upstream project dropped it. Now that LyricWiki itself is gone, the current version pulls lyrics from the [Genius](https://genius.com/) API instead.

## Requirements

- Windows with **iTunes** installed. The app talks to iTunes through COM, so it needs a working local install.
- **.NET 10**. The ClickOnce and portable builds are self-contained, so there is nothing extra to install.
- A free **Genius API access token**. Create a client at <https://genius.com/api-clients> and paste its Client Access Token into **Settings** on first launch.

## Install

**[ClickOnce Installer (auto-updates)](https://l.alejandro.md/ituneslyrics_clickonce)**

ClickOnce is the recommended install: it checks for updates in the background and keeps you on the latest release. A portable ZIP is also attached to each [GitHub release](https://github.com/amd989/iTunesLyrics/releases) if you prefer not to install anything.

## Using it

1. Launch iTunesLyrics. iTunes opens and comes to the foreground.
2. Paste your Genius token under **Settings** (one time). The app sends you there if the token is missing.
3. Select tracks in iTunes. The **Selection** count on the Home page follows along automatically; use **Refresh** if iTunes was closed or restarted.
4. Pick a mode:
   - **Update automatically** (on): lyrics are looked up and written in the background. The **Results** page lists each track as Found, Not Found, Skipped or Error, and you can cancel a run at any time.
   - **Update automatically** (off): review each match with its album art, edit the lyrics if needed, then choose **Update**, **Skip** or **Cancel All**.
5. Turn on **Overwrite existing lyrics** to replace lyrics that are already there instead of skipping those tracks.
6. Click **Get Lyrics**.

**Always on top** in Settings keeps the window above iTunes while you work.

## Building from source

```
dotnet build iTuneslyrics.sln
```

Requires the **.NET 10 SDK**. NuGet restore happens automatically. Open `iTuneslyrics.sln` in Visual Studio 2022 or later, or build from the command line with `dotnet build`.

The iTunes COM interop DLL (`lib/Interop.iTunesLib.dll`) is pre-generated and checked into the repo so CI builds work without a local iTunes install. To regenerate it (requires iTunes):

```
tlbimp "C:\Program Files\iTunes\iTunes.exe" /out:lib\Interop.iTunesLib.dll /namespace:iTunesLib
```

## Credits

All credit for the original utility goes to the authors of [iLyrics](https://code.google.com/p/ilyrics/). This fork exists only to keep it working against current lyrics sources.

## Feedback

Bugs, requests, or questions: please open an issue on the [GitHub issues page](https://github.com/amd989/iTunesLyrics/issues).

## Changelog

### 1.5.0 - 2026-11-17
- New **WPF** interface with Fluent Design (WPF-UI): icon sidebar with Home, Results and Settings pages, Mica backdrop, and light/dark theme that follows Windows.
- The track selection updates automatically as you select songs in iTunes.
- Auto update runs keep going when you switch pages, show live progress, and can be cancelled.
- Manual review shows album art and lets you edit lyrics before writing them.
- Clearer errors when iTunes is not running or the Genius token is missing.
- Removed the **Fix** option (library scan for lyrics with broken characters).
- Migrated from .NET Framework 4.7.2 to **.NET 10**.
- Replaced the Genius.NET library with a lightweight direct HTTP client.
- Build system modernized: `dotnet build` replaces `msbuild`/`nuget restore`.

### 1.4.0 - 2026-04-19
- Fuzzy matching (FuzzySharp) for more accurate Genius search results.
- Always-on-top..
- ClickOnce and portable ZIP builds via GitHub Actions.

### 1.3.0.0 - 2026
- Migrated from LyricWiki to the Genius API (LyricWiki was discontinued).
- Added a Settings menu for storing your Genius API token in `%APPDATA%\iTunesLyrics\config.txt` instead of hardcoding it.

### 1.2.1.5 - 2015-04-26
- Added HtmlAgilityPack as a reference.
- Changed publish location.

### 1.2.1.4 - 2014-10-31
- Stripped the scripted ad LyricWiki was injecting into lyrics.

### 1.2.1 - 2013-08-31
- Project uploaded to GitHub.

### 1.2 - 2012-07-15
- LyricWiki support restored after the upstream API change.

### Original changelog

**1.1.1 - 2008-09-19**
- 1.1.1.2 fixed crashes while processing deleted files.
- 1.1.1.1 blank artist/name tags are skipped in batch mode.
- 1.1.1.0 ability to overwrite lyrics in automatic mode.
- 1.1.0.1 UTF-8 character support (thanks to davidreis).

**1.1 - 2007-03-07**
- A few bug fixes.
- Update now skips songs that already have lyrics in iTunes.

**1.0 - 2007-02-08**
- Initial release.
