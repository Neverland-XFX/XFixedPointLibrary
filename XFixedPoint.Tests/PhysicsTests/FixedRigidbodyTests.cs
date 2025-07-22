using XFixedPoint.Core;
using XFixedPoint.Physics;
using XFixedPoint.Vectors;

namespace XFixedPoint.Tests.PhysicsTests;

public class FixedRigidbodyTests
{
    private const double Tolerance = 1e-5;

    [Fact]
    public void Integrate_NoForce_NoMovement()
    {
        var body = new FixedRigidbody();
        // 初始位置与速度都为零
        Assert.Equal(XFixedVector3.Zero, body.Position);
        Assert.Equal(XFixedVector3.Zero, body.Velocity);

        // Integrate 不应改变
        body.Integrate(XFixed.One);
        Assert.Equal(XFixedVector3.Zero, body.Position);
        Assert.Equal(XFixedVector3.Zero, body.Velocity);
    }

    [Fact]
    public void Integrate_WithConstantForce_UpdatesPositionAndVelocity()
    {
        var body = new FixedRigidbody
        {
            Mass = XFixed.FromDouble(2.0)  // 质量为 2
        };
        // 施加沿 X 方向恒力 F = 4
        var force = new XFixedVector3(XFixed.FromDouble(4.0), XFixed.Zero, XFixed.Zero);
        body.AddForce(force);

        // 用 dt = 1 秒
        var dt = XFixed.One;
        body.Integrate(dt);

        // 加速度 a = F / m = 4 / 2 = 2
        // 速度 v = a * dt = 2 * 1 = 2
        // 位移 x = v * dt = 2 * 1 = 2
        Assert.InRange(body.Velocity.X.ToDouble(), 2.0 - Tolerance, 2.0 + Tolerance);
        Assert.InRange(body.Position.X.ToDouble(), 2.0 - Tolerance, 2.0 + Tolerance);

        // 其它分量保持 0
        Assert.InRange(body.Velocity.Y.ToDouble(), 0.0 - Tolerance, 0.0 + Tolerance);
        Assert.InRange(body.Position.Y.ToDouble(), 0.0 - Tolerance, 0.0 + Tolerance);
        Assert.InRange(body.Velocity.Z.ToDouble(), 0.0 - Tolerance, 0.0 + Tolerance);
        Assert.InRange(body.Position.Z.ToDouble(), 0.0 - Tolerance, 0.0 + Tolerance);
    }
}