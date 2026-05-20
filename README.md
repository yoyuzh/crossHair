# Crosshair Overlay

Windows screen center marker built with C#, WPF, and Win32 APIs.

## Scope

This app displays a static crosshair in its own transparent top-level window. It is intended as a lightweight screen marker for desktop, training, and windowed or borderless-windowed games.

It does not:

- read, write, or scan game process memory
- inspect game objects, enemies, health, recoil, UI, or frame buffers
- capture screenshots for target detection
- send keyboard or mouse input
- implement recoil macros, clickers, auto-fire, or aim assist
- inject DLLs, hook DirectX/Vulkan/OpenGL, or bypass anti-cheat systems

## Compatibility

Recommended display modes:

- Desktop: supported
- Windowed games: expected to work
- Borderless-windowed games: expected to work
- Exclusive fullscreen games: not guaranteed
- Strong anti-cheat environments: not guaranteed

Some games, tournaments, or anti-cheat systems may restrict third-party overlays or external crosshairs. Check the rules of the game you play before using this app.

## Build

Install the .NET 8 SDK, then run:

```powershell
dotnet restore CrosshairOverlay.sln
dotnet build CrosshairOverlay.sln
dotnet test CrosshairOverlay.sln
```

Run the app:

```powershell
dotnet run --project CrosshairOverlay.App\CrosshairOverlay.App.csproj
```

## Current MVP

- Transparent topmost overlay window
- Mouse click-through behavior using Win32 extended styles and `WM_NCHITTEST`
- Four styles: dot, cross, circle, cross dot
- Color, size, thickness, gap, opacity, and offset settings
- JSON config under `%APPDATA%\CrosshairOverlay\config.json`
- First-run compliance notice
- F8 global toggle
- Tray menu for settings, toggle, and exit

## Uninstall Cleanup

Delete the app files and remove:

```text
%APPDATA%\CrosshairOverlay\config.json
```
