using System;
using XFixedPoint.Core;

namespace XFixedPoint.Vectors
{
    /// <summary>
    /// 三维定点向量
    /// </summary>
    public readonly struct XFixedVector3 : IEquatable<XFixedVector3>
    {
        /// <summary>
        /// X 分量
        /// </summary>
        public readonly XFixed X;
        /// <summary>
        /// Y 分量
        /// </summary>
        public readonly XFixed Y;
        /// <summary>
        /// Z 分量
        /// </summary>
        public readonly XFixed Z;

        /// <summary>
        /// 零向量 (0,0,0)
        /// </summary>
        public static readonly XFixedVector3 Zero  = new XFixedVector3(XFixed.Zero,  XFixed.Zero,  XFixed.Zero);
        /// <summary>
        /// 单位向量 (1,1,1)
        /// </summary>
        public static readonly XFixedVector3 One   = new XFixedVector3(XFixed.One,   XFixed.One,   XFixed.One);
        /// <summary>
        /// 单位 X 轴向量 (1,0,0)
        /// </summary>
        public static readonly XFixedVector3 UnitX = new XFixedVector3(XFixed.One,   XFixed.Zero,  XFixed.Zero);
        /// <summary>
        /// 单位 Y 轴向量 (0,1,0)
        /// </summary>
        public static readonly XFixedVector3 UnitY = new XFixedVector3(XFixed.Zero,  XFixed.One,   XFixed.Zero);
        /// <summary>
        /// 单位 Z 轴向量 (0,0,1)
        /// </summary>
        public static readonly XFixedVector3 UnitZ = new XFixedVector3(XFixed.Zero,  XFixed.Zero,  XFixed.One);

        /// <summary>
        /// 构造三维定点向量
        /// </summary>
        public XFixedVector3(XFixed x, XFixed y, XFixed z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// 从浮点坐标构造
        /// </summary>
        public static XFixedVector3 FromFloat(float x, float y, float z)
            => new XFixedVector3(XFixed.FromFloat(x), XFixed.FromFloat(y), XFixed.FromFloat(z));

        // 运算符重载
        public static XFixedVector3 operator +(XFixedVector3 a, XFixedVector3 b)
            => new XFixedVector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        public static XFixedVector3 operator -(XFixedVector3 a, XFixedVector3 b)
            => new XFixedVector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        public static XFixedVector3 operator -(XFixedVector3 v)
            => new XFixedVector3(-v.X, -v.Y, -v.Z);

        public static XFixedVector3 operator *(XFixedVector3 v, XFixed s)
            => new XFixedVector3(v.X * s, v.Y * s, v.Z * s);

        public static XFixedVector3 operator *(XFixed s, XFixedVector3 v)
            => new XFixedVector3(v.X * s, v.Y * s, v.Z * s);
        
        /// <summary>
        /// 向量除以标量
        /// </summary>
        public static XFixedVector3 operator /(XFixedVector3 v, XFixed s)
            => new XFixedVector3(v.X / s, v.Y / s, v.Z / s);
        
        /// <summary>
        /// 点乘
        /// </summary>
        public XFixed Dot(XFixedVector3 other)
            => X * other.X + Y * other.Y + Z * other.Z;

        /// <summary>
        /// 叉乘
        /// </summary>
        public XFixedVector3 Cross(XFixedVector3 other)
            => new XFixedVector3(
                Y * other.Z - Z * other.Y,
                Z * other.X - X * other.Z,
                X * other.Y - Y * other.X
            );

        /// <summary>
        /// 平方长度
        /// </summary>
        public XFixed SqrMagnitude
            => Dot(this);

        /// <summary>
        /// 向量长度
        /// </summary>
        public XFixed Magnitude
            => XFixedMath.Sqrt(SqrMagnitude);

        /// <summary>
        /// 归一化向量。若长度为零，则返回零向量。
        /// </summary>
        public XFixedVector3 Normalized
        {
            get
            {
                var mag = Magnitude;
                return mag == XFixed.Zero
                    ? Zero
                    : new XFixedVector3(X / mag, Y / mag, Z / mag);
            }
        }

        /// <summary>
        /// 线性插值 a→b，t ∈ [0,1]
        /// </summary>
        public static XFixedVector3 Lerp(XFixedVector3 a, XFixedVector3 b, XFixed t)
        {
            t = XFixedMath.Clamp(t, XFixed.Zero, XFixed.One);
            return new XFixedVector3(
                a.X + (b.X - a.X) * t,
                a.Y + (b.Y - a.Y) * t,
                a.Z + (b.Z - a.Z) * t
            );
        }

        public bool Equals(XFixedVector3 other)
            => X == other.X && Y == other.Y && Z == other.Z;

        public override bool Equals(object obj)
            => obj is XFixedVector3 v && Equals(v);

        public override int GetHashCode()
            => HashCode.Combine(X.Raw, Y.Raw, Z.Raw);

        public static bool operator ==(XFixedVector3 a, XFixedVector3 b)
            => a.Equals(b);

        public static bool operator !=(XFixedVector3 a, XFixedVector3 b)
            => !a.Equals(b);

        public override string ToString()
            => $"({X.ToDouble():F4}, {Y.ToDouble():F4}, {Z.ToDouble():F4})";
    }
}