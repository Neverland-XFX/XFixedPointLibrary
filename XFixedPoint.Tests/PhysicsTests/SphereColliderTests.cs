using XFixedPoint.Core;
using XFixedPoint.Physics.Collision;
using XFixedPoint.Vectors;

namespace XFixedPoint.Tests.PhysicsTests;

    public class SphereColliderTests
    {
        private const double Tolerance = 1e-5;

        [Fact]
        public void Overlaps_TwoSpheres_AtOrigin_True()
        {
            var a = new SphereCollider(XFixed.FromDouble(1.0));
            var b = new SphereCollider(XFixed.FromDouble(1.0));
            // 两球同心，半径均为 1，应重叠
            Assert.True(a.Overlaps(b));
            Assert.True(b.Overlaps(a));

            Assert.True(a.ComputeManifold(b, out var m) && m.Colliding);
            // 零距离时约定法线为 (1,0,0)
            Assert.Equal(new XFixedVector3(XFixed.One, XFixed.Zero, XFixed.Zero), m.Normal);
            // 穿透深度 rA+rB - 0 = 2
            Assert.InRange(m.PenetrationDepth.ToDouble(), 2.0 - Tolerance, 2.0 + Tolerance);
        }

        [Fact]
        public void Overlaps_TwoSpheres_Separated_False()
        {
            var a = new SphereCollider(XFixed.FromDouble(1.0));
            var b = new SphereCollider(XFixed.FromDouble(1.0))
            {
                LocalOffset = XFixedVector3.FromFloat(2.5f, 0f, 0f)
            };
            // 中心相隔 2.5，大于 sum=2，不重叠
            Assert.False(a.Overlaps(b));
            Assert.False(b.Overlaps(a));

            Assert.False(a.ComputeManifold(b, out var m));
            Assert.False(m.Colliding);
        }

        [Fact]
        public void ComputeManifold_PartialOverlap_CorrectDepthAndNormal()
        {
            var a = new SphereCollider(XFixed.FromDouble(1.0));
            var b = new SphereCollider(XFixed.FromDouble(1.0))
            {
                LocalOffset = XFixedVector3.FromFloat(1.5f, 0f, 0f)
            };
            Assert.True(a.Overlaps(b));
            Assert.True(a.ComputeManifold(b, out var m) && m.Colliding);

            // penetration = 2 - 1.5 = 0.5
            Assert.InRange(m.PenetrationDepth.ToDouble(), 0.5 - Tolerance, 0.5 + Tolerance);
            // 法线应沿 +X 方向
            Assert.Equal(new XFixedVector3(XFixed.One, XFixed.Zero, XFixed.Zero), m.Normal);

            // 接触点应该在 A 球表面：centerA + normal * (rA - penetration*0.5)
            // rA=1, penetration=0.5 -> 1 - 0.25 = 0.75
            Assert.InRange(m.ContactPoint.X.ToDouble(), 0.75 - Tolerance, 0.75 + Tolerance);
            Assert.InRange(m.ContactPoint.Y.ToDouble(), 0.0 - Tolerance,   0.0 + Tolerance);
            Assert.InRange(m.ContactPoint.Z.ToDouble(), 0.0 - Tolerance,   0.0 + Tolerance);
        }
    }