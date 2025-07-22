using XFixedPoint.Core;
using XFixedPoint.Utilities;

namespace XFixedPoint.Tests.UtilitiesTests;

public class XFixedTimeTests
{
    [Fact]
    public void Tick_IncrementsFrameCountAndTime()
    {
        XFixedTime.Reset();
        Assert.Equal(0, XFixedTime.FrameCount);
        Assert.Equal(XFixed.Zero, XFixedTime.Time);
        Assert.Equal(XFixed.Zero, XFixedTime.DeltaTime);

        var dt1 = XFixed.FromDouble(0.1);
        XFixedTime.Tick(dt1);
        Assert.Equal(1, XFixedTime.FrameCount);
        Assert.Equal(dt1, XFixedTime.DeltaTime);
        Assert.Equal(dt1, XFixedTime.Time);

        var dt2 = XFixed.FromDouble(0.2);
        XFixedTime.Tick(dt2);
        Assert.Equal(2, XFixedTime.FrameCount);
        Assert.Equal(dt2, XFixedTime.DeltaTime);
        Assert.Equal(dt1 + dt2, XFixedTime.Time);
    }

    [Fact]
    public void Reset_ClearsAll()
    {
        // Simulate some ticks
        XFixedTime.Tick(XFixed.FromDouble(0.5));
        XFixedTime.Tick(XFixed.FromDouble(0.25));
        Assert.True(XFixedTime.FrameCount > 0);

        XFixedTime.Reset();
        Assert.Equal(0, XFixedTime.FrameCount);
        Assert.Equal(XFixed.Zero, XFixedTime.Time);
        Assert.Equal(XFixed.Zero, XFixedTime.DeltaTime);
    }
}