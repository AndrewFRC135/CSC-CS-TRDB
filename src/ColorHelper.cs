using System.Drawing;

namespace CSCCSTRDB
{
    public static class ColorHelper
    {
        private const float byeHue = 210f; // Blue
        private const float highSeedHue = 150.0f; // Teal/Light Green
        private const float lowSeedHue = -30.0f; // Purple/Violet
        private const float saturation = 0.75f;
        private const float lightness = 0.25f;


        private static string byeHtmlColor = ColorTranslator.ToHtml(HslToColor(byeHue));
        private static string highSeedHtmlColor = ColorTranslator.ToHtml(HslToColor(highSeedHue));
        private static string lowSeedHtmlColor = ColorTranslator.ToHtml(HslToColor(lowSeedHue));

        private static string[] seedHtmlColorCache = new string[0];

        /// <summary>
        /// Get the correct color to identify this seed's color
        /// </summary>
        /// <param name="seedNum"></param>
        /// <param name="maxGroupSize"></param>
        /// <returns></returns>
        public static string GetSeedHtmlColor(int? seedNum, int maxGroupSize = 1)
        {
            if (seedNum.HasValue == false) 
            {
                seedNum = maxGroupSize;
            }

            if (seedNum.Value == 0)
            {   // "seed 0" players are ones that received a bye
                return byeHtmlColor;
            }

            if (maxGroupSize <= 1 || seedNum.Value >= maxGroupSize)
            {   // only one player per group -OR- seed num is >- maximum group size
                return lowSeedHtmlColor;
            }

            if (seedHtmlColorCache.Length != maxGroupSize)
            {   // the color cache is either the wrong size or doesn't exist yet. Generate it now
                seedHtmlColorCache = new string[maxGroupSize];

                // angle in degrees between each color
                float hueStepSize = (lowSeedHue - highSeedHue) / (maxGroupSize - 1);

                // the target hue value for each step
                float targetHue = highSeedHue;

                for (int i = 0; i < seedHtmlColorCache.Length; i++)
                {   // add the html color to the string array cache
                    seedHtmlColorCache[i] = ColorTranslator.ToHtml(HslToColor(targetHue));
                    targetHue += hueStepSize;
                }
            }

            // return the color from the cache
            return seedHtmlColorCache[seedNum.Value - 1];
        }

        private static Color HslToColor(float h, float s = saturation, float l = lightness)
        {
            byte r = 0;
            byte g = 0;
            byte b = 0;

            if (s == 0)
            {
                r = g = b = (byte)(l * 255);
            }
            else
            {
                float v1, v2;
                float hue = (float)h / 360;

                v2 = (l < 0.5) ? (l * (1 + s)) : ((l + s) - (l * s));
                v1 = 2 * l - v2;

                r = (byte)(255 * HueToRgb(v1, v2, hue + (1.0f / 3)));
                g = (byte)(255 * HueToRgb(v1, v2, hue));
                b = (byte)(255 * HueToRgb(v1, v2, hue - (1.0f / 3)));
            }

            return Color.FromArgb(r, g, b);
        }

        private static float HueToRgb(float v1, float v2, float vH)
        {
            if (vH < 0)
                vH += 1;

            if (vH > 1)
                vH -= 1;

            if ((6 * vH) < 1)
                return (v1 + (v2 - v1) * 6 * vH);

            if ((2 * vH) < 1)
                return v2;

            if ((3 * vH) < 2)
                return (v1 + (v2 - v1) * ((2.0f / 3) - vH) * 6);

            return v1;
        }
    }
}
