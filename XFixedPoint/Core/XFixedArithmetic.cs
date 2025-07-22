using System;
using System.Numerics;

namespace XFixedPoint.Core
{
    /// <summary>
    /// 定点数算术运算（加减乘除）
    /// </summary>
    internal static class XFixedArithmetic
    {
        /// <summary>
        /// 加法：直接累加原始值
        /// </summary>
        public static XFixed Add(XFixed a, XFixed b)
            => XFixed.FromRaw(unchecked(a.Raw + b.Raw));

        /// <summary>
        /// 减法：直接相减原始值
        /// </summary>
        public static XFixed Subtract(XFixed a, XFixed b)
            => XFixed.FromRaw(unchecked(a.Raw - b.Raw));

        /// <summary>
        /// 乘法：使用 128 位中间精度，(a.Raw * b.Raw) >> SHIFT
        /// </summary>
        public static XFixed Multiply(XFixed a, XFixed b)
        {
            // 使用 BigInteger 保证中间不溢出
            BigInteger product = (BigInteger)a.Raw * b.Raw;
            long resultRaw = (long)(product >> XFixed.SHIFT);
            return XFixed.FromRaw(resultRaw);
        }

        /// <summary>
        /// 除法：先左移再除
        /// </summary>
        /// <exception cref="DivideByZeroException">b.Raw 为 0 时抛出</exception>
        public static XFixed Divide(XFixed a, XFixed b)
        {
            if (b.Raw == 0)
                throw new DivideByZeroException("Fixed division by zero");
            BigInteger dividend = (BigInteger)a.Raw << XFixed.SHIFT;
            BigInteger quotient = dividend / b.Raw;
            long resultRaw = (long)quotient;
            return XFixed.FromRaw(resultRaw);
        }
    }
}