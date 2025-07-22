using XFixedPoint.Core;
using XFixedPoint.Vectors;

namespace XFixedPoint.Physics.Collision
{
    /// <summary>
    /// 有向包围盒（OBB）碰撞体
    /// </summary>
    public class OBBCollider : FixedCollider
    {
        /// <summary>
        /// 盒子在局部空间中的半尺寸
        /// </summary>
        public XFixedVector3 HalfSize { get; set; }

        public OBBCollider(XFixedVector3 halfSize)
        {
            HalfSize = halfSize;
        }

        public override bool Overlaps(FixedCollider other)
        {
            if (other is OBBCollider b)
                return ComputeOBBvsOBB(this, b).Colliding;
            // 交给对方处理，再翻转法线
            return other.Overlaps(this);
        }

        public override bool ComputeManifold(FixedCollider other, out CollisionManifold manifold)
        {
            if (other is OBBCollider b)
            {
                manifold = ComputeOBBvsOBB(this, b);
                return manifold.Colliding;
            }
            // 如果对方支持 OBB vs Other，则翻转法线
            if (other.ComputeManifold(this, out var m))
            {
                m.Normal = -m.Normal;
                manifold = m;
                return true;
            }

            manifold = default;
            return false;
        }

        private static CollisionManifold ComputeOBBvsOBB(OBBCollider a, OBBCollider b)
        {
            // 1. 准备中心、轴与半尺寸
            var centerA = a.WorldPosition;
            var centerB = b.WorldPosition;
            var halfA = a.HalfSize;
            var halfB = b.HalfSize;

            var axesA = new XFixedVector3[3]
            {
                a.WorldRotation.Rotate(XFixedVector3.UnitX),
                a.WorldRotation.Rotate(XFixedVector3.UnitY),
                a.WorldRotation.Rotate(XFixedVector3.UnitZ)
            };
            var axesB = new XFixedVector3[3]
            {
                b.WorldRotation.Rotate(XFixedVector3.UnitX),
                b.WorldRotation.Rotate(XFixedVector3.UnitY),
                b.WorldRotation.Rotate(XFixedVector3.UnitZ)
            };

            // 2. 构建旋转矩阵 R 和其绝对值 absR（加一个微小 eps）
            var R    = new XFixed[3,3];
            var absR = new XFixed[3,3];
            var eps  = XFixed.FromRaw(XFixedConstants.EPS);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    R[i,j]    = axesA[i].Dot(axesB[j]);
                    absR[i,j] = XFixedMath.Abs(R[i,j]) + eps;
                }
            }

            // 3. 计算中心差向量在 A 轴与 B 轴的投影
            var tVec = centerB - centerA;
            var tA   = new XFixed[3];
            var tB   = new XFixed[3];
            for (int i = 0; i < 3; i++)
            {
                tA[i] = tVec.Dot(axesA[i]);
                tB[i] = tVec.Dot(axesB[i]);
            }

            // 4. SAT 测试，跟踪最小穿透深度与对应法线
            XFixed bestPen = XFixed.Zero;
            XFixedVector3 bestAxis = XFixedVector3.Zero;
            bool found = false;

            // 4.1 A 的本地轴
            for (int i = 0; i < 3; i++)
            {
                var ra = i == 0 ? halfA.X : (i == 1 ? halfA.Y : halfA.Z);
                var rb = halfB.X * absR[i,0] + halfB.Y * absR[i,1] + halfB.Z * absR[i,2];
                var penetration = (ra + rb) - XFixedMath.Abs(tA[i]);
                if (penetration < XFixed.Zero)
                    return new CollisionManifold { Colliding = false };

                var axis = axesA[i] * (tA[i] < XFixed.Zero ? -XFixed.One : XFixed.One);
                if (!found || penetration < bestPen)
                {
                    bestPen = penetration;
                    bestAxis = axis;
                    found = true;
                }
            }

            // 4.2 B 的本地轴
            for (int j = 0; j < 3; j++)
            {
                var ra = halfA.X * absR[0,j] + halfA.Y * absR[1,j] + halfA.Z * absR[2,j];
                var rb = j == 0 ? halfB.X : (j == 1 ? halfB.Y : halfB.Z);
                var penetration = (ra + rb) - XFixedMath.Abs(tB[j]);
                if (penetration < XFixed.Zero)
                    return new CollisionManifold { Colliding = false };

                var axis = axesB[j] * (tB[j] < XFixed.Zero ? -XFixed.One : XFixed.One);
                if (!found || penetration < bestPen)
                {
                    bestPen = penetration;
                    bestAxis = axis;
                    found = true;
                }
            }

            // 4.3 交叉轴 A_i × B_j
            for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
            {
                int k = (i + 1) % 3, l = (i + 2) % 3;
                int m = (j + 1) % 3, n = (j + 2) % 3;

                var ra = (k == 0 ? halfA.X : (k == 1 ? halfA.Y : halfA.Z)) * absR[l,j]
                       + (l == 0 ? halfA.X : (l == 1 ? halfA.Y : halfA.Z)) * absR[k,j];
                var rb = (m == 0 ? halfB.X : (m == 1 ? halfB.Y : halfB.Z)) * absR[i,n]
                       + (n == 0 ? halfB.X : (n == 1 ? halfB.Y : halfB.Z)) * absR[i,m];

                var dist = XFixedMath.Abs(tA[l] * R[k,j] - tA[k] * R[l,j]);
                if (dist > ra + rb)
                    return new CollisionManifold { Colliding = false };

                var penetration = (ra + rb) - dist;
                var axisTemp = axesA[i].Cross(axesB[j]);
                if (axisTemp == XFixedVector3.Zero)
                    continue;

                var axisNorm = axisTemp / axisTemp.Magnitude;
                var sign = (tVec.Dot(axisTemp) < XFixed.Zero) ? -XFixed.One : XFixed.One;
                var axis    = axisNorm * sign;

                if (!found || penetration < bestPen)
                {
                    bestPen = penetration;
                    bestAxis = axis;
                    found = true;
                }
            }

            // 5. 计算接触点：A 在 -normal 方向上的支撑点和 B 在 normal 方向上的支撑点中点
            var supportA = Support(a, -bestAxis);
            var supportB = Support(b,  bestAxis);
            var contact  = (supportA + supportB) * XFixed.Half;

            return new CollisionManifold
            {
                Colliding        = true,
                Normal           = bestAxis,
                PenetrationDepth = bestPen,
                ContactPoint     = contact
            };
        }

        /// <summary>
        /// OBB 在给定方向 dir 上的支撑点（世界坐标）
        /// </summary>
        private static XFixedVector3 Support(OBBCollider obb, XFixedVector3 dir)
        {
            var c    = obb.WorldPosition;
            var axes = new XFixedVector3[3]
            {
                obb.WorldRotation.Rotate(XFixedVector3.UnitX),
                obb.WorldRotation.Rotate(XFixedVector3.UnitY),
                obb.WorldRotation.Rotate(XFixedVector3.UnitZ)
            };

            // 沿每个轴选择正负半尺寸
            var p = c;
            p += axes[0] * (dir.Dot(axes[0]) >= XFixed.Zero ? obb.HalfSize.X : -obb.HalfSize.X);
            p += axes[1] * (dir.Dot(axes[1]) >= XFixed.Zero ? obb.HalfSize.Y : -obb.HalfSize.Y);
            p += axes[2] * (dir.Dot(axes[2]) >= XFixed.Zero ? obb.HalfSize.Z : -obb.HalfSize.Z);
            return p;
        }
    }
}