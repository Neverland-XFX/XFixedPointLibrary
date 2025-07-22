using System.Collections.Generic;
using System.Linq;

namespace XFixedPoint.Networking
{
    /// <summary>
    /// 输入缓冲区：按 tick 存储并检索输入，并能清理旧数据
    /// </summary>
    public class InputBuffer<TInput>
    {
        private readonly SortedDictionary<int, TInput> _buffer = new SortedDictionary<int, TInput>();

        /// <summary>
        /// 添加或覆盖指定 tick 的输入
        /// </summary>
        public void AddInput(int tick, TInput input)
        {
            _buffer[tick] = input;
        }

        /// <summary>
        /// 尝试获取指定 tick 的输入
        /// </summary>
        public bool TryGetInput(int tick, out TInput input)
            => _buffer.TryGetValue(tick, out input);

        /// <summary>
        /// 获取指定 tick 的输入，若不存在则抛出异常
        /// </summary>
        public TInput GetInput(int tick)
        {
            if (!_buffer.TryGetValue(tick, out var input))
                throw new KeyNotFoundException($"No input for tick {tick}");
            return input;
        }

        /// <summary>
        /// 移除所有早于指定 tick 的输入
        /// </summary>
        public void RemoveOld(int tick)
        {
            var old = _buffer.Keys.Where(t => t < tick).ToList();
            foreach (var t in old)
                _buffer.Remove(t);
        }

        /// <summary>
        /// 已缓存的所有 tick（用于检测延迟输入）
        /// </summary>
        public IEnumerable<int> Ticks => _buffer.Keys;
    }
}