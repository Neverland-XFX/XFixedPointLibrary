using System;
using XFixedPoint.Core;
using XFixedPoint.Matrices;
using XFixedPoint.Vectors;

namespace XFixedPoint.Quaternions
{
    /// <summary>
    /// 四元数辅助操作：点乘、插值、欧拉角转换、旋转矩阵
    /// </summary>
    public static class XFixedQuaternionOps
    {
        /// <summary>
        /// 四元数点乘
        /// </summary>
        public static XFixed Dot(XFixedQuaternion a, XFixedQuaternion b)
            => a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;

        /// <summary>
        /// 四元数夹角（弧度）
        /// </summary>
        public static XFixed Angle(XFixedQuaternion a, XFixedQuaternion b)
        {
            var dot = Dot(a.Normalized, b.Normalized);
            dot = XFixedMath.Clamp(dot, -XFixed.One, XFixed.One);
            // 使用 Math.Acos + 双精度转换
            var theta = XFixed.FromDouble(Math.Acos(dot.ToDouble()));
            return theta * XFixed.FromInt(2);
        }

        /// <summary>
        /// 球面线性插值（Slerp）
        /// </summary>
        public static XFixedQuaternion Slerp(XFixedQuaternion a, XFixedQuaternion b, XFixed t)
        {
            var tt = XFixedMath.Clamp(t, XFixed.Zero, XFixed.One);
            var cosTheta = Dot(a, b);

            // 如果角度大于 90°，反向 b 保证最短路径
            if (cosTheta < XFixed.Zero)
            {
                b = new XFixedQuaternion(-b.X, -b.Y, -b.Z, -b.W);
                cosTheta = -cosTheta;
            }

            // 若接近平行，退化为线性插值
            if (cosTheta > XFixed.FromDouble(0.9995))
            {
                var result = new XFixedQuaternion(
                    a.X + (b.X - a.X) * tt,
                    a.Y + (b.Y - a.Y) * tt,
                    a.Z + (b.Z - a.Z) * tt,
                    a.W + (b.W - a.W) * tt
                );
                return result.Normalized;
            }

            // 正常 Slerp
            var theta = XFixed.FromDouble(Math.Acos(cosTheta.ToDouble()));
            var sinTheta = XFixedMath.Sqrt(XFixed.One - cosTheta * cosTheta);
            var invSin = XFixed.One / sinTheta;
            var w1 = XFixedMath.Sin((XFixed.One - tt) * theta) * invSin;
            var w2 = XFixedMath.Sin(tt * theta) * invSin;

            return new XFixedQuaternion(
                a.X * w1 + b.X * w2,
                a.Y * w1 + b.Y * w2,
                a.Z * w1 + b.Z * w2,
                a.W * w1 + b.W * w2
            );
        }

        /// <summary>
        /// 欧拉角 → 四元数
        /// euler：X = pitch，Y = yaw，Z = roll，单位：弧度
        /// </summary>
        public static XFixedQuaternion FromEulerAngles(XFixedVector3 euler)
        {
            var halfX = euler.X * XFixed.Half;
            var halfY = euler.Y * XFixed.Half;
            var halfZ = euler.Z * XFixed.Half;

            var sinX = XFixedMath.Sin(halfX);
            var cosX = XFixedMath.Cos(halfX);
            var sinY = XFixedMath.Sin(halfY);
            var cosY = XFixedMath.Cos(halfY);
            var sinZ = XFixedMath.Sin(halfZ);
            var cosZ = XFixedMath.Cos(halfZ);

            // 组合顺序：yaw (Y) → pitch (X) → roll (Z)
            return new XFixedQuaternion(
                cosY * sinX * cosZ + sinY * cosX * sinZ, // x
                sinY * cosX * cosZ - cosY * sinX * sinZ, // y
                cosY * cosX * sinZ - sinY * sinX * cosZ, // z
                cosY * cosX * cosZ + sinY * sinX * sinZ // w
            );
        }

        /// <summary>
        /// 四元数 → 欧拉角
        /// 返回 Vector3(pitch=X, yaw=Y, roll=Z)，单位：弧度
        /// </summary>
        public static XFixedVector3 ToEulerAngles(XFixedQuaternion q)
        {
            // Roll (Z 轴旋转)
            var sinRCosP = XFixed.FromDouble(2.0) * (q.W * q.Z + q.X * q.Y);
            var cosRCosP = XFixed.One - XFixed.FromDouble(2.0) * (q.Y * q.Y + q.Z * q.Z);
            var roll = XFixed.FromDouble(Math.Atan2(sinRCosP.ToDouble(), cosRCosP.ToDouble()));

            // Pitch (X 轴旋转)
            var sinP = XFixed.FromDouble(2.0) * (q.W * q.X - q.Z * q.Y);
            sinP = XFixedMath.Clamp(sinP, -XFixed.One, XFixed.One);
            var pitch = XFixed.FromDouble(Math.Asin(sinP.ToDouble()));

            // Yaw (Y 轴旋转)
            var sinYCosP = XFixed.FromDouble(2.0) * (q.W * q.Y + q.Z * q.X);
            var cosYCosP = XFixed.One - XFixed.FromDouble(2.0) * (q.X * q.X + q.Y * q.Y);
            var yaw = XFixed.FromDouble(Math.Atan2(sinYCosP.ToDouble(), cosYCosP.ToDouble()));

            return new XFixedVector3(pitch, yaw, roll);
        }

        /// <summary>
        /// 四元数 → 4×4 旋转矩阵
        /// </summary>
        public static XFixedMatrix4x4 ToRotationMatrix(XFixedQuaternion q)
        {
            var xx = q.X * q.X;
            var yy = q.Y * q.Y;
            var zz = q.Z * q.Z;
            var xy = q.X * q.Y;
            var xz = q.X * q.Z;
            var yz = q.Y * q.Z;
            var wx = q.W * q.X;
            var wy = q.W * q.Y;
            var wz = q.W * q.Z;

            return new XFixedMatrix4x4(
                // 第一行
                XFixed.One - XFixed.FromDouble(2.0) * (yy + zz), XFixed.FromDouble(2.0) * (xy - wz),
                XFixed.FromDouble(2.0) * (xz + wy), XFixed.Zero,
                // 第二行
                XFixed.FromDouble(2.0) * (xy + wz), XFixed.One - XFixed.FromDouble(2.0) * (xx + zz),
                XFixed.FromDouble(2.0) * (yz - wx), XFixed.Zero,
                // 第三行
                XFixed.FromDouble(2.0) * (xz - wy), XFixed.FromDouble(2.0) * (yz + wx),
                XFixed.One - XFixed.FromDouble(2.0) * (xx + yy), XFixed.Zero,
                // 第四行
                XFixed.Zero, XFixed.Zero, XFixed.Zero, XFixed.One
            );
        }
    }
}