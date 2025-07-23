
using IdGen;
using System;
using Unity.Mathematics;

namespace UselessFrame.NewRuntime.Randoms
{
    public class TimeRandom : IRandom
    {
        private Unity.Mathematics.Random _random;

        public TimeRandom(ITimeSource timeSource)
        {
            _random = new Unity.Mathematics.Random((uint)timeSource.GetTicks());
        }

        public bool NextBoolean()
        {
            return _random.NextBool();
        }

        public double NextDouble()
        {
            return _random.NextDouble();
        }

        public double NextDouble(double min, double max)
        {
            return _random.NextDouble(min, max);
        }

        public float NextFloat()
        {
            return _random.NextFloat();
        }

        public float NextFloat(float min, float max)
        {
            return _random.NextFloat(min, max);
        }

        public float2 NextFloat2()
        {
            return _random.NextFloat2();
        }

        public float2 NextFloat2(float2 min, float2 max)
        {
            return _random.NextFloat2(min, max);
        }

        public float3 NextFloat3()
        {
            return _random.NextFloat3();
        }

        public float3 NextFloat3(float3 min, float3 max)
        {
            return _random.NextFloat3(min, max);
        }

        public float4 NextFloat4()
        {
            return _random.NextFloat4();
        }

        public float4 NextFloat4(float4 min, float4 max)
        {
            return _random.NextFloat4(min, max);
        }

        public int NextInt()
        {
            return _random.NextInt();
        }

        public int NextInt(int min, int max)
        {
            return _random.NextInt(min, max);
        }

        public int4 RandHSVColor(int2 hueRange, float2 saturationRange, float2 valueRange, float2 alphaRange)
        {
            return HSVToRGB(
                NextInt(hueRange.x, hueRange.y),
                NextFloat(saturationRange.x, saturationRange.y),
                NextFloat(valueRange.x, valueRange.y),
                NextFloat(alphaRange.x, alphaRange.y));
        }

        public int4 RandHSVColor(int2 hueRange)
        {
            return HSVToRGB(NextInt(hueRange.x, hueRange.y), 1, 1, 1);
        }

        public int4 RandHSVColor()
        {
            return RandHSVColor(new int2(0, 360));
        }

        private int4 HSVToRGB(int hue, float saturation, float value, float alpha)
        {
            hue = Math.Clamp(hue, 0, 360);
            saturation = Math.Clamp(saturation, 0, 1);
            value = Math.Clamp(value, 0, 1);

            float c = value * saturation;
            float x = c * (1 - Math.Abs((hue / 60f) % 2 - 1));
            float m = value - c;

            float r, g, b;

            if (hue < 60)
            {
                r = c;
                g = x;
                b = 0;
            }
            else if (hue < 120)
            {
                r = x;
                g = c;
                b = 0;
            }
            else if (hue < 180)
            {
                r = 0;
                g = c;
                b = x;
            }
            else if (hue < 240)
            {
                r = 0;
                g = x;
                b = c;
            }
            else if (hue < 300)
            {
                r = x;
                g = 0;
                b = c;
            }
            else
            {
                r = c;
                g = 0;
                b = x;
            }

            return new int4((int)((r + m) * 255), (int)((g + m) * 255), (int)((b + m) * 255), (int)(alpha * 255));
        }
    }
}
