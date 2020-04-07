using System;
using System.Windows.Media;

namespace AMAGE.Common.Extensions
{
    public static class ColorEx
    {
        const float BrightnessR = 0.299f;
        const float BrightnessG = 0.587f;
        const float BrightnessB = 0.114f;

        public static double GetBrightness(this Color color)
        {
            return BrightnessR * color.R + BrightnessG * color.G + BrightnessB * color.B;
        }

        public static int ToArgb(this Color color)
        {
            return (color.A << 24) | (color.R << 16) | (color.G << 8) | (color.B);
        }


        public static Color Darker(this Color color, byte value)
        {
            return Color.FromArgb(
                color.A,
                (byte)Math.Max(color.R - value, 0),
                (byte)Math.Max(color.G - value, 0),
                (byte)Math.Max(color.B - value, 0)
                );
        }

        public static Color Lighter(this Color color, byte value)
        {
            return Color.FromArgb(
                color.A,
                (byte)Math.Min(color.R + value, 255),
                (byte)Math.Min(color.G + value, 255),
                (byte)Math.Min(color.B + value, 255)
                );
        }

        public static void ToHSL(byte r, byte g, byte b, out int h, out float s, out float l)
        {
            float rF = r / 255.0F;
            float gF = g / 255.0F;
            float bF = b / 255.0F;

            float cMax = Math.Max(Math.Max(rF, gF), bF);
            float cMin = Math.Min(Math.Min(rF, gF), bF);
            float delta = cMax - cMin;

            l = (cMax + cMin) / 2;

            if (delta > float.Epsilon)
            {
                s = delta / (1 - Math.Abs(2 * l - 1));

                if (cMax == rF)
                    h = (int)(60.0F * (((gF - bF) / delta) % 6));
                else if (cMax == gF)
                    h = (int)(60.0F * (((bF - rF) / delta) + 2));
                else
                    h = (int)(60.0F * (((rF - gF) / delta) + 4));
            }
            else
            {
                h = 0;
                s = 0.0F;
            }

            if (h < 0)
                h = -h;
        }

        public static void ToRgb(int h, float s, float l, out byte r, out byte g, out byte b)
        {
            float c = (1 - Math.Abs(2 * l - 1)) * s;
            float x = c * (1 - Math.Abs(((h / 60.0F) % 2) - 1));
            float m = l - c / 2;

            float rF, gF, bF;

            if (h < 60)
            {
                rF = c;
                gF = x;
                bF = 0;
            }
            else if (h < 120)
            {
                rF = x;
                gF = c;
                bF = 0;
            }
            else if (h < 180)
            {
                rF = 0;
                gF = c;
                bF = x;
            }
            else if (h < 240)
            {
                rF = 0;
                gF = x;
                bF = c;
            }
            else if (h < 300)
            {
                rF = x;
                gF = 0;
                bF = c;
            }
            else
            {
                rF = c;
                gF = 0;
                bF = x;
            }

            checked
            {
                r = (byte)(255 * (rF + m));
                g = (byte)(255 * (gF + m));
                b = (byte)(255 * (bF + m));
            }
        }
    }
}