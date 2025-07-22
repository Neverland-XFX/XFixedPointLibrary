using System;

namespace XFixedPoint.Core
{
    /// <summary>
    /// 常用数学函数：开方、三角、指数／对数、辅助函数
    /// </summary>
    internal static class XFixedMath
    {
        // 基本常量
        public static readonly XFixed Pi     = XFixed.FromDouble(Math.PI);
        public static readonly XFixed TwoPi  = XFixed.FromDouble(2.0 * Math.PI);
        public static readonly XFixed HalfPi = XFixed.FromDouble(Math.PI / 2.0);
        public static readonly XFixed E      = XFixed.FromDouble(Math.E);

        // CORDIC 迭代次数
        private const int Iterations = 32;

        // CORDIC 角度表：atan(2⁻ⁱ)
        private static readonly XFixed[] AtanTable = new XFixed[Iterations];
        // CORDIC 缩放系数 K = ∏₀ⁿ⁻¹ 1/√(1+2⁻²ⁱ)
        private static readonly XFixed CordicK;

        // 静态构造：预计算 AtanTable 与 CordicK
        static XFixedMath()
        {
            double k = 1.0;
            for (int i = 0; i < Iterations; i++)
            {
                double angle = Math.Atan(Math.Pow(2.0, -i));
                AtanTable[i] = XFixed.FromDouble(angle);
                k *= 1.0 / Math.Sqrt(1.0 + Math.Pow(2.0, -2.0 * i));
            }
            CordicK = XFixed.FromDouble(k);
        }

        #region 开方

        /// <summary>
        /// 牛顿迭代开方
        /// </summary>
        public static XFixed Sqrt(XFixed x)
        {
            if (x.Raw <= 0) return XFixed.Zero;
            XFixed guess = x;
            // 16 次迭代通常可达 32 位精度
            for (int i = 0; i < 16; i++)
            {
                guess = (guess + x / guess) >> 1;
            }
            return guess;
        }

        #endregion

        #region 三角函数（CORDIC）

        /// <summary>
        /// 同时计算 sin 与 cos
        /// </summary>
        public static void SinCos(XFixed angle, out XFixed sin, out XFixed cos)
        {
            // 归一化到 [-π, π]
            long raw = angle.Raw % TwoPi.Raw;
            if (raw < 0) raw += TwoPi.Raw;
            XFixed z = XFixed.FromRaw(raw);
            if (z > Pi) z -= TwoPi;

            // 初始向量 (K, 0)
            XFixed x = CordicK;
            XFixed y = XFixed.Zero;

            // 迭代旋转
            for (int i = 0; i < Iterations; i++)
            {
                XFixed dx = x >> i;
                XFixed dy = y >> i;
                if (z.Raw >= 0)
                {
                    x = x - dy;
                    y = y + dx;
                    z = z - AtanTable[i];
                }
                else
                {
                    x = x + dy;
                    y = y - dx;
                    z = z + AtanTable[i];
                }
            }

            cos = x;
            sin = y;
        }

        public static XFixed Sin(XFixed angle)
        {
            SinCos(angle, out var s, out _);
            return s;
        }

        public static XFixed Cos(XFixed angle)
        {
            SinCos(angle, out _, out var c);
            return c;
        }

        public static XFixed Tan(XFixed angle)
            => Sin(angle) / Cos(angle);

        /// <summary>
        /// CORDIC 向量化计算 Atan2(y, x)，范围 (-π, π]
        /// </summary>
        public static XFixed Atan2(XFixed y, XFixed x)
        {
            // 向量化模式，初始 z=0
            XFixed x1 = x;
            XFixed y1 = y;
            XFixed z = XFixed.Zero;

            for (int i = 0; i < Iterations; i++)
            {
                XFixed dx = x1 >> i;
                XFixed dy = y1 >> i;
                if (y1.Raw > 0)
                {
                    x1 = x1 + dy;
                    y1 = y1 - dx;
                    z = z + AtanTable[i];
                }
                else
                {
                    x1 = x1 - dy;
                    y1 = y1 + dx;
                    z = z - AtanTable[i];
                }
            }

            // 象限修正
            if (x.Raw < 0)
            {
                z = y.Raw >= 0 ? z + Pi : z - Pi;
            }
            return z;
        }

        #endregion

        #region 指数 / 对数

        /// <summary>
        /// 自然指数 eˣ（当前版本借助双精度，后续可用泰勒级数或分段逼近实现纯定点）
        /// </summary>
        public static XFixed Exp(XFixed x)
            => XFixed.FromDouble(Math.Exp(x.ToDouble()));

        /// <summary>
        /// 自然对数 ln(x)（当前版本借助双精度，x 必须 > 0）
        /// </summary>
        public static XFixed Log(XFixed x)
            => XFixed.FromDouble(Math.Log(x.ToDouble()));

        /// <summary>
        /// 任意幂 xʸ = exp(y * ln(x))
        /// </summary>
        public static XFixed Pow(XFixed x, XFixed y)
            => Exp(y * Log(x));

        #endregion

        #region 辅助函数

        public static XFixed Abs(XFixed x)
            => x.Raw < 0 ? XFixed.FromRaw(-x.Raw) : x;

        public static XFixed Min(XFixed a, XFixed b)
            => a.Raw < b.Raw ? a : b;

        public static XFixed Max(XFixed a, XFixed b)
            => a.Raw > b.Raw ? a : b;

        public static XFixed Clamp(XFixed value, XFixed min, XFixed max)
            => value.Raw < min.Raw ? min : (value.Raw > max.Raw ? max : value);

        public static XFixed Lerp(XFixed a, XFixed b, XFixed t)
            => a + (b - a) * t;

        #endregion
    }
}