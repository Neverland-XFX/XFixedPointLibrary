using System.Collections.Generic;
using XFixedPoint.Core;
using XFixedPoint.Physics.Collision;
using XFixedPoint.Vectors;

namespace XFixedPoint.Physics
{
    /// <summary>
    /// 简单物理系统：管理刚体与碰撞体，执行积分与碰撞响应
    /// </summary>
    public class PhysicsSystem
    {
        /// <summary>所有注册的刚体</summary>
        private readonly List<FixedRigidbody> _bodies = new List<FixedRigidbody>();

        /// <summary>所有注册的碰撞体</summary>
        private readonly List<FixedCollider> _colliders = new List<FixedCollider>();

        /// <summary>碰撞恢复参数：弹性系数（0：完全非弹性，1：近似完全弹性）</summary>
        public XFixed Restitution { get; set; } = XFixed.FromFloat(0.5f);

        /// <summary>
        /// 全局重力向量，默认 (0, -9.81, 0)
        /// </summary>
        public XFixedVector3 Gravity { get; set; } = XFixedVector3.FromFloat(0f, -9.81f, 0f);

        /// <summary>
        /// 注册一个刚体及其碰撞体（可注册无碰撞体的纯动力学刚体）
        /// </summary>
        public void AddBody(FixedRigidbody body, FixedCollider collider = null)
        {
            if (!_bodies.Contains(body))
                _bodies.Add(body);
            if (collider != null && !_colliders.Contains(collider))
            {
                collider.Rigidbody = body;
                _colliders.Add(collider);
            }
        }

        /// <summary>
        /// 移除刚体及其碰撞体
        /// </summary>
        public void RemoveBody(FixedRigidbody body)
        {
            _bodies.Remove(body);
            _colliders.RemoveAll(c => c.Rigidbody == body);
        }

        /// <summary>
        /// 单步模拟：应用重力、积分 & 碰撞检测响应
        /// </summary>
        public void Step(XFixed dt)
        {
            if (dt == XFixed.Zero) return;

            // —— 1. 应用重力 —— 
            foreach (var body in _bodies)
            {
                if (body.IsKinematic || body.Mass == XFixed.Zero) continue;
                body.AddForce(Gravity * body.Mass);
            }

            // —— 2. 刚体积分 —— 
            foreach (var body in _bodies)
            {
                body.Integrate(dt);
            }

            // —— 3. 碰撞检测与响应 —— 
            int count = _colliders.Count;
            for (int i = 0; i < count; i++)
            {
                var A = _colliders[i];
                for (int j = i + 1; j < count; j++)
                {
                    var B = _colliders[j];
                    // 仅处理两个均有关联刚体，且至少一个非运动学刚体
                    if (A.Rigidbody == null && B.Rigidbody == null) continue;
                    if (A.Rigidbody?.IsKinematic == true && B.Rigidbody?.IsKinematic == true) continue;

                    if (A.ComputeManifold(B, out var manifold) && manifold.Colliding)
                    {
                        ResolveCollision(A, B, manifold);
                    }
                }
            }
        }

        /// <summary>
        /// 简单的冲量-反冲碰撞响应
        /// </summary>
        private void ResolveCollision(FixedCollider colA, FixedCollider colB, CollisionManifold m)
        {
            var a = colA.Rigidbody;
            var b = colB.Rigidbody;

            // 1. 分离矫正：按质量比例平移
            var totalInvMass = (a?.InverseMass ?? XFixed.Zero) + (b?.InverseMass ?? XFixed.Zero);
            if (totalInvMass != XFixed.Zero)
            {
                var correction = m.Normal * (m.PenetrationDepth / totalInvMass);
                if (a != null && !a.IsKinematic)
                    a.Position -= correction * a.InverseMass;
                if (b != null && !b.IsKinematic)
                    b.Position += correction * b.InverseMass;
            }

            // 2. 相对速度
            var velA = a?.Velocity ?? XFixedVector3.Zero;
            var velB = b?.Velocity ?? XFixedVector3.Zero;
            // 点速度近似为刚体线速度
            var rv = velB - velA;
            // 计算法向分量
            var velAlongNormal = rv.Dot(m.Normal);
            if (velAlongNormal > XFixed.Zero) 
                return; // 分离中或已分离，不处理

            // 3. 计算冲量标量 j
            var e = Restitution;
            var j = -(XFixed.One + e) * velAlongNormal / totalInvMass;

            // 4. 应用冲量
            var impulse = m.Normal * j;
            if (a != null && !a.IsKinematic)
                a.Velocity -= impulse * a.InverseMass;
            if (b != null && !b.IsKinematic)
                b.Velocity += impulse * b.InverseMass;
        }
    }
}