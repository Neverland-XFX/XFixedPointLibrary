using XFixedPoint.Core;
using XFixedPoint.Networking;
using XFixedPoint.Physics;
using XFixedPoint.Quaternions;
using XFixedPoint.Vectors;

namespace XFixedPoint.Tests.NetworkingTests;

public class SnapshotTests
{
    [Fact]
    public void CreateAndRestore_RestoresRigidbodyState()
    {
        // 准备一个刚体列表
        var body = new FixedRigidbody
        {
            Position        = new XFixedVector3(XFixed.FromInt(1), XFixed.FromInt(2), XFixed.FromInt(3)),
            Rotation        = XFixedQuaternion.FromAxisAngle(
                new XFixedVector3(XFixed.Zero, XFixed.Zero, XFixed.One),
                XFixed.FromDouble(0.5)),
            Velocity        = new XFixedVector3(XFixed.FromInt(4), XFixed.FromInt(5), XFixed.FromInt(6)),
            AngularVelocity = new XFixedVector3(XFixed.FromInt(7), XFixed.FromInt(8), XFixed.FromInt(9))
        };
        var bodies = new List<FixedRigidbody> { body };

        // 创建快照
        var snap = Snapshot.Create(tick: 42, bodies);

        // 修改刚体状态
        body.Position        = XFixedVector3.Zero;
        body.Rotation        = XFixedQuaternion.Identity;
        body.Velocity        = XFixedVector3.Zero;
        body.AngularVelocity = XFixedVector3.Zero;

        // 恢复快照
        snap.Restore(bodies);

        // 验证恢复正确
        Assert.Equal(1,     body.Position.X.ToInt());
        Assert.Equal(2,     body.Position.Y.ToInt());
        Assert.Equal(3,     body.Position.Z.ToInt());
        Assert.Equal(0.5,   body.Rotation.ToEulerAngles().Z.ToDouble(), 3); // 只有 Z=roll 非 0
        Assert.Equal(4,     body.Velocity.X.ToInt());
        Assert.Equal(5,     body.Velocity.Y.ToInt());
        Assert.Equal(6,     body.Velocity.Z.ToInt());
        Assert.Equal(7,     body.AngularVelocity.X.ToInt());
        Assert.Equal(8,     body.AngularVelocity.Y.ToInt());
        Assert.Equal(9,     body.AngularVelocity.Z.ToInt());
    }
}