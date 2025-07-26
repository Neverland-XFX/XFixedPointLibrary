// XFixedPoint/Networking/RollbackSystem.cs
using System;
using System.Collections.Generic;
using XFixedPoint.Core;
using XFixedPoint.Physics;
using XFixedPoint.Vectors;

namespace XFixedPoint.Networking
{
    /// <summary>
    /// 回滚系统：管理状态快照与多条输入，支持延迟输入的回滚重放
    /// </summary>
    public class RollbackSystem<TInput>
    {
        private readonly PhysicsSystem _physicsSystem;
        private readonly IList<FixedRigidbody> _bodies;
        private readonly InputBuffer<TInput> _inputBuffer = new InputBuffer<TInput>();
        private readonly Dictionary<int, Snapshot> _snapshots = new Dictionary<int, Snapshot>();
        private int _lastAppliedTick = -1;

        public RollbackSystem(PhysicsSystem physicsSystem, IList<FixedRigidbody> bodies)
        {
            _physicsSystem = physicsSystem;
            _bodies = bodies;
        }

        public void SubmitInput(int tick, TInput input)
        {
            _inputBuffer.AddInput(tick, input);
        }

        public void SaveSnapshot(int tick)
        {
            _snapshots[tick] = Snapshot.Create(tick, _bodies);
        }

        public void AdvanceTo(int targetTick, XFixed dt, Action<TInput> applyInput)
        {
            // 1) 检查有没有“延迟输入”落在已模拟帧之前
            int earliestLate = int.MaxValue;
            foreach (var t in _inputBuffer.Ticks)
                if (t <= _lastAppliedTick && t < earliestLate)
                    earliestLate = t;

            // 2) 如果有，就回滚到最早那帧
            if (earliestLate != int.MaxValue
                && _snapshots.TryGetValue(earliestLate, out var snap))
            {
                snap.Restore(_bodies);
                _lastAppliedTick = earliestLate - 1;
            }

            // 3) 从 (_lastAppliedTick+1) 模拟到 targetTick
            for (int tick = _lastAppliedTick + 1; tick <= targetTick; tick++)
            {
                // 3.1) 保存这一帧的快照
                SaveSnapshot(tick);

                // —— **重置所有刚体速度** —— 
                foreach (var body in _bodies)
                    body.Velocity = XFixedVector3.Zero;

                // 3.2) 取出这一帧所有输入，依次调用 applyInput
                if (_inputBuffer.TryGetInputs(tick, out var inputs))
                {
                    foreach (var inp in inputs)
                        applyInput(inp);
                }

                // 3.3) 物理步进
                _physicsSystem.Step(dt);

                _lastAppliedTick = tick;
            }

            // 4) 清理过旧快照和输入（只保留最近 200 帧，按需调整）
            Cleanup(targetTick, keepHistory: 200);
        }

        private void Cleanup(int currentTick, int keepHistory)
        {
            var oldSnaps = new List<int>();
            foreach (var k in _snapshots.Keys)
                if (k < currentTick - keepHistory)
                    oldSnaps.Add(k);
            foreach (var k in oldSnaps)
                _snapshots.Remove(k);

            _inputBuffer.RemoveOld(currentTick - keepHistory);
        }
    }
}
