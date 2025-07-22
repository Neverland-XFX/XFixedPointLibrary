using XFixedPoint.Core;
using XFixedPoint.Quaternions;
using XFixedPoint.Vectors;

namespace XFixedPoint.Physics.Collision
{
    /// <summary>
    /// 碰撞信息：在两碰撞体检测到重叠时，返回法线、穿透深度及一个接触点
    /// </summary>
    public struct CollisionManifold
    {
        /// <summary>
        /// 是否发生碰撞
        /// </summary>
        public bool Colliding;

        /// <summary>
        /// 碰撞法线，从 this 指向 other
        /// </summary>
        public XFixedVector3 Normal;

        /// <summary>
        /// 穿透深度（定点表示）
        /// </summary>
        public XFixed PenetrationDepth;

        /// <summary>
        /// 接触点（世界坐标）
        /// </summary>
        public XFixedVector3 ContactPoint;
    }
}