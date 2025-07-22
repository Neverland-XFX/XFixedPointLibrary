using XFixedPoint.Core;

namespace XFixedPoint.Tests.CoreTests;

public class XFixedArithmeticTests
{
    private const double Tolerance = 1e-6;

    [Theory]
    [InlineData( 1.0,  2.0)]
    [InlineData(-1.5,  3.5)]
    [InlineData( 0.0,  5.0)]
    public void Add_Subtract_ShouldMatchDouble(double a, double b)
    {
        var fa = XFixed.FromDouble(a);
        var fb = XFixed.FromDouble(b);

        var sum   = (fa + fb).ToDouble();
        var diff1 = (fa - fb).ToDouble();
        var diff2 = (fb - fa).ToDouble();

        Assert.InRange(sum,   a + b - Tolerance, a + b + Tolerance);
        Assert.InRange(diff1, a - b - Tolerance, a - b + Tolerance);
        Assert.InRange(diff2, b - a - Tolerance, b - a + Tolerance);
    }

    [Theory]
    [InlineData( 1.0,  2.0)]
    [InlineData( 3.5, -1.5)]
    [InlineData(-2.0, -4.0)]
    public void Multiply_ShouldMatchDouble(double a, double b)
    {
        var fa = XFixed.FromDouble(a);
        var fb = XFixed.FromDouble(b);

        var prod = (fa * fb).ToDouble();
        Assert.InRange(prod, a * b - Tolerance, a * b + Tolerance);
    }

    [Theory]
    [InlineData( 5.0,  2.0)]
    [InlineData( 7.5, -2.5)]
    [InlineData(-4.0,  2.0)]
    public void Divide_ShouldMatchDouble(double a, double b)
    {
        var fa = XFixed.FromDouble(a);
        var fb = XFixed.FromDouble(b);

        var quotient = (fa / fb).ToDouble();
        Assert.InRange(quotient, a / b - Tolerance, a / b + Tolerance);
    }

    [Fact]
    public void Divide_ByZero_ShouldThrow()
    {
        var fa = XFixed.FromInt(1);
        var fb = XFixed.Zero;
        Assert.Throws<DivideByZeroException>(() => _ = fa / fb);
    }
}