using System;
using XFixedPoint.Core;
using XFixedPoint.Vectors;

namespace XFixedPoint.Matrices
{
    /// <summary>
    /// 4×4 定点矩阵，行优先存储
    /// </summary>
    public readonly struct XFixedMatrix4x4 : IEquatable<XFixedMatrix4x4>
    {
        // 行 0
        public readonly XFixed M00, M01, M02, M03;
        // 行 1
        public readonly XFixed M10, M11, M12, M13;
        // 行 2
        public readonly XFixed M20, M21, M22, M23;
        // 行 3
        public readonly XFixed M30, M31, M32, M33;

        /// <summary>
        /// 单位矩阵
        /// </summary>
        public static readonly XFixedMatrix4x4 Identity = new XFixedMatrix4x4(
            XFixed.One,  XFixed.Zero, XFixed.Zero, XFixed.Zero,
            XFixed.Zero, XFixed.One,  XFixed.Zero, XFixed.Zero,
            XFixed.Zero, XFixed.Zero, XFixed.One,  XFixed.Zero,
            XFixed.Zero, XFixed.Zero, XFixed.Zero, XFixed.One
        );

        /// <summary>
        /// 构造完整矩阵，按行传入 16 个元素
        /// </summary>
        public XFixedMatrix4x4(
            XFixed m00, XFixed m01, XFixed m02, XFixed m03,
            XFixed m10, XFixed m11, XFixed m12, XFixed m13,
            XFixed m20, XFixed m21, XFixed m22, XFixed m23,
            XFixed m30, XFixed m31, XFixed m32, XFixed m33)
        {
            M00 = m00; M01 = m01; M02 = m02; M03 = m03;
            M10 = m10; M11 = m11; M12 = m12; M13 = m13;
            M20 = m20; M21 = m21; M22 = m22; M23 = m23;
            M30 = m30; M31 = m31; M32 = m32; M33 = m33;
        }

        #region 矩阵乘法

        /// <summary>
        /// 矩阵乘法：this * other
        /// </summary>
        public static XFixedMatrix4x4 operator *(XFixedMatrix4x4 a, XFixedMatrix4x4 b)
        {
            return new XFixedMatrix4x4(
                // Row 0
                a.M00 * b.M00 + a.M01 * b.M10 + a.M02 * b.M20 + a.M03 * b.M30,
                a.M00 * b.M01 + a.M01 * b.M11 + a.M02 * b.M21 + a.M03 * b.M31,
                a.M00 * b.M02 + a.M01 * b.M12 + a.M02 * b.M22 + a.M03 * b.M32,
                a.M00 * b.M03 + a.M01 * b.M13 + a.M02 * b.M23 + a.M03 * b.M33,
                // Row 1
                a.M10 * b.M00 + a.M11 * b.M10 + a.M12 * b.M20 + a.M13 * b.M30,
                a.M10 * b.M01 + a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31,
                a.M10 * b.M02 + a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32,
                a.M10 * b.M03 + a.M11 * b.M13 + a.M12 * b.M23 + a.M13 * b.M33,
                // Row 2
                a.M20 * b.M00 + a.M21 * b.M10 + a.M22 * b.M20 + a.M23 * b.M30,
                a.M20 * b.M01 + a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31,
                a.M20 * b.M02 + a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32,
                a.M20 * b.M03 + a.M21 * b.M13 + a.M22 * b.M23 + a.M23 * b.M33,
                // Row 3
                a.M30 * b.M00 + a.M31 * b.M10 + a.M32 * b.M20 + a.M33 * b.M30,
                a.M30 * b.M01 + a.M31 * b.M11 + a.M32 * b.M21 + a.M33 * b.M31,
                a.M30 * b.M02 + a.M31 * b.M12 + a.M32 * b.M22 + a.M33 * b.M32,
                a.M30 * b.M03 + a.M31 * b.M13 + a.M32 * b.M23 + a.M33 * b.M33
            );
        }

        #endregion

        #region 点/向量变换

        /// <summary>
        /// 将一个四维向量乘以本矩阵
        /// </summary>
        public XFixedVector4 Multiply(XFixedVector4 v)
            => new XFixedVector4(
                M00 * v.X + M01 * v.Y + M02 * v.Z + M03 * v.W,
                M10 * v.X + M11 * v.Y + M12 * v.Z + M13 * v.W,
                M20 * v.X + M21 * v.Y + M22 * v.Z + M23 * v.W,
                M30 * v.X + M31 * v.Y + M32 * v.Z + M33 * v.W
            );

        /// <summary>
        /// 将三维点（w=1）应用本矩阵，自动进行齐次除法
        /// </summary>
        public XFixedVector3 MultiplyPoint(XFixedVector3 p)
        {
            var v = Multiply(new XFixedVector4(p.X, p.Y, p.Z, XFixed.One));
            return new XFixedVector3(v.X / v.W, v.Y / v.W, v.Z / v.W);
        }

        /// <summary>
        /// 将三维向量（w=0）应用本矩阵，不做透视除法
        /// </summary>
        public XFixedVector3 MultiplyVector(XFixedVector3 v)
        {
            var r = Multiply(new XFixedVector4(v.X, v.Y, v.Z, XFixed.Zero));
            return new XFixedVector3(r.X, r.Y, r.Z);
        }

        #endregion

        #region 相等与哈希

        public bool Equals(XFixedMatrix4x4 other)
            => 
            M00 == other.M00 && M01 == other.M01 && M02 == other.M02 && M03 == other.M03 &&
            M10 == other.M10 && M11 == other.M11 && M12 == other.M12 && M13 == other.M13 &&
            M20 == other.M20 && M21 == other.M21 && M22 == other.M22 && M23 == other.M23 &&
            M30 == other.M30 && M31 == other.M31 && M32 == other.M32 && M33 == other.M33;

        public override bool Equals(object obj)
            => obj is XFixedMatrix4x4 m && Equals(m);

        public override int GetHashCode()
        {
            // 先把前 8 个元素 hash 到 hash1
            int hash1 = HashCode.Combine(
                M00.Raw, M01.Raw, M02.Raw, M03.Raw,
                M10.Raw, M11.Raw, M12.Raw, M13.Raw
            );
            // 再把后 8 个元素 hash 到 hash2
            int hash2 = HashCode.Combine(
                M20.Raw, M21.Raw, M22.Raw, M23.Raw,
                M30.Raw, M31.Raw, M32.Raw, M33.Raw
            );
            // 最后将两个中间值合并出最终哈希
            return HashCode.Combine(hash1, hash2);
        }

        public static bool operator ==(XFixedMatrix4x4 a, XFixedMatrix4x4 b) => a.Equals(b);
        public static bool operator !=(XFixedMatrix4x4 a, XFixedMatrix4x4 b) => !a.Equals(b);

        #endregion

        public override string ToString()
            => 
            $"|{M00.ToDouble():F3} {M01.ToDouble():F3} {M02.ToDouble():F3} {M03.ToDouble():F3}|\n" +
            $"|{M10.ToDouble():F3} {M11.ToDouble():F3} {M12.ToDouble():F3} {M13.ToDouble():F3}|\n" +
            $"|{M20.ToDouble():F3} {M21.ToDouble():F3} {M22.ToDouble():F3} {M23.ToDouble():F3}|\n" +
            $"|{M30.ToDouble():F3} {M31.ToDouble():F3} {M32.ToDouble():F3} {M33.ToDouble():F3}|";
    }
}