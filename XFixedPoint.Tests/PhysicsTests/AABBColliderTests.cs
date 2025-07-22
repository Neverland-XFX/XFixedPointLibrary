using XFixedPoint.Core;
using XFixedPoint.Physics.Collision;
using XFixedPoint.Vectors;

namespace XFixedPoint.Tests.PhysicsTests;

    public class AABBColliderTests
    {
        private const double Tolerance = 1e-5;

        [Fact]
        public void Overlaps_TwoAABBs_AtOrigin_True()
        {
            var a = new AABBCollider(XFixedVector3.FromFloat(1f,1f,1f));
            var b = new AABBCollider(XFixedVector3.FromFloat(1f,1f,1f));
            // 都在原点，半尺寸相同，应当重叠
            Assert.True(a.Overlaps(b));
            Assert.True(b.Overlaps(a));

            Assert.True(a.ComputeManifold(b, out var m) && m.Colliding);
            // 穿透深度应为 2*half - 0 = 2
            Assert.InRange(m.PenetrationDepth.ToDouble(), 2.0 - Tolerance, 2.0 + Tolerance);
            // 法线方向任意取 X 轴正方向
            Assert.Equal(new XFixedVector3(XFixed.One, XFixed.Zero, XFixed.Zero), m.Normal);
        }

        [Fact]
        public void Overlaps_TwoAABBs_Separated_False()
        {
            var a = new AABBCollider(XFixedVector3.FromFloat(1f,1f,1f));
            var b = new AABBCollider(XFixedVector3.FromFloat(1f,1f,1f))
            {
                LocalOffset = XFixedVector3.FromFloat(3f, 0f, 0f)
            };
            // 中心相隔 3，大于 sum(half)=2，不应重叠
            Assert.False(a.Overlaps(b));
            Assert.False(b.Overlaps(a));

            // ComputeManifold 应返回 Colliding = false
            Assert.False(a.ComputeManifold(b, out var m));
            Assert.False(m.Colliding);
        }

        [Fact]
        public void ComputeManifold_PartialOverlap_CorrectDepthAndNormal()
        {
            var a = new AABBCollider(XFixedVector3.FromFloat(1f,1f,1f));
            var b = new AABBCollider(XFixedVector3.FromFloat(1f,1f,1f))
            {
                LocalOffset = XFixedVector3.FromFloat(1.5f, 0f, 0f)
            };
            Assert.True(a.Overlaps(b));
            Assert.True(a.ComputeManifold(b, out var m) && m.Colliding);

            // 按手算：sum = 2, centerDist = 1.5, penetration = 2 - 1.5 = 0.5
            Assert.InRange(m.PenetrationDepth.ToDouble(), 0.5 - Tolerance, 0.5 + Tolerance);
            // 法线应沿 +X 方向
            Assert.Equal(new XFixedVector3(XFixed.One, XFixed.Zero, XFixed.Zero), m.Normal);
        }
    }