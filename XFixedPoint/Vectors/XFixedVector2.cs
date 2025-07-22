using System;
using XFixedPoint.Core;

namespace XFixedPoint.Vectors
{
    /// <summary>
    /// 二维定点向量
    /// </summary>
    public readonly struct XFixedVector2 : IEquatable<XFixedVector2>
    {
        /// <summary>
        /// 横坐标
        /// </summary>
        public readonly XFixed X;
        /// <summary>
        /// 纵坐标
        /// </summary>
        public readonly XFixed Y;

        /// <summary>
        /// 默认构造：(0, 0)
        /// </summary>
        public static readonly XFixedVector2 Zero = new XFixedVector2(XFixed.Zero, XFixed.Zero);
        /// <summary>
        /// 单位向量：(1, 1)
        /// </summary>
        public static readonly XFixedVector2 One  = new XFixedVector2(XFixed.One,  XFixed.One);

        /// <summary>
        /// 构造一个定点向量
        /// </summary>
        public XFixedVector2(XFixed x, XFixed y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// 从浮点坐标构造
        /// </summary>
        public static XFixedVector2 FromFloat(float x, float y)
            => new XFixedVector2(XFixed.FromFloat(x), XFixed.FromFloat(y));

        /// <summary>
        /// 两个向量相加
        /// </summary>
        public static XFixedVector2 operator +(XFixedVector2 a, XFixedVector2 b)
            => new XFixedVector2(a.X + b.X, a.Y + b.Y);

        /// <summary>
        /// 两个向量相减
        /// </summary>
        public static XFixedVector2 operator -(XFixedVector2 a, XFixedVector2 b)
            => new XFixedVector2(a.X - b.X, a.Y - b.Y);

        /// <summary>
        /// 向量取反
        /// </summary>
        public static XFixedVector2 operator -(XFixedVector2 v)
            => new XFixedVector2(-v.X, -v.Y);

        /// <summary>
        /// 向量乘以标量
        /// </summary>
        public static XFixedVector2 operator *(XFixedVector2 v, XFixed s)
            => new XFixedVector2(v.X * s, v.Y * s);

        /// <summary>
        /// 标量乘以向量
        /// </summary>
        public static XFixedVector2 operator *(XFixed s, XFixedVector2 v)
            => new XFixedVector2(v.X * s, v.Y * s);

        /// <summary>
        /// 点乘
        /// </summary>
        public XFixed Dot(XFixedVector2 other)
            => X * other.X + Y * other.Y;

        /// <summary>
        /// 二维向量叉积的“伪标量”结果（等于 X1·Y2 − Y1·X2）
        /// </summary>
        public XFixed Cross(XFixedVector2 other)
            => X * other.Y - Y * other.X;

        /// <summary>
        /// 向量平方长度
        /// </summary>
        public XFixed SqrMagnitude 
            => Dot(this);

        /// <summary>
        /// 向量长度（模）
        /// </summary>
        public XFixed Magnitude 
            => XFixedMath.Sqrt(SqrMagnitude);

        /// <summary>
        /// 单位化向量。如零向量则返回零。
        /// </summary>
        public XFixedVector2 Normalized
        {
            get
            {
                var mag = Magnitude;
                return mag == XFixed.Zero
                    ? Zero
                    : new XFixedVector2(X / mag, Y / mag);
            }
        }

        /// <summary>
        /// 线性插值 a→b，t ∈ [0,1]
        /// </summary>
        public static XFixedVector2 Lerp(XFixedVector2 a, XFixedVector2 b, XFixed t)
            => a + (b - a) * XFixedMath.Clamp(t, XFixed.Zero, XFixed.One);

        /// <summary>
        /// 两个向量是否相等
        /// </summary>
        public bool Equals(XFixedVector2 other)
            => X == other.X && Y == other.Y;

        public override bool Equals(object obj)
            => obj is XFixedVector2 v && Equals(v);

        public override int GetHashCode()
            => HashCode.Combine(X.Raw, Y.Raw);

        public static bool operator ==(XFixedVector2 a, XFixedVector2 b)
            => a.Equals(b);

        public static bool operator !=(XFixedVector2 a, XFixedVector2 b)
            => !a.Equals(b);

        public override string ToString()
            => $"({X.ToDouble():F4}, {Y.ToDouble():F4})";
    }
}