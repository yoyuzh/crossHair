# Crosshair Overlay

一个 Windows 屏幕中心标记工具。它用 C#、WPF 和少量 Win32 API 做一个透明置顶窗口，在屏幕中心画一个静态准星。

这个项目的边界很窄：只显示自己的窗口，不碰游戏进程，不做输入自动化。

## 现在能做什么

- 在桌面或无边框窗口化游戏上显示准星
- 支持圆点、十字、圆圈、十字加圆点
- 可以调颜色、大小、线宽、间隙、透明度和偏移
- 4 个自定义方案槽位，可以保存不同样式
- 默认 `F8` 显示或隐藏准星，默认 `F9` 切换到下一个方案
- 默认 `Ctrl+Alt+1` 到 `Ctrl+Alt+4` 直接选择方案
- 快捷键可以在设置窗口里改
- 托盘菜单可以打开设置、切换准星、退出程序
- 配置保存到 `%APPDATA%\CrosshairOverlay\config.json`
- 第一次启动会显示使用边界提示

## 不做什么

这个工具不会：

- 读取、写入或扫描游戏进程内存
- 识别敌人、血量、轮廓、后坐力、弹道或游戏 UI
- 截图做目标检测
- 发送键盘或鼠标输入
- 做压枪、连点、自动开火或自动瞄准
- 注入 DLL、Hook DirectX/Vulkan/OpenGL，或绕过反作弊

不同游戏、平台和赛事对第三方 overlay 的规则不一样。使用前请先看对应规则，尤其是排位、赛事和强反作弊游戏。

## 兼容性

| 场景 | 预期 |
| --- | --- |
| Windows 桌面 | 支持 |
| 普通窗口模式游戏 | 通常可用 |
| 无边框窗口化游戏 | 推荐 |
| 独占全屏游戏 | 不保证显示 |
| 强反作弊环境 | 不保证可用 |

如果游戏支持无边框窗口化，优先用这个模式。

## 快速启动

安装 .NET SDK 后，在仓库根目录运行：

```powershell
.\start.ps1
```

脚本会执行 restore、build，然后启动程序。

如果已经构建过，可以跳过构建：

```powershell
.\start.ps1 -SkipBuild
```

## 手动构建和测试

项目目标框架是 `net8.0-windows`。本机使用 .NET 10 SDK 也可以构建。

```powershell
dotnet restore CrosshairOverlay.sln
dotnet build CrosshairOverlay.sln
dotnet test CrosshairOverlay.sln
```

手动启动：

```powershell
dotnet run --project CrosshairOverlay.App\CrosshairOverlay.App.csproj
```

## 打包 exe

生成 Windows x64 自包含单文件：

```powershell
.\publish.ps1
```

输出位置：

```text
artifacts\publish\win-x64\CrosshairOverlay.App.exe
```

如果目标机器已经安装 .NET Desktop Runtime，也可以生成依赖运行时的版本：

```powershell
.\publish.ps1 -FrameworkDependent
```

## 项目结构

```text
CrosshairOverlay.App/
  App.xaml(.cs)                  启动流程、合规提示、窗口和托盘初始化
  MainWindow.xaml(.cs)           设置窗口
  OverlayWindow.xaml(.cs)        透明置顶准星窗口
  Config/ConfigService.cs        JSON 配置读写
  Compliance/ComplianceNotice.cs 首次启动提示文本
  Hotkeys/HotkeyService.cs       F8 全局热键
  Models/                        设置模型和准星样式
  Native/Win32.cs                Win32 API 封装
  Rendering/                     准星几何和 WPF 绘制
  Tray/TrayService.cs            托盘菜单

CrosshairOverlay.Tests/          配置、几何和合规提示测试
```

## 卸载清理

删除程序文件后，再删除配置文件：

```text
%APPDATA%\CrosshairOverlay\config.json
```
