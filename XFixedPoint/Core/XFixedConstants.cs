namespace XFixedPoint.Core
{
    /// <summary>
    /// 定点数库常量定义
    /// </summary>
    public static class XFixedConstants
    {
        /// <summary>
        /// 小数位数（Number of fractional bits）
        /// </summary>
        public const int SHIFT = 32;

        /// <summary>
        /// 单位 1 对应的原始值（1 << SHIFT）
        /// </summary>
        public const long ONE = 1L << SHIFT;

        /// <summary>
        /// 半单位（0.5），原始值形式
        /// </summary>
        public const long HALF = ONE >> 1;

        /// <summary>
        /// 最小增量（最小可表示的定点差值）
        /// </summary>
        public const long EPS = 1L;
    }
}