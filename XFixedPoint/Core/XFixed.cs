using System;

namespace XFixedPoint.Core
{
    /// <summary>
    /// 64 位定点数，内部用 long 存储，格式为 32.32（高 32 位整数，低 32 位小数）
    /// </summary>
    public readonly struct XFixed : IComparable<XFixed>, IEquatable<XFixed>
    {
        private readonly long _raw;

        /// <summary>
        /// 获取内部原始值（仅供高级调试或高级运算使用）
        /// </summary>
        public long Raw => _raw;

        /// <summary>
        /// 小数位数
        /// </summary>
        public const int SHIFT = XFixedConstants.SHIFT;

        /// <summary>
        /// 单位 1 的原始值
        /// </summary>
        public const long ONE = XFixedConstants.ONE;

        /// <summary>
        /// 定点 0
        /// </summary>
        public static readonly XFixed Zero = new XFixed(0);

        /// <summary>
        /// 定点 1
        /// </summary>
        public static readonly XFixed One = new XFixed(ONE);

        /// <summary>
        /// 定点 0.5
        /// </summary>
        public static readonly XFixed Half = new XFixed(XFixedConstants.HALF);

        /// <summary>
        /// 由原始值构造（私有），更推荐使用 FromXXX 工厂方法
        /// </summary>
        private XFixed(long raw) => _raw = raw;

        #region 构造 / From 工厂

        public static XFixed FromRaw(long raw)       => new XFixed(raw);
        public static XFixed FromInt(int v)          => new XFixed((long)v << SHIFT);
        public static XFixed FromLong(long v)        => new XFixed(v << SHIFT);
        public static XFixed FromFloat(float v)      => new XFixed((long)(v * ONE));
        public static XFixed FromDouble(double v)    => new XFixed((long)(v * ONE));

        #endregion

        #region 转换为原生类型

        public int    ToInt()    => (int)(_raw >> SHIFT);
        public long   ToLong()   =>    _raw >> SHIFT;
        public float  ToFloat()  => (float)_raw / ONE;
        public double ToDouble() => (double)_raw / ONE;

        #endregion
        
        public override string ToString()
            => ToDouble().ToString("G9"); // 保留足够精度

        public static XFixed operator -(XFixed a)
            => FromRaw(unchecked(-a._raw));

        public static XFixed operator +(XFixed a, XFixed b)    => XFixedArithmetic.Add(a, b);
        public static XFixed operator -(XFixed a, XFixed b)    => XFixedArithmetic.Subtract(a, b);
        public static XFixed operator *(XFixed a, XFixed b)    => XFixedArithmetic.Multiply(a, b);
        public static XFixed operator /(XFixed a, XFixed b)    => XFixedArithmetic.Divide(a, b);

        public static XFixed operator <<(XFixed a, int bits)  => new XFixed(a._raw << bits);
        public static XFixed operator >>(XFixed a, int bits)  => new XFixed(a._raw >> bits);

        public int CompareTo(XFixed other) 
            => XFixedComparison.Compare(this, other);

        public bool Equals(XFixed other)   
            => XFixedComparison.Equals(this, other);

        public override bool Equals(object? obj)
            => obj is XFixed f && Equals(f);

        public override int GetHashCode() 
            => Raw.GetHashCode();

        public static bool operator ==(XFixed a, XFixed b) 
            => XFixedComparison.Equals(a, b);

        public static bool operator !=(XFixed a, XFixed b) 
            => !XFixedComparison.Equals(a, b);

        public static bool operator <(XFixed a, XFixed b) 
            => XFixedComparison.LessThan(a, b);

        public static bool operator >(XFixed a, XFixed b) 
            => XFixedComparison.GreaterThan(a, b);

        public static bool operator <=(XFixed a, XFixed b) 
            => XFixedComparison.LessOrEqual(a, b);

        public static bool operator >=(XFixed a, XFixed b) 
            => XFixedComparison.GreaterOrEqual(a, b);

    }
}