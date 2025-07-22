using XFixedPoint.Core;
using XFixedPoint.Vectors;

namespace XFixedPoint.Tests.VectorTests;

    public class XFixedVector2Tests
    {
        private const double Tolerance = 1e-5;

        [Fact]
        public void Addition_Subtraction_Equality()
        {
            var a = new XFixedVector2(XFixed.FromDouble(1.5), XFixed.FromDouble(-2.25));
            var b = new XFixedVector2(XFixed.FromDouble(-0.5), XFixed.FromDouble( 1.25));

            var sum = a + b;
            Assert.InRange(sum.X.ToDouble(), 1.5 + -0.5 - Tolerance, 1.5 + -0.5 + Tolerance);
            Assert.InRange(sum.Y.ToDouble(), -2.25 + 1.25 - Tolerance, -2.25 + 1.25 + Tolerance);

            var diff = a - b;
            Assert.InRange(diff.X.ToDouble(), 1.5 - -0.5 - Tolerance, 1.5 - -0.5 + Tolerance);
            Assert.InRange(diff.Y.ToDouble(), -2.25 - 1.25 - Tolerance, -2.25 - 1.25 + Tolerance);

            Assert.True(a == new XFixedVector2(XFixed.FromDouble(1.5), XFixed.FromDouble(-2.25)));
            Assert.False(a == b);
            Assert.True(a != b);
        }

        [Fact]
        public void Dot_Cross_Magnitude_Normalization()
        {
            var v1 = new XFixedVector2(XFixed.FromDouble(3), XFixed.FromDouble(4));
            var v2 = new XFixedVector2(XFixed.FromDouble(-4), XFixed.FromDouble(3));

            // dot: 3*(-4) + 4*3 = 0
            Assert.InRange(v1.Dot(v2).ToDouble(), 0 - Tolerance, 0 + Tolerance);

            // cross pseudo-scalar: 3*3 - 4*(-4) = 9 + 16 = 25
            Assert.InRange(v1.Cross(v2).ToDouble(), 25 - Tolerance, 25 + Tolerance);

            // magnitude of v1: 5
            Assert.InRange(v1.Magnitude.ToDouble(), 5 - Tolerance, 5 + Tolerance);
            // sqr magnitude
            Assert.InRange(v1.SqrMagnitude.ToDouble(), 25 - Tolerance, 25 + Tolerance);

            // normalization of v1 yields unit vector (0.6, 0.8)
            var n = v1.Normalized;
            Assert.InRange(n.X.ToDouble(), 0.6 - Tolerance, 0.6 + Tolerance);
            Assert.InRange(n.Y.ToDouble(), 0.8 - Tolerance, 0.8 + Tolerance);

            // normalization of zero vector remains zero
            Assert.Equal(XFixedVector2.Zero, XFixedVector2.Zero.Normalized);
        }

        [Fact]
        public void Lerp_WorksCorrectly()
        {
            var a = new XFixedVector2(XFixed.FromDouble(0), XFixed.FromDouble(0));
            var b = new XFixedVector2(XFixed.FromDouble(10), XFixed.FromDouble(20));

            var mid = XFixedVector2.Lerp(a, b, XFixed.FromDouble(0.5));
            Assert.InRange(mid.X.ToDouble(), 5 - Tolerance, 5 + Tolerance);
            Assert.InRange(mid.Y.ToDouble(), 10 - Tolerance, 10 + Tolerance);

            // t outside [0,1] is clamped
            var before = XFixedVector2.Lerp(a, b, XFixed.FromDouble(-0.5));
            Assert.Equal(a, before);
            var after  = XFixedVector2.Lerp(a, b, XFixed.FromDouble(1.5));
            Assert.Equal(b, after);
        }
    }