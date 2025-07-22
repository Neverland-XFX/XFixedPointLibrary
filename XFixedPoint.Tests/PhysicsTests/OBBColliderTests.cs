using XFixedPoint.Core;
using XFixedPoint.Physics.Collision;
using XFixedPoint.Quaternions;
using XFixedPoint.Vectors;

namespace XFixedPoint.Tests.PhysicsTests;

    public class OBBColliderTests
    {
        private const double Tolerance = 1e-5;

        [Fact]
        public void Overlaps_IdenticalOBBs_True()
        {
            var half = XFixedVector3.FromFloat(1f, 2f, 3f);
            var a = new OBBCollider(half);
            var b = new OBBCollider(half);
            // 默认局部偏移和旋转都为 Identity/Zero，中心同心，必重叠
            Assert.True(a.Overlaps(b));
            Assert.True(b.Overlaps(a));

            Assert.True(a.ComputeManifold(b, out var m) && m.Colliding);
            // 对于中心重合，取任一分量最小的轴，penetration = 2*half（在最小半尺寸方向）
            // 最小半尺寸是 X 轴 half.X = 1 → 2*1 = 2
            Assert.InRange(m.PenetrationDepth.ToDouble(), 2.0 - Tolerance, 2.0 + Tolerance);
            // 法线应为单位向量
            Assert.InRange(m.Normal.Magnitude.ToDouble(), 1.0 - Tolerance, 1.0 + Tolerance);
        }

        [Fact]
        public void Overlaps_SeparatedOBBs_False()
        {
            var half = XFixedVector3.FromFloat(1f, 1f, 1f);
            var a = new OBBCollider(half);
            var b = new OBBCollider(half)
            {
                LocalOffset = XFixedVector3.FromFloat(3f, 0f, 0f)
            };
            // 中心相隔 3，大于 sum(half)=2，不应重叠
            Assert.False(a.Overlaps(b));
            Assert.False(b.Overlaps(a));

            Assert.False(a.ComputeManifold(b, out var m));
            Assert.False(m.Colliding);
        }

        [Fact]
        public void ComputeManifold_PartialOverlap_CorrectDepthAndDirection()
        {
            var half = XFixedVector3.FromFloat(1f, 2f, 1f);
            var a = new OBBCollider(half);
            var b = new OBBCollider(half)
            {
                LocalOffset = XFixedVector3.FromFloat(1.5f, 0f, 0f)
            };
            Assert.True(a.Overlaps(b));
            Assert.True(a.ComputeManifold(b, out var m) && m.Colliding);

            // 沿 X 轴： penetration = (1+1) - 1.5 = 0.5
            Assert.InRange(m.PenetrationDepth.ToDouble(), 0.5 - Tolerance, 0.5 + Tolerance);
            // 法线应沿 X 轴正方向
            Assert.Equal(new XFixedVector3(XFixed.One, XFixed.Zero, XFixed.Zero), m.Normal);
        }

        [Fact]
        public void Overlaps_RotatedOBBs_True()
        {
            var half = XFixedVector3.FromFloat(1f, 2f, 1f);
            var a = new OBBCollider(half);
            var b = new OBBCollider(half)
            {
                // 旋转 90° 绕 Z 轴
                LocalRotation = XFixedQuaternion.FromAxisAngle(
                    new XFixedVector3(XFixed.Zero, XFixed.Zero, XFixed.One),
                    XFixed.FromDouble(Math.PI / 2))
            };
            // 中心同心，即使一个盒子旋转，也必重叠
            Assert.True(a.Overlaps(b));
            Assert.True(b.Overlaps(a));

            Assert.True(a.ComputeManifold(b, out var m) && m.Colliding);
            // 旋转后法线方向可能不同，但深度仍大于零
            Assert.True(m.PenetrationDepth.ToDouble() > 0);
            // 法线仍应为单位向量
            Assert.InRange(m.Normal.Magnitude.ToDouble(), 1.0 - Tolerance, 1.0 + Tolerance);
        }
    }