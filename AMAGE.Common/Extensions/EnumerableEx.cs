using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace AMAGE.Common.Extensions
{
    public static class EnumerableEx
    {
        public static int[] Interpolate(this IEnumerable<int> input, int outputCount)
        {
            return Interpolate(input.Select(i => (double)i), outputCount)
                .Select(i => (int)(i + 0.5)).ToArray();
        }

        public static float[] Interpolate(this IEnumerable<float> input, int outputCount)
        {
            return Interpolate(input.Select(i => (double)i), outputCount)
                .Select(i => (float)i).ToArray();
        }

        public static double[] Interpolate(this IEnumerable<double> input, int outputCount)
        {
            IReadOnlyList<double> inputList = input as IReadOnlyList<double> ?? input.ToArray();

            int inputCount = inputList.Count;
            double[] output = new double[outputCount];

            if (outputCount == 0 || inputCount == 0)
                return output;

            if (outputCount == 1)
            {
                output[0] = inputList[0];
                return output;
            }

            if (inputCount == 1)
            {
                for (int i = 0; i < outputCount; ++i)
                    output[i] = inputList[0];
                return output;
            }

            if (outputCount > inputCount)
            {
                double multipiler = (double)(outputCount - 1) / ((inputCount - 1) * outputCount / inputCount);

                for (int i = 0; i < inputCount; ++i)
                {
                    int position = (int)(multipiler * (i * outputCount / inputCount));
                    output[position] = inputList[i];

                    if (position > 0)
                    {
                        int previousPosition = (int)(multipiler * ((i - 1) * outputCount / inputCount));

                        for (int j = previousPosition + 1; j < position; ++j)
                        {
                            output[j] = output[previousPosition] + (output[position] - output[previousPosition]) /
                                (position - previousPosition) * (j - previousPosition);
                        }
                    }
                }
            }
            else
            {
                double multipiler = (double)(inputCount - 1) / ((outputCount - 1) * inputCount / outputCount);

                for (int i = 0; i < outputCount; ++i)
                {
                    int position = (int)(multipiler * (i * inputCount / outputCount));
                    output[i] = inputList[position];
                }
            }

            return output;
        }

        public static Matrix3D[] Interpolate(this IEnumerable<Matrix3D> input, int outputCount)
        {
            double[] m11 = input.Select(m => m.M11).Interpolate(outputCount);
            double[] m12 = input.Select(m => m.M12).Interpolate(outputCount);
            double[] m13 = input.Select(m => m.M13).Interpolate(outputCount);
            double[] m14 = input.Select(m => m.M14).Interpolate(outputCount);
            double[] m21 = input.Select(m => m.M21).Interpolate(outputCount);
            double[] m22 = input.Select(m => m.M22).Interpolate(outputCount);
            double[] m23 = input.Select(m => m.M23).Interpolate(outputCount);
            double[] m24 = input.Select(m => m.M24).Interpolate(outputCount);
            double[] m31 = input.Select(m => m.M31).Interpolate(outputCount);
            double[] m32 = input.Select(m => m.M32).Interpolate(outputCount);
            double[] m33 = input.Select(m => m.M33).Interpolate(outputCount);
            double[] m34 = input.Select(m => m.M34).Interpolate(outputCount);
            double[] m41 = input.Select(m => m.OffsetX).Interpolate(outputCount);
            double[] m42 = input.Select(m => m.OffsetY).Interpolate(outputCount);
            double[] m43 = input.Select(m => m.OffsetZ).Interpolate(outputCount);
            double[] m44 = input.Select(m => m.M44).Interpolate(outputCount);

            Matrix3D[] result = new Matrix3D[outputCount];

            for (int i = 0; i < outputCount; ++i)
            {
                result[i] = new Matrix3D(m11[i], m12[i], m13[i], m14[i], m21[i], m22[i], m23[i],
                    m24[i], m31[i], m32[i], m33[i], m34[i], m41[i], m42[i], m43[i], m44[i]);
            }

            return result;
        }

        public static Color[] Interpolate(this IEnumerable<Color> input, int outputCount)
        {
            double[] r = input.Select(c => (double)c.R).Interpolate(outputCount);
            double[] g = input.Select(c => (double)c.G).Interpolate(outputCount);
            double[] b = input.Select(c => (double)c.B).Interpolate(outputCount);
            double[] a = input.Select(c => (double)c.A).Interpolate(outputCount);

            Color[] result = new Color[outputCount];

            for (int i = 0; i < outputCount; ++i)
                result[i] = Color.FromArgb(
                    (byte)(a[i] + 0.5),
                    (byte)(r[i] + 0.5),
                    (byte)(g[i] + 0.5),
                    (byte)(b[i] + 0.5));

            return result;
        }
    }
}

