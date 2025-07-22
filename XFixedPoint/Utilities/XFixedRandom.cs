using XFixedPoint.Core;

namespace XFixedPoint.Utilities
{
    /// <summary>
    /// 线性同余法随机数生成器，输出定点随机值
    /// </summary>
    public class XFixedRandom
    {
        // 32 位种子
        private uint _seed;

        /// <summary>
        /// 构造，使用指定初始种子
        /// </summary>
        public XFixedRandom(uint seed) => _seed = seed;

        /// <summary>
        /// 获取下一个 32 位无符号随机数
        /// </summary>
        public uint NextUInt()
        {
            // 常见 LCG 参数
            _seed = _seed * 1664525u + 1013904223u;
            return _seed;
        }

        /// <summary>
        /// 获取 [0,1) 区间的定点随机值
        /// </summary>
        public XFixed NextFixed()
        {
            // 将 32 位随机数扩展到 Fixed 的 Raw 范围：[0, ONE)
            // Raw 格式为 32.32，高 32 位取随机数
            long raw = (long)NextUInt() << (XFixed.SHIFT - 32);
            return XFixed.FromRaw(raw);
        }

        /// <summary>
        /// 获取 [min, max) 之间的定点随机值
        /// </summary>
        public XFixed Range(XFixed min, XFixed max)
        {
            // 注意 NextFixed() ∈ [0,1)
            return min + (max - min) * NextFixed();
        }
    }
}