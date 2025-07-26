// XFixedPoint/Networking/InputBuffer.cs
using System.Collections.Generic;
using System.Linq;

namespace XFixedPoint.Networking
{
    /// <summary>
    /// 输入缓冲区：每个 tick 可以存多条输入，支持延迟输入不丢失
    /// </summary>
    public class InputBuffer<TInput>
    {
        // tick -> 当帧所有的输入列表
        private readonly SortedDictionary<int, List<TInput>> _buffer
            = new SortedDictionary<int, List<TInput>>();

        /// <summary>
        /// 添加一条输入到指定 tick（不会覆盖同帧已有输入）
        /// </summary>
        public void AddInput(int tick, TInput input)
        {
            if (!_buffer.TryGetValue(tick, out var list))
            {
                list = new List<TInput>();
                _buffer[tick] = list;
            }
            list.Add(input);
        }

        /// <summary>
        /// 尝试获取指定 tick 的所有输入列表
        /// </summary>
        public bool TryGetInputs(int tick, out List<TInput> inputs)
            => _buffer.TryGetValue(tick, out inputs);

        /// <summary>
        /// 移除所有早于 beforeTick 的输入列表
        /// </summary>
        public void RemoveOld(int beforeTick)
        {
            // 不能在遍历时修改字典，先 ToList
            var oldKeys = _buffer.Keys.Where(t => t < beforeTick).ToList();
            foreach (var t in oldKeys)
                _buffer.Remove(t);
        }

        /// <summary>
        /// 当前已缓存的所有 tick（用于检测延迟输入）
        /// </summary>
        public IEnumerable<int> Ticks => _buffer.Keys;
    }
}