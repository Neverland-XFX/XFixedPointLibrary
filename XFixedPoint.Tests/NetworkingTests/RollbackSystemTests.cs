// using XFixedPoint.Core;
// using XFixedPoint.Networking;
// using XFixedPoint.Physics;
// using XFixedPoint.Vectors;
//
// namespace XFixedPoint.Tests.NetworkingTests;
//
//     public class RollbackSystemTests
//     {
//         private const double Tolerance = 1e-5;
//
//         [Fact]
//         public void AdvanceTo_AppliesInputs_NoDelay()
//         {
//             // 物理系统：无重力，无碰撞
//             var physics = new PhysicsSystem
//             {
//                 Gravity = XFixedVector3.Zero
//             };
//             var body = new FixedRigidbody();
//             physics.AddBody(body);
//
//             // RollbackSystem：输入类型为 FixedVector3 (力)
//             var rbList = new List<FixedRigidbody> { body };
//             var rollback = new RollbackSystem<XFixedVector3>(physics, rbList);
//
//             // 应用两个 tick 的输入
//             rollback.SubmitInput(1, new XFixedVector3(XFixed.FromInt(1), XFixed.Zero, XFixed.Zero));
//             rollback.SubmitInput(2, new XFixedVector3(XFixed.FromInt(2), XFixed.Zero, XFixed.Zero));
//
//             // 推进到 tick=2，dt=1s
//             rollback.AdvanceTo(targetTick: 2, dt: XFixed.One, applyInput: force =>
//             {
//                 body.AddForce(force);
//             });
//
//             // Tick1: force=1 -> a=1, v1=1, x1=1
//             // Tick2: force=2 -> a=2, v2=1+2=3, x2=1+3=4
//             Assert.InRange(body.Position.X.ToDouble(), 4.0 - Tolerance, 4.0 + Tolerance);
//             Assert.InRange(body.Velocity.X.ToDouble(), 3.0 - Tolerance, 3.0 + Tolerance);
//         }
//
//         [Fact]
//         public void AdvanceTo_WithDelayedInput_RollsBackAndReplays()
//         {
//             // 物理系统：无重力
//             var physics = new PhysicsSystem
//             {
//                 Gravity = XFixedVector3.Zero
//             };
//             var body = new FixedRigidbody();
//             physics.AddBody(body);
//
//             var rbList = new List<FixedRigidbody> { body };
//             var rollback = new RollbackSystem<XFixedVector3>(physics, rbList);
//
//             // 第一次推进到 tick=2，未提交任何输入
//             rollback.AdvanceTo(targetTick: 2, dt: XFixed.One, applyInput: force =>
//             {
//                 body.AddForce(force);
//             });
//             // 此时 body.Position = 0
//
//             // 延迟提交 tick=1 的输入
//             rollback.SubmitInput(1, new XFixedVector3(XFixed.FromInt(1), XFixed.Zero, XFixed.Zero));
//
//             // 再次推进到 tick=2，触发回滚：重演 tick1, tick2
//             rollback.AdvanceTo(targetTick: 2, dt: XFixed.One, applyInput: force =>
//             {
//                 body.AddForce(force);
//             });
//
//             // 重演结果：Tick1: force=1 -> v1=1, x1=1; Tick2: no force -> v2=1, x2=2
//             Assert.InRange(body.Position.X.ToDouble(), 2.0 - Tolerance, 2.0 + Tolerance);
//             Assert.InRange(body.Velocity.X.ToDouble(), 1.0 - Tolerance, 1.0 + Tolerance);
//         }
//     }