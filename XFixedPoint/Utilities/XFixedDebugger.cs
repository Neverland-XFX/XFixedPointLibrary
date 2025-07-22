using System;
using XFixedPoint.Core;

namespace XFixedPoint.Utilities
{
    /// <summary>
    /// 定点运算调试辅助：误差计算与打印
    /// </summary>
    public static class XFixedDebugger
    {
        /// <summary>
        /// 将定点值与预期浮点值进行对比，返回误差
        /// </summary>
        public static double ComputeError(XFixed xFixedValue, double expected)
        {
            double actual = xFixedValue.ToDouble();
            return actual - expected;
        }

        /// <summary>
        /// 打印定点值与预期值的比较结果到控制台
        /// </summary>
        public static void PrintError(string label, XFixed xFixedValue, double expected)
        {
            double actual = xFixedValue.ToDouble();
            double error  = actual - expected;
            Console.WriteLine($"{label}: Fixed={actual:F6}, Expected={expected:F6}, Error={error:E6}");
        }

        /// <summary>
        /// 对一组定点 vs 浮点结果做误差统计：最大误差与平均误差
        /// </summary>
        public static void PrintStatistics(string label, XFixed[] fixedValues, double[] expectedValues)
        {
            if (fixedValues.Length != expectedValues.Length)
                throw new ArgumentException("Arrays must have the same length");

            double sumErr = 0;
            double maxErr = 0;
            for (int i = 0; i < fixedValues.Length; i++)
            {
                double err = Math.Abs(fixedValues[i].ToDouble() - expectedValues[i]);
                sumErr += err;
                if (err > maxErr) maxErr = err;
            }
            double avgErr = sumErr / fixedValues.Length;
            Console.WriteLine($"{label} Statistics: Count={fixedValues.Length}, AvgError={avgErr:E6}, MaxError={maxErr:E6}");
        }
    }
}