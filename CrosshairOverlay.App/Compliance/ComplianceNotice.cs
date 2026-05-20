namespace CrosshairOverlay.App.Compliance;

public static class ComplianceNotice
{
    public const string Version = "2026-05-20";
    public const string Title = "外置准星使用提示";
    public const string AcceptText = "我已了解";
    public const string ExitText = "退出";

    public static string Body =>
        "本工具只在屏幕上显示静态中心标记。\n" +
        "本工具不会读取、修改或控制任何游戏进程。\n" +
        "本工具不会自动瞄准、压枪、连点、开火或发送任何游戏输入。\n" +
        "部分游戏、赛事或反作弊系统可能禁止外置准星；使用前请确认对应规则。";

    public static bool RequiresAcceptance(string? acceptedVersion, bool accepted)
    {
        return !accepted || !string.Equals(acceptedVersion, Version, StringComparison.Ordinal);
    }
}
