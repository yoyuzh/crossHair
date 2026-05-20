using CrosshairOverlay.App.Compliance;
using Xunit;

namespace CrosshairOverlay.Tests;

public sealed class ComplianceNoticeTests
{
    [Fact]
    public void NoticeStatesCoreSafetyBoundaries()
    {
        Assert.Contains("不会读取", ComplianceNotice.Body);
        Assert.Contains("游戏进程", ComplianceNotice.Body);
        Assert.Contains("不会自动瞄准", ComplianceNotice.Body);
        Assert.Contains("压枪", ComplianceNotice.Body);
        Assert.Contains("使用前请确认对应规则", ComplianceNotice.Body);
    }

    [Fact]
    public void RequiresAcceptanceWhenVersionIsMissingOrOutdated()
    {
        Assert.True(ComplianceNotice.RequiresAcceptance(null, false));
        Assert.True(ComplianceNotice.RequiresAcceptance("old", true));
        Assert.False(ComplianceNotice.RequiresAcceptance(ComplianceNotice.Version, true));
    }
}
