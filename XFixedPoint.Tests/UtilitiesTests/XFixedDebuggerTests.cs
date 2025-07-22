using XFixedPoint.Core;
using XFixedPoint.Utilities;

namespace XFixedPoint.Tests.UtilitiesTests;

public class XFixedDebuggerTests
{
    private const double Tolerance = 1e-9;

    [Fact]
    public void ComputeError_ReturnsDifference()
    {
        var fixedVal = XFixed.FromDouble(2.75);
        double expected = 3.125;
        double err = XFixedDebugger.ComputeError(fixedVal, expected);
        // actual = 2.75, so err = 2.75 - 3.125 = -0.375
        Assert.InRange(err, -0.375 - Tolerance, -0.375 + Tolerance);
    }

    [Fact]
    public void PrintError_WritesFormattedOutput()
    {
        var fixedVal = XFixed.FromDouble(1.234567);
        double expected = 1.0;
        using var sw = new StringWriter();
        Console.SetOut(sw);

        XFixedDebugger.PrintError("TestErr", fixedVal, expected);
        var output = sw.ToString();

        // Should contain label and both values
        Assert.Contains("TestErr:", output);
        Assert.Contains("Fixed=", output);
        Assert.Contains("Expected=", output);
        Assert.Contains("Error=", output);
    }

    [Fact]
    public void PrintStatistics_CalculatesAndWrites()
    {
        var fixedVals    = new[] { XFixed.FromDouble(1.0), XFixed.FromDouble(2.0), XFixed.FromDouble(3.0) };
        var expectedVals = new[] { 1.1,             1.9,             3.05          };
        // errors: -0.1, +0.1, -0.05 -> avg=0.016666..., max=0.1

        using var sw = new StringWriter();
        Console.SetOut(sw);

        XFixedDebugger.PrintStatistics("Stats", fixedVals, expectedVals);
        var output = sw.ToString();

        Assert.Contains("Stats Statistics:", output);
        Assert.Contains("Count=3", output);
        Assert.Contains("AvgError=", output);
        Assert.Contains("MaxError=", output);
    }
}