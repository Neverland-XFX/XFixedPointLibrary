using XFixedPoint.Core;
using XFixedPoint.Vectors;

namespace XFixedPoint.Tests.VectorTests;

    public class XFixedVector3Tests
    {
        private const double Tolerance = 1e-5;

        [Fact]
        public void Addition_Subtraction_Equality()
        {
            var a = new XFixedVector3(XFixed.FromDouble(1), XFixed.FromDouble(2), XFixed.FromDouble(3));
            var b = new XFixedVector3(XFixed.FromDouble(-3), XFixed.FromDouble(0.5), XFixed.FromDouble(1));

            var sum = a + b;
            Assert.InRange(sum.X.ToDouble(), 1 + -3 - Tolerance, 1 + -3 + Tolerance);
            Assert.InRange(sum.Y.ToDouble(), 2 + 0.5 - Tolerance, 2 + 0.5 + Tolerance);
            Assert.InRange(sum.Z.ToDouble(), 3 + 1 - Tolerance, 3 + 1 + Tolerance);

            var diff = a - b;
            Assert.InRange(diff.X.ToDouble(), 1 - -3 - Tolerance, 1 - -3 + Tolerance);
            Assert.InRange(diff.Y.ToDouble(), 2 - 0.5 - Tolerance, 2 - 0.5 + Tolerance);
            Assert.InRange(diff.Z.ToDouble(), 3 - 1 - Tolerance, 3 - 1 + Tolerance);

            Assert.True(a == new XFixedVector3(XFixed.FromDouble(1), XFixed.FromDouble(2), XFixed.FromDouble(3)));
            Assert.False(a == b);
            Assert.True(a != b);
        }

        [Fact]
        public void Dot_Cross_Magnitude_Normalization()
        {
            var vX = XFixedVector3.UnitX;
            var vY = XFixedVector3.UnitY;
            var vZ = XFixedVector3.UnitZ;

            // dot products
            Assert.InRange(vX.Dot(vY).ToDouble(), 0 - Tolerance, 0 + Tolerance);
            Assert.InRange(vY.Dot(vZ).ToDouble(), 0 - Tolerance, 0 + Tolerance);
            Assert.InRange(vZ.Dot(vX).ToDouble(), 0 - Tolerance, 0 + Tolerance);
            Assert.InRange(vX.Dot(vX).ToDouble(), 1 - Tolerance, 1 + Tolerance);

            // cross products
            var xy = vX.Cross(vY);
            Assert.Equal(vZ, xy);
            var yz = vY.Cross(vZ);
            Assert.Equal(vX, yz);
            var zx = vZ.Cross(vX);
            Assert.Equal(vY, zx);

            // magnitude
            var v = new XFixedVector3(XFixed.FromDouble(2), XFixed.FromDouble(-3), XFixed.FromDouble(6));
            var mag = v.Magnitude.ToDouble();
            Assert.InRange(mag, Math.Sqrt(4 + 9 + 36) - Tolerance, Math.Sqrt(4 + 9 + 36) + Tolerance);

            // normalization
            var n = v.Normalized;
            var nm = n.Magnitude.ToDouble();
            Assert.InRange(nm, 1 - Tolerance, 1 + Tolerance);
            // zero vector stays zero
            Assert.Equal(XFixedVector3.Zero, XFixedVector3.Zero.Normalized);
        }

        [Fact]
        public void Lerp_WorksCorrectly()
        {
            var a = new XFixedVector3(XFixed.FromDouble(0), XFixed.FromDouble(0), XFixed.FromDouble(0));
            var b = new XFixedVector3(XFixed.FromDouble(1), XFixed.FromDouble(2), XFixed.FromDouble(3));

            var half = XFixedVector3.Lerp(a, b, XFixed.FromDouble(0.5));
            Assert.InRange(half.X.ToDouble(), 0.5 - Tolerance, 0.5 + Tolerance);
            Assert.InRange(half.Y.ToDouble(), 1.0 - Tolerance, 1.0 + Tolerance);
            Assert.InRange(half.Z.ToDouble(), 1.5 - Tolerance, 1.5 + Tolerance);

            // t clamped
            Assert.Equal(a, XFixedVector3.Lerp(a, b, XFixed.FromDouble(-0.1)));
            Assert.Equal(b, XFixedVector3.Lerp(a, b, XFixed.FromDouble(1.1)));
        }
    }