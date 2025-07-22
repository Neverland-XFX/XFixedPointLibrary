using XFixedPoint.Core;
using XFixedPoint.Quaternions;
using XFixedPoint.Vectors;

namespace XFixedPoint.Physics
{
    /// <summary>
    /// 定点刚体，支持线性与角运动的积分
    /// </summary>
    public class FixedRigidbody
    {
        /// <summary>
        /// 位置（世界坐标）
        /// </summary>
        public XFixedVector3 Position;

        /// <summary>
        /// 旋转（世界坐标）
        /// </summary>
        public XFixedQuaternion Rotation;

        /// <summary>
        /// 线速度
        /// </summary>
        public XFixedVector3 Velocity;

        /// <summary>
        /// 角速度（以弧度/秒为量纲的向量）
        /// </summary>
        public XFixedVector3 AngularVelocity;

        /// <summary>
        /// 质量
        /// </summary>
        public XFixed Mass
        {
            get => _mass;
            set
            {
                _mass = value;
                _invMass = (value == XFixed.Zero) ? XFixed.Zero : XFixed.One / value;
            }
        }
        private XFixed _mass = XFixed.One;

        /// <summary>
        /// 质量的倒数（0 表示无限大质量/静态物体）
        /// </summary>
        public XFixed InverseMass => _invMass;
        private XFixed _invMass = XFixed.One;

        /// <summary>
        /// 是否为运动学刚体（不受力学影响，仅跟随 Position/Rotation）
        /// </summary>
        public bool IsKinematic = false;

        // 积累本帧的外力与外力矩
        private XFixedVector3 _forceAccumulator = XFixedVector3.Zero;
        private XFixedVector3 _torqueAccumulator = XFixedVector3.Zero;

        /// <summary>
        /// 在本帧添加应用于质心的力（牛顿）
        /// </summary>
        public void AddForce(XFixedVector3 force)
        {
            _forceAccumulator += force;
        }

        /// <summary>
        /// 在本帧添加应用于质心的力矩（扭矩，牛·米）
        /// </summary>
        public void AddTorque(XFixedVector3 torque)
        {
            _torqueAccumulator += torque;
        }

        /// <summary>
        /// 清除累积的力和力矩，通常在每次 Integrate 结束后调用
        /// </summary>
        public void ClearAccumulators()
        {
            _forceAccumulator = XFixedVector3.Zero;
            _torqueAccumulator = XFixedVector3.Zero;
        }

        /// <summary>
        /// 显式欧拉积分：更新速度、位置，以及角速度、旋转
        /// </summary>
        /// <param name="dt">积分步长（秒），定点表示</param>
        public void Integrate(XFixed dt)
        {
            if (IsKinematic || dt == XFixed.Zero || Mass == XFixed.Zero)
            {
                ClearAccumulators();
                return;
            }

            // —— 线性运动 —— 
            // 线加速度 a = F / m
            var acceleration = _forceAccumulator * InverseMass;
            // v += a * dt
            Velocity += acceleration * dt;
            // x += v * dt
            Position += Velocity * dt;

            // —— 角运动 —— 
            // 简化处理：不做惯性张量，直接用“角加速度 = torque × InvMass”
            var angularAcc = _torqueAccumulator * InverseMass;
            // ω += α * dt
            AngularVelocity += angularAcc * dt;

            // 更新旋转：使用增量轴角近似
            var ωmag = AngularVelocity.Magnitude;
            if (ωmag != XFixed.Zero)
            {
                // 轴 = ω / |ω|，角度 = |ω| * dt
                var axis = AngularVelocity / ωmag;
                var angle = ωmag * dt;
                var deltaQ = XFixedQuaternion.FromAxisAngle(axis, angle);
                Rotation = (deltaQ * Rotation).Normalized;
            }

            // 清除累积，以备下一帧
            ClearAccumulators();
        }
    }
}