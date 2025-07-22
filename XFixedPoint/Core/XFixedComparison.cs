namespace XFixedPoint.Core
{
    /// <summary>
    /// 定点数比较运算（IComparable / IEquatable 支撑 + 运算符重载用）
    /// </summary>
    internal static class XFixedComparison
    {
        /// <summary>
        /// 相等比较
        /// </summary>
        public static bool Equals(XFixed a, XFixed b) 
            => a.Raw == b.Raw;

        /// <summary>
        /// 比较大小：返回 -1、0、+1
        /// </summary>
        public static int Compare(XFixed a, XFixed b) 
            => a.Raw.CompareTo(b.Raw);

        /// <summary>
        /// 小于
        /// </summary>
        public static bool LessThan(XFixed a, XFixed b) 
            => a.Raw < b.Raw;

        /// <summary>
        /// 大于
        /// </summary>
        public static bool GreaterThan(XFixed a, XFixed b) 
            => a.Raw > b.Raw;

        /// <summary>
        /// 小于等于
        /// </summary>
        public static bool LessOrEqual(XFixed a, XFixed b) 
            => a.Raw <= b.Raw;

        /// <summary>
        /// 大于等于
        /// </summary>
        public static bool GreaterOrEqual(XFixed a, XFixed b) 
            => a.Raw >= b.Raw;
    }
}