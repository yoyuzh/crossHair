# MVP Overlay Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build the first usable Windows crosshair overlay skeleton with compliance boundaries, local configuration, core rendering math, and a WPF shell.

**Architecture:** The app is split into a WPF desktop project and a test project. Testable logic lives in small classes under `Config`, `Models`, `Rendering`, and `Compliance`; WPF and Win32 code is kept in `OverlayWindow`, `MainWindow`, `HotkeyService`, and `Win32` wrappers. The first version uses a normal transparent topmost window and explicitly does not scan game processes, hook graphics APIs, inject DLLs, capture frames, or send input.

**Tech Stack:** C# 12, .NET 8 Windows Desktop, WPF, Win32 P/Invoke, System.Text.Json, xUnit.

---

## File Structure

- Create `CrosshairOverlay.sln`: solution root.
- Create `CrosshairOverlay.App/CrosshairOverlay.App.csproj`: WPF app project targeting `net8.0-windows`.
- Create `CrosshairOverlay.Tests/CrosshairOverlay.Tests.csproj`: xUnit test project.
- Create `CrosshairOverlay.App/Models/CrosshairStyle.cs`: crosshair style enum.
- Create `CrosshairOverlay.App/Models/CrosshairSettings.cs`: persisted settings model and defaults.
- Create `CrosshairOverlay.App/Config/ConfigService.cs`: JSON load/save with fallback for missing or invalid config.
- Create `CrosshairOverlay.App/Compliance/ComplianceNotice.cs`: compliance notice version and text.
- Create `CrosshairOverlay.App/Rendering/CrosshairGeometry.cs`: shape-independent line/circle geometry.
- Create `CrosshairOverlay.App/Rendering/CrosshairRenderer.cs`: WPF drawing adapter that consumes geometry.
- Create `CrosshairOverlay.App/Native/Win32.cs`: P/Invoke and constants for overlay styles, hit-test, and hotkeys.
- Create `CrosshairOverlay.App/OverlayWindow.xaml` and `.xaml.cs`: transparent, click-through, topmost overlay.
- Create `CrosshairOverlay.App/MainWindow.xaml` and `.xaml.cs`: simple settings window.
- Create `CrosshairOverlay.App/App.xaml` and `.xaml.cs`: startup, config load, compliance prompt, overlay creation.
- Create `CrosshairOverlay.App/Hotkeys/HotkeyService.cs`: `RegisterHotKey` wrapper for F8 toggle.
- Create `CrosshairOverlay.App/Tray/TrayService.cs`: WinForms notify icon with settings and exit commands.
- Create `README.md`: scope, compliance, build/run instructions.

---

### Task 1: Create Solution Skeleton

**Files:**
- Create: `CrosshairOverlay.sln`
- Create: `CrosshairOverlay.App/CrosshairOverlay.App.csproj`
- Create: `CrosshairOverlay.Tests/CrosshairOverlay.Tests.csproj`
- Create: `.gitignore`

- [ ] **Step 1: Create the solution and project files**

Create a WPF executable and xUnit test project targeting .NET 8. Use Windows desktop SDK for the app and reference the app from tests.

- [ ] **Step 2: Verify project discovery**

Run: `dotnet sln CrosshairOverlay.sln list`

Expected: lists `CrosshairOverlay.App` and `CrosshairOverlay.Tests`.

- [ ] **Step 3: Commit**

Run:

```bash
git add CrosshairOverlay.sln CrosshairOverlay.App/CrosshairOverlay.App.csproj CrosshairOverlay.Tests/CrosshairOverlay.Tests.csproj .gitignore
git commit -m "chore: add solution skeleton"
```

---

### Task 2: Add Settings Model and Config Service With Tests

**Files:**
- Create: `CrosshairOverlay.App/Models/CrosshairStyle.cs`
- Create: `CrosshairOverlay.App/Models/CrosshairSettings.cs`
- Create: `CrosshairOverlay.App/Config/ConfigService.cs`
- Create: `CrosshairOverlay.Tests/ConfigServiceTests.cs`

- [ ] **Step 1: Write failing tests**

Test defaults, round-trip JSON persistence, and invalid JSON fallback.

- [ ] **Step 2: Run tests to verify they fail**

Run: `dotnet test CrosshairOverlay.Tests/CrosshairOverlay.Tests.csproj --filter ConfigServiceTests`

Expected: FAIL because settings and config classes do not exist yet.

- [ ] **Step 3: Implement model and config service**

Implement `CrosshairSettings.CreateDefault()`, `ConfigService.Load()`, and `ConfigService.Save()` using `System.Text.Json`.

- [ ] **Step 4: Run tests to verify they pass**

Run: `dotnet test CrosshairOverlay.Tests/CrosshairOverlay.Tests.csproj --filter ConfigServiceTests`

Expected: PASS.

- [ ] **Step 5: Commit**

Run:

```bash
git add CrosshairOverlay.App/Models CrosshairOverlay.App/Config CrosshairOverlay.Tests/ConfigServiceTests.cs
git commit -m "feat: add persisted crosshair settings"
```

---

### Task 3: Add Crosshair Geometry With Tests

**Files:**
- Create: `CrosshairOverlay.App/Rendering/CrosshairGeometry.cs`
- Create: `CrosshairOverlay.Tests/CrosshairGeometryTests.cs`

- [ ] **Step 1: Write failing tests**

Test `CrossDot` four line segments around the center, dot radius from thickness, circle radius from size, and offset handling.

- [ ] **Step 2: Run tests to verify they fail**

Run: `dotnet test CrosshairOverlay.Tests/CrosshairOverlay.Tests.csproj --filter CrosshairGeometryTests`

Expected: FAIL because geometry classes do not exist yet.

- [ ] **Step 3: Implement geometry**

Create immutable records for line and circle primitives and a `CrosshairGeometry.Build()` method that converts settings into primitives.

- [ ] **Step 4: Run tests to verify they pass**

Run: `dotnet test CrosshairOverlay.Tests/CrosshairOverlay.Tests.csproj --filter CrosshairGeometryTests`

Expected: PASS.

- [ ] **Step 5: Commit**

Run:

```bash
git add CrosshairOverlay.App/Rendering/CrosshairGeometry.cs CrosshairOverlay.Tests/CrosshairGeometryTests.cs
git commit -m "feat: add crosshair geometry"
```

---

### Task 4: Add Compliance Notice

**Files:**
- Create: `CrosshairOverlay.App/Compliance/ComplianceNotice.cs`
- Create: `CrosshairOverlay.Tests/ComplianceNoticeTests.cs`

- [ ] **Step 1: Write failing tests**

Test the notice contains the required boundaries: no game process access, no game modification, no input automation, and rule-check warning.

- [ ] **Step 2: Run tests to verify they fail**

Run: `dotnet test CrosshairOverlay.Tests/CrosshairOverlay.Tests.csproj --filter ComplianceNoticeTests`

Expected: FAIL because compliance class does not exist yet.

- [ ] **Step 3: Implement compliance notice**

Add static notice version `2026-05-20`, title, body, and acceptance button text.

- [ ] **Step 4: Run tests to verify they pass**

Run: `dotnet test CrosshairOverlay.Tests/CrosshairOverlay.Tests.csproj --filter ComplianceNoticeTests`

Expected: PASS.

- [ ] **Step 5: Commit**

Run:

```bash
git add CrosshairOverlay.App/Compliance CrosshairOverlay.Tests/ComplianceNoticeTests.cs
git commit -m "feat: add compliance notice"
```

---

### Task 5: Add WPF Overlay and Settings Shell

**Files:**
- Create: `CrosshairOverlay.App/App.xaml`
- Create: `CrosshairOverlay.App/App.xaml.cs`
- Create: `CrosshairOverlay.App/MainWindow.xaml`
- Create: `CrosshairOverlay.App/MainWindow.xaml.cs`
- Create: `CrosshairOverlay.App/OverlayWindow.xaml`
- Create: `CrosshairOverlay.App/OverlayWindow.xaml.cs`
- Create: `CrosshairOverlay.App/Rendering/CrosshairRenderer.cs`
- Create: `CrosshairOverlay.App/Native/Win32.cs`

- [ ] **Step 1: Implement app startup**

Load config from `%APPDATA%\CrosshairOverlay\config.json`, show compliance notice if not accepted, then show overlay and settings window.

- [ ] **Step 2: Implement overlay**

Use borderless transparent topmost WPF window. Set click-through styles after source initialization and return `HTTRANSPARENT` from `WM_NCHITTEST`.

- [ ] **Step 3: Implement renderer**

Use `DrawingContext` to draw geometry primitives with anti-aliased WPF drawing.

- [ ] **Step 4: Implement settings window**

Provide enabled toggle, style combo box, size/thickness/gap/opacity sliders, offset fields, save/reset buttons, and live overlay update.

- [ ] **Step 5: Build**

Run: `dotnet build CrosshairOverlay.sln`

Expected: build succeeds.

- [ ] **Step 6: Commit**

Run:

```bash
git add CrosshairOverlay.App
git commit -m "feat: add WPF overlay shell"
```

---

### Task 6: Add Hotkey, Tray, README, and Final Verification

**Files:**
- Create: `CrosshairOverlay.App/Hotkeys/HotkeyService.cs`
- Create: `CrosshairOverlay.App/Tray/TrayService.cs`
- Create: `README.md`

- [ ] **Step 1: Implement hotkey service**

Register F8 with `RegisterHotKey` and invoke a toggle callback on `WM_HOTKEY`.

- [ ] **Step 2: Implement tray service**

Use `System.Windows.Forms.NotifyIcon` with menu items: Open Settings, Toggle Crosshair, Exit.

- [ ] **Step 3: Write README**

Document scope, compliance boundary, build instructions, supported game display modes, config location, and uninstall cleanup.

- [ ] **Step 4: Run full verification**

Run:

```bash
dotnet test CrosshairOverlay.sln
dotnet build CrosshairOverlay.sln
```

Expected: tests and build pass.

- [ ] **Step 5: Commit**

Run:

```bash
git add CrosshairOverlay.App/Hotkeys CrosshairOverlay.App/Tray README.md
git commit -m "feat: add app controls and documentation"
```

---

## Self-Review

- Spec coverage: The plan implements MVP overlay display, click-through, local JSON config, F8 toggle, settings window, tray, compliance notice, and README boundaries.
- Known environment risk: Current machine has .NET runtimes but no .NET SDK. `dotnet build` and `dotnet test` require installing the .NET 8 SDK or using a machine with the SDK.
- Scope intentionally deferred: multi-monitor selection polish, startup registration, Game Bar widget, packaging, signing, and visual theme refinement.
