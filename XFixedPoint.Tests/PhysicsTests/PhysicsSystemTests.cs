using XFixedPoint.Core;
using XFixedPoint.Physics;
using XFixedPoint.Physics.Collision;
using XFixedPoint.Vectors;

namespace XFixedPoint.Tests.PhysicsTests;

    public class PhysicsSystemTests
    {
        private const double Tolerance = 1e-5;

        [Fact]
        public void SingleBody_NoColliders_AppliesGravityAndIntegrates()
        {
            var physics = new PhysicsSystem();
            // 设置重力 (0, -9.81, 0)
            physics.Gravity = XFixedVector3.FromFloat(0f, -9.81f, 0f);

            var body = new FixedRigidbody();
            physics.AddBody(body);

            // 步长 dt = 0.5 秒
            var dt = XFixed.FromDouble(0.5);
            physics.Step(dt);

            // v = g * dt = -9.81 * 0.5 = -4.905
            Assert.InRange(body.Velocity.Y.ToDouble(), -4.905 - Tolerance, -4.905 + Tolerance);
            // x = v * dt = -4.905 * 0.5 = -2.4525
            Assert.InRange(body.Position.Y.ToDouble(), -2.4525 - Tolerance, -2.4525 + Tolerance);
        }

        [Fact]
        public void TwoSpheres_CollideAndBounce()
        {
            var physics = new PhysicsSystem();
            // 关闭重力，设置弹性系数 e = 0.5
            physics.Gravity     = XFixedVector3.Zero;
            physics.Restitution = XFixed.FromDouble(0.5);

            // 初始化两个刚体
            var bodyA = new FixedRigidbody();
            var bodyB = new FixedRigidbody();

            // 初始位置：A 在 0，B 在 1.5（半径 1 的球重叠 0.5）
            bodyA.Position = new XFixedVector3(XFixed.FromInt(0),    XFixed.Zero, XFixed.Zero);
            bodyB.Position = new XFixedVector3(XFixed.FromDouble(1.5), XFixed.Zero, XFixed.Zero);

            // 初始速度：A 向右 1，B 向左 1
            bodyA.Velocity = new XFixedVector3(XFixed.FromDouble(1),  XFixed.Zero, XFixed.Zero);
            bodyB.Velocity = new XFixedVector3(XFixed.FromDouble(-1), XFixed.Zero, XFixed.Zero);

            // 关联球形碰撞体
            var colA = new SphereCollider(XFixed.FromInt(1));
            var colB = new SphereCollider(XFixed.FromInt(1));
            physics.AddBody(bodyA, colA);
            physics.AddBody(bodyB, colB);

            // 使用小步长确保在下一步依然重叠
            var dt = XFixed.FromDouble(0.1);
            physics.Step(dt);

            // 根据弹性系数 0.5，碰撞后速度应为 ±0.5
            Assert.InRange(bodyA.Velocity.X.ToDouble(), -0.5 - Tolerance, -0.5 + Tolerance);
            Assert.InRange(bodyB.Velocity.X.ToDouble(),  0.5 - Tolerance,  0.5 + Tolerance);
        }
    }