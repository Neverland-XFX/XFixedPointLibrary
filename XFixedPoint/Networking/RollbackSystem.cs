using System;
using System.Collections.Generic;
using XFixedPoint.Core;
using XFixedPoint.Physics;

namespace XFixedPoint.Networking
{
    /// <summary>
    /// 回滚系统：管理状态快照与输入，支持延迟输入的回滚重放
    /// </summary>
    public class RollbackSystem<TInput>
    {
        private readonly PhysicsSystem _physicsSystem;
        private readonly IList<FixedRigidbody> _bodies;
        private readonly InputBuffer<TInput> _inputBuffer = new InputBuffer<TInput>();
        private readonly Dictionary<int, Snapshot> _snapshots = new Dictionary<int, Snapshot>();

        private int _lastAppliedTick = -1;

        /// <summary>
        /// 构造：需要物理系统与刚体列表（顺序决定快照读写顺序）
        /// </summary>
        public RollbackSystem(PhysicsSystem physicsSystem, IList<FixedRigidbody> bodies)
        {
            _physicsSystem = physicsSystem;
            _bodies = bodies;
        }

        /// <summary>
        /// 提交指定 tick 的输入
        /// </summary>
        public void SubmitInput(int tick, TInput input)
        {
            _inputBuffer.AddInput(tick, input);
        }

        /// <summary>
        /// 保存当前 tick 的状态快照
        /// </summary>
        public void SaveSnapshot(int tick)
        {
            _snapshots[tick] = Snapshot.Create(tick, _bodies);
        }

        /// <summary>
        /// 按顺序推进模拟到 targetTick，自动处理回滚/重放
        /// </summary>
        /// <param name="targetTick">要模拟到的最新 tick</param>
        /// <param name="dt">每 tick 的固定步长</param>
        /// <param name="applyInput">在每个 tick 前调用的输入应用函数</param>
        public void AdvanceTo(int targetTick, XFixed dt, Action<TInput> applyInput)
        {
            // 1. 检查是否有延迟输入发生在已应用过的 tick 之前
            int earliestLate = int.MaxValue;
            foreach (var t in _inputBuffer.Ticks)
            {
                if (t <= _lastAppliedTick && t < earliestLate)
                    earliestLate = t;
            }

            // 2. 如果有延迟输入，回滚到相应快照
            if (earliestLate != int.MaxValue && _snapshots.TryGetValue(earliestLate, out var snap))
            {
                snap.Restore(_bodies);
                _lastAppliedTick = earliestLate - 1;
            }

            // 3. 从 (_lastAppliedTick+1) 模拟到 targetTick
            for (int tick = _lastAppliedTick + 1; tick <= targetTick; tick++)
            {
                // 3.1 保存快照
                SaveSnapshot(tick);
                // 3.2 应用输入（若有）
                if (_inputBuffer.TryGetInput(tick, out var inp))
                    applyInput(inp);
                // 3.3 物理步进
                _physicsSystem.Step(dt);
                _lastAppliedTick = tick;
            }

            // 4. 清理过旧的快照和输入（可选：只保留最近 N 帧）
            Cleanup(targetTick, keepHistory: 200);
        }

        private void Cleanup(int currentTick, int keepHistory)
        {
            // 删除过旧快照
            var oldSnaps = new List<int>();
            foreach (var k in _snapshots.Keys)
                if (k < currentTick - keepHistory)
                    oldSnaps.Add(k);
            foreach (var k in oldSnaps)
                _snapshots.Remove(k);

            // 删除过旧输入
            _inputBuffer.RemoveOld(currentTick - keepHistory);
        }
    }
}