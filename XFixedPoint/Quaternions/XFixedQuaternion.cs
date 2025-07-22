using System;
using XFixedPoint.Core;
using XFixedPoint.Vectors;

namespace XFixedPoint.Quaternions
{
    /// <summary>
    /// 定点四元数，用于表示三维旋转
    /// </summary>
    public readonly struct XFixedQuaternion : IEquatable<XFixedQuaternion>
    {
        /// <summary>
        /// x 分量
        /// </summary>
        public readonly XFixed X;
        /// <summary>
        /// y 分量
        /// </summary>
        public readonly XFixed Y;
        /// <summary>
        /// z 分量
        /// </summary>
        public readonly XFixed Z;
        /// <summary>
        /// w 分量（标量部分）
        /// </summary>
        public readonly XFixed W;

        /// <summary>
        /// 单位四元数，表示“无旋转”
        /// </summary>
        public static readonly XFixedQuaternion Identity =
            new XFixedQuaternion(XFixed.Zero, XFixed.Zero, XFixed.Zero, XFixed.One);

        /// <summary>
        /// 构造四元数 (x, y, z, w)
        /// </summary>
        public XFixedQuaternion(XFixed x, XFixed y, XFixed z, XFixed w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// 从浮点数构造四元数
        /// </summary>
        public static XFixedQuaternion FromFloats(float x, float y, float z, float w)
            => new XFixedQuaternion(
                XFixed.FromFloat(x),
                XFixed.FromFloat(y),
                XFixed.FromFloat(z),
                XFixed.FromFloat(w)
            );

        /// <summary>
        /// 从轴–角（Axis–Angle）构造：axis 必须归一化，angle 单位为弧度
        /// </summary>
        public static XFixedQuaternion FromAxisAngle(XFixedVector3 axis, XFixed angle)
        {
            var half = angle * XFixed.Half;
            var s = XFixedMath.Sin(half);
            return new XFixedQuaternion(
                axis.X * s,
                axis.Y * s,
                axis.Z * s,
                XFixedMath.Cos(half)
            );
        }

        /// <summary>
        /// 四元数长度平方
        /// </summary>
        public XFixed SqrMagnitude
            => X * X + Y * Y + Z * Z + W * W;

        /// <summary>
        /// 四元数长度
        /// </summary>
        public XFixed Magnitude
            => XFixedMath.Sqrt(SqrMagnitude);

        /// <summary>
        /// 归一化四元数
        /// </summary>
        public XFixedQuaternion Normalized
        {
            get
            {
                var mag = Magnitude;
                if (mag == XFixed.Zero) return Identity;
                var inv = XFixed.One / mag;
                return new XFixedQuaternion(X * inv, Y * inv, Z * inv, W * inv);
            }
        }

        /// <summary>
        /// 四元数共轭 (x,y,z 取反, w 保持不变)
        /// </summary>
        public XFixedQuaternion Conjugate
            => new XFixedQuaternion(-X, -Y, -Z, W);

        /// <summary>
        /// 四元数逆：q⁻¹ = q* / |q|²
        /// </summary>
        public XFixedQuaternion Inverse
        {
            get
            {
                var sq = SqrMagnitude;
                if (sq == XFixed.Zero) return Identity;
                return Conjugate * (XFixed.One / sq);
            }
        }

        /// <summary>
        /// 四元数与四元数相乘：组合旋转
        /// </summary>
        public static XFixedQuaternion operator *(XFixedQuaternion a, XFixedQuaternion b)
        {
            return new XFixedQuaternion(
                a.W * b.X + a.X * b.W + a.Y * b.Z - a.Z * b.Y,
                a.W * b.Y - a.X * b.Z + a.Y * b.W + a.Z * b.X,
                a.W * b.Z + a.X * b.Y - a.Y * b.X + a.Z * b.W,
                a.W * b.W - a.X * b.X - a.Y * b.Y - a.Z * b.Z
            );
        }
        
        /// <summary>
        /// 将本四元数转换为欧拉角 (pitch, yaw, roll)
        /// </summary>
        public XFixedVector3 ToEulerAngles()
        {
            return XFixedQuaternionOps.ToEulerAngles(this);
        }
        
        /// <summary>
        /// 四元数缩放：每个分量乘以标量
        /// </summary>
        public static XFixedQuaternion operator *(XFixedQuaternion q, XFixed s)
            => new XFixedQuaternion(q.X * s, q.Y * s, q.Z * s, q.W * s);

        /// <summary>
        /// 标量左乘四元数
        /// </summary>
        public static XFixedQuaternion operator *(XFixed s, XFixedQuaternion q)
            => q * s;

        /// <summary>
        /// 四元数除以标量（可选，但有时会用到）
        /// </summary>
        public static XFixedQuaternion operator /(XFixedQuaternion q, XFixed s)
            => new XFixedQuaternion(q.X / s, q.Y / s, q.Z / s, q.W / s);

        /// <summary>
        /// 使用四元数旋转向量：q * v * q⁻¹ 的优化形式
        /// </summary>
        public XFixedVector3 Rotate(XFixedVector3 v)
        {
            var u = new XFixedVector3(X, Y, Z);
            var s = W;
            // t = 2 * cross(u, v)
            var t = u.Cross(v) * XFixed.FromInt(2);
            // result = v + s * t + cross(u, t)
            return v + t * s + u.Cross(t);
        }

        /// <summary>
        /// 检查相等
        /// </summary>
        public bool Equals(XFixedQuaternion other)
            => X == other.X && Y == other.Y && Z == other.Z && W == other.W;

        public override bool Equals(object obj)
            => obj is XFixedQuaternion q && Equals(q);

        public override int GetHashCode()
            => HashCode.Combine(X.Raw, Y.Raw, Z.Raw, W.Raw);

        public static bool operator ==(XFixedQuaternion a, XFixedQuaternion b)
            => a.Equals(b);

        public static bool operator !=(XFixedQuaternion a, XFixedQuaternion b)
            => !a.Equals(b);

        public override string ToString()
            => $"({X.ToDouble():F4}, {Y.ToDouble():F4}, {Z.ToDouble():F4}, {W.ToDouble():F4})";
    }
}