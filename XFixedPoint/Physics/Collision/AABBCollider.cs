using XFixedPoint.Core;
using XFixedPoint.Vectors;

namespace XFixedPoint.Physics.Collision
{
    /// <summary>
    /// 轴对齐包围盒碰撞体
    /// </summary>
    public class AABBCollider : FixedCollider
    {
        /// <summary>
        /// 包围盒半尺寸（沿 X/Y/Z 方向的半长）
        /// </summary>
        public XFixedVector3 HalfSize { get; set; }

        public AABBCollider(XFixedVector3 halfSize)
        {
            HalfSize = halfSize;
        }

        public override bool Overlaps(FixedCollider other)
        {
            if (other is AABBCollider b)
            {
                return OverlapsAABBvsAABB(this, b);
            }
            // 否则交给对方实现（并可能在对方里反转法线）
            return other.Overlaps(this);
        }

        private static bool OverlapsAABBvsAABB(AABBCollider a, AABBCollider b)
        {
            var amin = a.WorldPosition - a.HalfSize;
            var amax = a.WorldPosition + a.HalfSize;
            var bmin = b.WorldPosition - b.HalfSize;
            var bmax = b.WorldPosition + b.HalfSize;

            return
                amin.X <= bmax.X && amax.X >= bmin.X &&
                amin.Y <= bmax.Y && amax.Y >= bmin.Y &&
                amin.Z <= bmax.Z && amax.Z >= bmin.Z;
        }

        public override bool ComputeManifold(FixedCollider other, out CollisionManifold manifold)
        {
            if (other is AABBCollider b)
            {
                manifold = ComputeAABBvsAABB(this, b);
                return manifold.Colliding;
            }
            // 如果对方处理了 AABB vs Other 的情况，则反转法线
            if (other.ComputeManifold(this, out var m))
            {
                m.Normal = -m.Normal;
                manifold = m;
                return true;
            }

            manifold = default;
            return false;
        }

        private static CollisionManifold ComputeAABBvsAABB(AABBCollider a, AABBCollider b)
        {
            var amin = a.WorldPosition - a.HalfSize;
            var amax = a.WorldPosition + a.HalfSize;
            var bmin = b.WorldPosition - b.HalfSize;
            var bmax = b.WorldPosition + b.HalfSize;

            // 计算各轴穿透深度
            var dx = XFixedMath.Min(amax.X, bmax.X) - XFixedMath.Max(amin.X, bmin.X);
            var dy = XFixedMath.Min(amax.Y, bmax.Y) - XFixedMath.Max(amin.Y, bmin.Y);
            var dz = XFixedMath.Min(amax.Z, bmax.Z) - XFixedMath.Max(amin.Z, bmin.Z);

            bool colliding = dx > XFixed.Zero && dy > XFixed.Zero && dz > XFixed.Zero;
            if (!colliding)
            {
                return new CollisionManifold { Colliding = false };
            }

            // 选择最小穿透轴
            var penetration = dx;
            var normal = new XFixedVector3(XFixed.One, XFixed.Zero, XFixed.Zero);
            if (dy < penetration)
            {
                penetration = dy;
                normal = new XFixedVector3(XFixed.Zero, XFixed.One, XFixed.Zero);
            }
            if (dz < penetration)
            {
                penetration = dz;
                normal = new XFixedVector3(XFixed.Zero, XFixed.Zero, XFixed.One);
            }

            // 根据两中心位置决定法线指向（从 a 指向 b）
            var centerA = a.WorldPosition;
            var centerB = b.WorldPosition;
            if (normal.X != XFixed.Zero && (centerB.X - centerA.X) < XFixed.Zero)
                normal = new XFixedVector3(-XFixed.One, XFixed.Zero, XFixed.Zero);
            else if (normal.Y != XFixed.Zero && (centerB.Y - centerA.Y) < XFixed.Zero)
                normal = new XFixedVector3(XFixed.Zero, -XFixed.One, XFixed.Zero);
            else if (normal.Z != XFixed.Zero && (centerB.Z - centerA.Z) < XFixed.Zero)
                normal = new XFixedVector3(XFixed.Zero, XFixed.Zero, -XFixed.One);

            // 计算接触点：重叠区中点
            var contact = new XFixedVector3(
                (XFixedMath.Max(amin.X, bmin.X) + XFixedMath.Min(amax.X, bmax.X)) * XFixed.Half,
                (XFixedMath.Max(amin.Y, bmin.Y) + XFixedMath.Min(amax.Y, bmax.Y)) * XFixed.Half,
                (XFixedMath.Max(amin.Z, bmin.Z) + XFixedMath.Min(amax.Z, bmax.Z)) * XFixed.Half
            );

            return new CollisionManifold
            {
                Colliding        = true,
                Normal           = normal,
                PenetrationDepth = penetration,
                ContactPoint     = contact
            };
        }
    }
}