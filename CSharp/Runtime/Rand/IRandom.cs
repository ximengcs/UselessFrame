
using Unity.Mathematics;

namespace UselessFrame.NewRuntime.Randoms
{
    public interface IRandom
    {
        string NextString(int length);

        int NextInt();

        int NextInt(int min, int max);

        float NextFloat();

        float NextFloat(float min, float max);

        bool NextBoolean();

        double NextDouble();

        double NextDouble(double min, double max);

        float2 NextFloat2();

        float2 NextFloat2(float2 min, float2 max);

        float3 NextFloat3();

        float3 NextFloat3(float3 min, float3 max);

        float4 NextFloat4();

        float4 NextFloat4(float4 min, float4 max);

        int4 RandHSVColor();

        int4 RandHSVColor(int2 hueRange);

        int4 RandHSVColor(int2 hueRange, float2 saturationRange, float2 valueRange, float2 alphaRange);
    }
}
