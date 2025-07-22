using XFixedPoint.Core;
using XFixedPoint.Vectors;

namespace XFixedPoint.Physics.Collision
{
    /// <summary>
    /// 球形碰撞体
    /// </summary>
    public class SphereCollider : FixedCollider
    {
        /// <summary>
        /// 球半径
        /// </summary>
        public XFixed Radius { get; set; }

        public SphereCollider(XFixed radius)
        {
            Radius = radius;
        }

        #region 重叠检测

        public override bool Overlaps(FixedCollider other)
        {
            if (other is SphereCollider b)
                return OverlapsSphereVsSphere(this, b);
            // 交给对方实现，返回时法线需翻转
            return other.Overlaps(this);
        }

        private static bool OverlapsSphereVsSphere(SphereCollider a, SphereCollider b)
        {
            var diff = b.WorldPosition - a.WorldPosition;
            var distSq = diff.Dot(diff);
            var rSum   = a.Radius + b.Radius;
            return distSq <= rSum * rSum;
        }

        #endregion

        #region 流形计算

        public override bool ComputeManifold(FixedCollider other, out CollisionManifold manifold)
        {
            if (other is SphereCollider b)
            {
                manifold = ComputeSphereVsSphere(this, b);
                return manifold.Colliding;
            }
            // 如果对方支持 Sphere vs Other，则反转法线
            if (other.ComputeManifold(this, out var m))
            {
                m.Normal = -m.Normal;
                manifold = m;
                return true;
            }

            manifold = default;
            return false;
        }

        private static CollisionManifold ComputeSphereVsSphere(SphereCollider a, SphereCollider b)
        {
            var centerA = a.WorldPosition;
            var centerB = b.WorldPosition;
            var diff     = centerB - centerA;
            var distSq   = diff.Dot(diff);
            var rSum     = a.Radius + b.Radius;

            if (distSq <= rSum * rSum)
            {
                var dist = XFixedMath.Sqrt(distSq);
                // 零距离时任意法线
                var normal = dist == XFixed.Zero
                    ? new XFixedVector3(XFixed.One, XFixed.Zero, XFixed.Zero)
                    : diff / dist;
                var penetration = rSum - dist;
                // 接触点：从 A 中心沿法线移至半径深度处
                var contact = centerA + normal * (a.Radius - penetration * XFixed.Half);

                return new CollisionManifold
                {
                    Colliding        = true,
                    Normal           = normal,
                    PenetrationDepth = penetration,
                    ContactPoint     = contact
                };
            }

            return new CollisionManifold { Colliding = false };
        }

        #endregion
    }
}