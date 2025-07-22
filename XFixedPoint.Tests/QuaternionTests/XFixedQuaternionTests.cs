using XFixedPoint.Core;
using XFixedPoint.Quaternions;
using XFixedPoint.Vectors;

namespace XFixedPoint.Tests.QuaternionTests;

    public class XFixedQuaternionTests
    {
        private const double Tolerance = 1e-5;

        [Fact]
        public void Identity_Rotation_IsNoOp()
        {
            var q = XFixedQuaternion.Identity;
            var v = new XFixedVector3(XFixed.FromDouble(1), XFixed.FromDouble(2), XFixed.FromDouble(3));
            var rotated = q.Rotate(v);
            Assert.Equal(v, rotated);
        }

        [Fact]
        public void FromAxisAngle_RotatesCorrectly_AroundZ90deg()
        {
            // 90° = PI/2 around Z, rotating (1,0,0) → (0,1,0)
            var axis = new XFixedVector3(XFixed.Zero, XFixed.Zero, XFixed.One);
            var angle = XFixed.FromDouble(Math.PI / 2);
            var q = XFixedQuaternion.FromAxisAngle(axis, angle);

            var v = new XFixedVector3(XFixed.One, XFixed.Zero, XFixed.Zero);
            var r = q.Rotate(v);
            Assert.InRange(r.X.ToDouble(), 0 - Tolerance, 0 + Tolerance);
            Assert.InRange(r.Y.ToDouble(), 1 - Tolerance, 1 + Tolerance);
            Assert.InRange(r.Z.ToDouble(), 0 - Tolerance, 0 + Tolerance);
        }

        [Fact]
        public void Conjugate_Multiplication_YieldsIdentity()
        {
            var axis = new XFixedVector3(XFixed.Zero, XFixed.One, XFixed.Zero);
            var angle = XFixed.FromDouble(1.234); 
            var q = XFixedQuaternion.FromAxisAngle(axis, angle);
            var qConj = q.Conjugate;
            var res = q * qConj;
            // Should be identity (0,0,0,1)
            Assert.Equal(XFixedQuaternion.Identity, res.Normalized);
        }

        [Fact]
        public void Normalize_ZeroQuaternion_GivesIdentity()
        {
            var zeroQ = new XFixedQuaternion(XFixed.Zero, XFixed.Zero, XFixed.Zero, XFixed.Zero);
            Assert.Equal(XFixedQuaternion.Identity, zeroQ.Normalized);
        }

        [Fact]
        public void Multiplication_IsAssociative()
        {
            var q1 = XFixedQuaternion.FromAxisAngle(XFixedVector3.UnitX, XFixed.FromDouble(0.3));
            var q2 = XFixedQuaternion.FromAxisAngle(XFixedVector3.UnitY, XFixed.FromDouble(0.5));
            var q3 = XFixedQuaternion.FromAxisAngle(XFixedVector3.UnitZ, XFixed.FromDouble(0.7));

            var a = (q1 * q2) * q3;
            var b = q1 * (q2 * q3);
            // Up to normalization they should match
            Assert.Equal(a.Normalized, b.Normalized);
        }
    }