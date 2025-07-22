using XFixedPoint.Quaternions;
using XFixedPoint.Vectors;

namespace XFixedPoint.Physics.Collision
{
    /// <summary>
    /// 定点碰撞体基类。所有具体形状（AABB、Sphere、OBB 等）继承此类，
    /// 并实现自己的重叠检测与碰撞流形生成逻辑。
    /// </summary>
    public abstract class FixedCollider
    {
        /// <summary>
        /// 所属刚体（可为 null，表示静态或独立碰撞体）
        /// </summary>
        public FixedRigidbody Rigidbody { get; set; }

        /// <summary>
        /// 相对于刚体的局部偏移
        /// </summary>
        public XFixedVector3 LocalOffset { get; set; } = XFixedVector3.Zero;

        /// <summary>
        /// 相对于刚体的局部旋转
        /// </summary>
        public XFixedQuaternion LocalRotation { get; set; } = XFixedQuaternion.Identity;

        /// <summary>
        /// 全局位置 = (Rigidbody.Position) + LocalOffset 经过 Rigidbody.Rotation 与 LocalRotation 变换
        /// </summary>
        public XFixedVector3 WorldPosition
        {
            get
            {
                if (Rigidbody == null) return LocalOffset;
                // 先用刚体旋转，再用本地旋转，再加位置
                var rotated = Rigidbody.Rotation.Rotate(LocalOffset);
                return Rigidbody.Position + rotated;
            }
        }

        /// <summary>
        /// 全局朝向 = Rigidbody.Rotation * LocalRotation
        /// </summary>
        public XFixedQuaternion WorldRotation
            => Rigidbody == null
                ? LocalRotation
                : (Rigidbody.Rotation * LocalRotation).Normalized;

        /// <summary>
        /// 判断与另一个碰撞体是否重叠（仅检测，不生成流形）
        /// </summary>
        public abstract bool Overlaps(FixedCollider other);

        /// <summary>
        /// 若与 other 重叠，计算碰撞流形（法线、穿透深度、接触点）
        /// </summary>
        public abstract bool ComputeManifold(FixedCollider other, out CollisionManifold manifold);
    }
}