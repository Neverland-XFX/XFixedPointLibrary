using XFixedPoint.Core;
using XFixedPoint.Quaternions;
using XFixedPoint.Vectors;

namespace XFixedPoint.Tests.QuaternionTests;

    public class XFixedQuaternionOpsTests
    {
        private const double Tolerance = 1e-5;

        [Fact]
        public void Dot_And_Angle_AreConsistent()
        {
            var q1 = XFixedQuaternion.FromAxisAngle(XFixedVector3.UnitX, XFixed.FromDouble(0.4));
            var q2 = XFixedQuaternion.FromAxisAngle(XFixedVector3.UnitX, XFixed.FromDouble(0.9));

            var dot = XFixedQuaternionOps.Dot(q1, q2).ToDouble();
            var angle = XFixedQuaternionOps.Angle(q1, q2).ToDouble();

            // dot = cos(theta/2) if both normalized and same axis
            var expectedDot = Math.Cos((0.9 - 0.4) / 2);
            Assert.InRange(dot, expectedDot - Tolerance, expectedDot + Tolerance);
            Assert.InRange(angle, Math.Abs(0.9 - 0.4) - Tolerance, Math.Abs(0.9 - 0.4) + Tolerance);
        }

        [Fact]
        public void Slerp_BetweenIdentityAnd180deg_HalfwayGives90deg()
        {
            var q0 = XFixedQuaternion.Identity;
            // 180° around Y: cos=0, sin=1 for half-angle
            var q1 = XFixedQuaternion.FromAxisAngle(XFixedVector3.UnitY, XFixed.FromDouble(Math.PI));
            var qm = XFixedQuaternionOps.Slerp(q0, q1, XFixed.FromDouble(0.5));
            // Should be ~90° around Y
            var v = new XFixedVector3(XFixed.One, XFixed.Zero, XFixed.Zero);
            var rotated = qm.Rotate(v);
            // Rotating X axis 90° around Y → Z axis negative direction
            Assert.InRange(rotated.X.ToDouble(), 0 - Tolerance, 0 + Tolerance);
            Assert.InRange(rotated.Z.ToDouble(), -1 - Tolerance, -1 + Tolerance);
        }

        [Fact]
        public void EulerConversion_RoundTrips()
        {
            var euler = new XFixedVector3(
                XFixed.FromDouble(0.1),
                XFixed.FromDouble(-0.4),
                XFixed.FromDouble(1.2)
            );
            var q = XFixedQuaternionOps.FromEulerAngles(euler);
            var outEuler = XFixedQuaternionOps.ToEulerAngles(q);
            // Normalize angles to [-pi,pi] for comparison
            Assert.InRange(outEuler.X.ToDouble(), euler.X.ToDouble() - Tolerance, euler.X.ToDouble() + Tolerance);
            Assert.InRange(outEuler.Y.ToDouble(), euler.Y.ToDouble() - Tolerance, euler.Y.ToDouble() + Tolerance);
            Assert.InRange(outEuler.Z.ToDouble(), euler.Z.ToDouble() - Tolerance, euler.Z.ToDouble() + Tolerance);
        }

        [Fact]
        public void ToRotationMatrix_MatchesQuaternionRotate()
        {
            var axis = new XFixedVector3(XFixed.Zero, XFixed.One, XFixed.Zero);
            var angle = XFixed.FromDouble(1.0);
            var q = XFixedQuaternion.FromAxisAngle(axis, angle).Normalized;
            var mat = XFixedQuaternionOps.ToRotationMatrix(q);

            var v = new XFixedVector3(XFixed.FromDouble(2), XFixed.FromDouble(3), XFixed.FromDouble(5));
            var byQuat = q.Rotate(v);
            var byMat  = mat.MultiplyPoint(v);

            Assert.InRange(byMat.X.ToDouble(), byQuat.X.ToDouble() - Tolerance, byQuat.X.ToDouble() + Tolerance);
            Assert.InRange(byMat.Y.ToDouble(), byQuat.Y.ToDouble() - Tolerance, byQuat.Y.ToDouble() + Tolerance);
            Assert.InRange(byMat.Z.ToDouble(), byQuat.Z.ToDouble() - Tolerance, byQuat.Z.ToDouble() + Tolerance);
        }
    }