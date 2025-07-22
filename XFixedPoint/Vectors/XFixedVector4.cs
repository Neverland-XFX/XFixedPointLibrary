using System;
using XFixedPoint.Core;

namespace XFixedPoint.Vectors
{
    /// <summary>
    /// 四维定点向量，常用于齐次坐标与矩阵变换
    /// </summary>
    public readonly struct XFixedVector4 : IEquatable<XFixedVector4>
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
        /// W 分量（齐次坐标）
        /// </summary>
        public readonly XFixed W;

        /// <summary>
        /// 零向量 (0,0,0,0)
        /// </summary>
        public static readonly XFixedVector4 Zero   = new XFixedVector4(XFixed.Zero, XFixed.Zero, XFixed.Zero, XFixed.Zero);
        /// <summary>
        /// 单位向量 (1,1,1,1)
        /// </summary>
        public static readonly XFixedVector4 One    = new XFixedVector4(XFixed.One,  XFixed.One,  XFixed.One,  XFixed.One);
        /// <summary>
        /// 单位 W 轴向量 (0,0,0,1)
        /// </summary>
        public static readonly XFixedVector4 UnitW  = new XFixedVector4(XFixed.Zero, XFixed.Zero, XFixed.Zero, XFixed.One);

        /// <summary>
        /// 构造四维向量
        /// </summary>
        public XFixedVector4(XFixed x, XFixed y, XFixed z, XFixed w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// 从浮点坐标构造
        /// </summary>
        public static XFixedVector4 FromFloat(float x, float y, float z, float w)
            => new XFixedVector4(
                XFixed.FromFloat(x),
                XFixed.FromFloat(y),
                XFixed.FromFloat(z),
                XFixed.FromFloat(w)
            );

        // 加减运算
        public static XFixedVector4 operator +(XFixedVector4 a, XFixedVector4 b)
            => new XFixedVector4(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);

        public static XFixedVector4 operator -(XFixedVector4 a, XFixedVector4 b)
            => new XFixedVector4(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);

        public static XFixedVector4 operator -(XFixedVector4 v)
            => new XFixedVector4(-v.X, -v.Y, -v.Z, -v.W);

        // 标量乘法
        public static XFixedVector4 operator *(XFixedVector4 v, XFixed s)
            => new XFixedVector4(v.X * s, v.Y * s, v.Z * s, v.W * s);

        public static XFixedVector4 operator *(XFixed s, XFixedVector4 v)
            => v * s;

        /// <summary>
        /// 四维点乘（Dot）。
        /// </summary>
        public XFixed Dot(XFixedVector4 other)
            => X * other.X + Y * other.Y + Z * other.Z + W * other.W;

        /// <summary>
        /// 四维向量的平方长度。
        /// </summary>
        public XFixed SqrMagnitude 
            => Dot(this);

        /// <summary>
        /// 四维向量的长度（Euclidean norm）。
        /// </summary>
        public XFixed Magnitude
            => XFixedMath.Sqrt(SqrMagnitude);

        /// <summary>
        /// 归一化：将向量缩放到单位长度。若原长为零，则返回 Zero。
        /// </summary>
        public XFixedVector4 Normalized
        {
            get
            {
                var mag = Magnitude;
                return mag == XFixed.Zero
                    ? Zero
                    : new XFixedVector4(X / mag, Y / mag, Z / mag, W / mag);
            }
        }

        /// <summary>
        /// 齐次除法：将三维部分除以 W 分量，返回 (X/W, Y/W, Z/W, 1)。
        /// </summary>
        public XFixedVector4 Homogenized
        {
            get
            {
                if (W == XFixed.Zero)
                    return new XFixedVector4(X, Y, Z, W); // 不做除法，保持原值
                var invW = XFixed.One / W;
                return new XFixedVector4(X * invW, Y * invW, Z * invW, XFixed.One);
            }
        }

        /// <summary>
        /// 线性插值 a→b，t ∈ [0,1]
        /// </summary>
        public static XFixedVector4 Lerp(XFixedVector4 a, XFixedVector4 b, XFixed t)
        {
            t = XFixedMath.Clamp(t, XFixed.Zero, XFixed.One);
            return new XFixedVector4(
                a.X + (b.X - a.X) * t,
                a.Y + (b.Y - a.Y) * t,
                a.Z + (b.Z - a.Z) * t,
                a.W + (b.W - a.W) * t
            );
        }

        #region 相等与哈希

        public bool Equals(XFixedVector4 other)
            => X == other.X && Y == other.Y && Z == other.Z && W == other.W;

        public override bool Equals(object obj)
            => obj is XFixedVector4 v && Equals(v);

        public override int GetHashCode()
            => HashCode.Combine(X.Raw, Y.Raw, Z.Raw, W.Raw);

        public static bool operator ==(XFixedVector4 a, XFixedVector4 b)
            => a.Equals(b);

        public static bool operator !=(XFixedVector4 a, XFixedVector4 b)
            => !a.Equals(b);

        #endregion

        public override string ToString()
            => $"({X.ToDouble():F4}, {Y.ToDouble():F4}, {Z.ToDouble():F4}, {W.ToDouble():F4})";
    }
}