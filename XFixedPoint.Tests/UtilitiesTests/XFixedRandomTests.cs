using XFixedPoint.Core;
using XFixedPoint.Utilities;

namespace XFixedPoint.Tests.UtilitiesTests;

public class XFixedRandomTests
{
    private const int SampleCount = 1000;

    [Fact]
    public void NextFixed_IsWithinZeroOne()
    {
        var rnd = new XFixedRandom(seed: 12345u);
        for (int i = 0; i < SampleCount; i++)
        {
            var f = rnd.NextFixed();
            // 0 <= f < 1
            Assert.InRange(f.Raw, 0L, XFixed.ONE - 1);
        }
    }

    [Fact]
    public void Range_ProducesValuesWithinBounds()
    {
        var rnd = new XFixedRandom(seed: 54321u);
        var min = XFixed.FromDouble(-5.5);
        var max = XFixed.FromDouble( 3.2);

        for (int i = 0; i < SampleCount; i++)
        {
            var f = rnd.Range(min, max);
            // min <= f < max
            Assert.True(f.Raw >= min.Raw, $"f={f.ToDouble()} < min={min.ToDouble()}");
            Assert.True(f.Raw <  max.Raw, $"f={f.ToDouble()} >= max={max.ToDouble()}");
        }
    }

    [Fact]
    public void NextUInt_ProducesDeterministicSequence()
    {
        var rnd1 = new XFixedRandom(seed: 1u);
        var rnd2 = new XFixedRandom(seed: 1u);
        // first few UInts should match
        for (int i = 0; i < 10; i++)
        {
            Assert.Equal(rnd1.NextUInt(), rnd2.NextUInt());
        }
    }
}