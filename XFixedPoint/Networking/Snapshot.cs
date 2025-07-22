using System.Collections.Generic;
using System.IO;
using XFixedPoint.Core;
using XFixedPoint.Physics;
using XFixedPoint.Quaternions;
using XFixedPoint.Vectors;

namespace XFixedPoint.Networking
{
    /// <summary>
    /// 状态快照：保存一组刚体在特定 tick 下的完整状态，
    /// 严格按 bodies 顺序写入/读取，保证位级一致性。
    /// </summary>
    public class Snapshot
    {
        public int Tick { get; }
        private readonly byte[] _data;

        private Snapshot(int tick, byte[] data)
        {
            Tick = tick;
            _data = data;
        }

        /// <summary>
        /// 根据给定刚体列表生成快照（严格按 bodies 顺序写入数据）
        /// </summary>
        public static Snapshot Create(int tick, IList<FixedRigidbody> bodies)
        {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);
            foreach (var b in bodies)
            {
                // Position
                writer.Write(b.Position.X.Raw);
                writer.Write(b.Position.Y.Raw);
                writer.Write(b.Position.Z.Raw);
                // Rotation
                writer.Write(b.Rotation.X.Raw);
                writer.Write(b.Rotation.Y.Raw);
                writer.Write(b.Rotation.Z.Raw);
                writer.Write(b.Rotation.W.Raw);
                // Velocity
                writer.Write(b.Velocity.X.Raw);
                writer.Write(b.Velocity.Y.Raw);
                writer.Write(b.Velocity.Z.Raw);
                // AngularVelocity
                writer.Write(b.AngularVelocity.X.Raw);
                writer.Write(b.AngularVelocity.Y.Raw);
                writer.Write(b.AngularVelocity.Z.Raw);
            }
            return new Snapshot(tick, ms.ToArray());
        }

        /// <summary>
        /// 将快照的数据恢复到给定刚体列表（顺序必须与创建时相同）
        /// </summary>
        public void Restore(IList<FixedRigidbody> bodies)
        {
            using var ms = new MemoryStream(_data);
            using var reader = new BinaryReader(ms);
            foreach (var b in bodies)
            {
                // Position
                var px = reader.ReadInt64();
                var py = reader.ReadInt64();
                var pz = reader.ReadInt64();
                b.Position = new XFixedVector3(XFixed.FromRaw(px), XFixed.FromRaw(py), XFixed.FromRaw(pz));
                // Rotation
                var rx = reader.ReadInt64();
                var ry = reader.ReadInt64();
                var rz = reader.ReadInt64();
                var rw = reader.ReadInt64();
                b.Rotation = new XFixedQuaternion(
                    XFixed.FromRaw(rx),
                    XFixed.FromRaw(ry),
                    XFixed.FromRaw(rz),
                    XFixed.FromRaw(rw));
                // Velocity
                var vx = reader.ReadInt64();
                var vy = reader.ReadInt64();
                var vz = reader.ReadInt64();
                b.Velocity = new XFixedVector3(XFixed.FromRaw(vx), XFixed.FromRaw(vy), XFixed.FromRaw(vz));
                // AngularVelocity
                var ax = reader.ReadInt64();
                var ay = reader.ReadInt64();
                var az = reader.ReadInt64();
                b.AngularVelocity = new XFixedVector3(XFixed.FromRaw(ax), XFixed.FromRaw(ay), XFixed.FromRaw(az));
            }
        }
    }
}