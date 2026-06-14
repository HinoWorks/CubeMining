#ifndef ORE_SPARKLE_LIT_CORE_INCLUDED
#define ORE_SPARKLE_LIT_CORE_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

struct OreSparkleAttributes
{
    float4 positionOS : POSITION;
    float3 normalOS : NORMAL;
    float4 tangentOS : TANGENT;
    float2 uv : TEXCOORD0;
};

struct OreSparkleVaryings
{
    float4 positionCS : SV_POSITION;
    float2 uv : TEXCOORD0;
    float3 positionWS : TEXCOORD1;
    float3 normalWS : TEXCOORD2;
    float4 tangentWS : TEXCOORD3;
};

TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);

CBUFFER_START(UnityPerMaterial)
    float4 _MainTex_ST;
    float4 _Color;
    float _Metallic;
    float _Smoothness;
    float4 _SparkleColor;
    float _SparkleIntensity;
    float _SparkleInterval;
    float _SparkleDuration;
    float _SparkleWidth;
    float _SparkleSharpness;
    float4 _SparkleDirection;
    float _Alpha;
    float _Cutoff;
CBUFFER_END

OreSparkleVaryings OreSparkleVert(OreSparkleAttributes input)
{
    OreSparkleVaryings output = (OreSparkleVaryings)0;

    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
    VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

    output.positionCS = vertexInput.positionCS;
    output.positionWS = vertexInput.positionWS;
    output.normalWS = normalInput.normalWS;
    output.tangentWS = float4(normalInput.tangentWS.xyz, input.tangentOS.w);
    output.uv = TRANSFORM_TEX(input.uv, _MainTex);
    return output;
}

half OreSparkleSampleAlpha(float2 uv)
{
    half4 baseC = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv) * _Color;
    return baseC.a;
}

half3 OreSparkleEmission(float2 uv, float time)
{
    float interval = max(_SparkleInterval, 0.001);
    float activeDuration = min(max(_SparkleDuration, 0.001), interval);

    float cycleTime = fmod(time, interval);
    float active01 = saturate(1.0 - cycleTime / activeDuration);
    float pulse = active01 * active01;

    float2 dir = normalize(max(abs(_SparkleDirection.xy), float2(0.0001, 0.0001)) * sign(_SparkleDirection.xy + 0.0001));
    float scan01 = saturate(cycleTime / activeDuration);
    float sweepCenter = lerp(-0.9, 0.9, scan01);
    float sweepCoord = dot(uv - 0.5, dir);

    float width = max(_SparkleWidth, 0.001);
    float dist = abs(sweepCoord - sweepCenter) / width;
    float sweepMask = exp(-dist * dist * _SparkleSharpness);

    half sparkle = (half)(pulse * sweepMask * _SparkleIntensity);
    return _SparkleColor.rgb * sparkle;
}

half4 OreSparkleLitFragment(OreSparkleVaryings input, half alphaScale, half forceOpaqueAlpha)
{
    float2 uv = input.uv;
    half4 baseC = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv) * _Color;
    half3 emission = OreSparkleEmission(uv, _Time.y);

    float3 normalWS = normalize(input.normalWS);
    InputData inputData;
    inputData.positionWS = input.positionWS;
    inputData.normalWS = normalWS;
    inputData.viewDirectionWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
    inputData.shadowCoord = TransformWorldToShadowCoord(input.positionWS);
    inputData.fogCoord = ComputeFogFactor(input.positionCS.z);
    inputData.vertexLighting = half3(0, 0, 0);
    inputData.bakedGI = SampleSH(normalWS);
    inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
    inputData.shadowMask = half4(1, 1, 1, 1);

    half alpha = lerp(baseC.a * alphaScale, 1.0h, forceOpaqueAlpha);

    SurfaceData surfaceData;
    surfaceData.albedo = baseC.rgb;
    surfaceData.metallic = _Metallic;
    surfaceData.specular = half3(0.0h, 0.0h, 0.0h);
    surfaceData.smoothness = _Smoothness;
    surfaceData.normalTS = half3(0, 0, 1);
    surfaceData.emission = emission;
    surfaceData.occlusion = 1.0;
    surfaceData.alpha = alpha;
    surfaceData.clearCoatMask = 0.0h;
    surfaceData.clearCoatSmoothness = 0.0h;

    half4 color = UniversalFragmentPBR(inputData, surfaceData);
    color.a = alpha;
    color.rgb = MixFog(color.rgb, inputData.fogCoord);
    return color;
}

#endif
