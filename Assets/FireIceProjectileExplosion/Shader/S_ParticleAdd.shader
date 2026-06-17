Shader "S_ParticleAdd"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.BlendMode)]_Add_Blend("Add_Blend", Float) = 1
        [HDR]_Tint_01("Tint_01", Color) = (1,1,1,0)
        [HDR]_Tint_02("Tint_02", Color) = (1,1,1,0)
        _MainTex("MainTex", 2D) = "white" {}
        _SpeedDirMainTex("Speed Dir MainTex", Vector) = (0,0,0,0)
        _NoiseMainTex("Noise MainTex", 2D) = "white" {}
        _NoiseStrengthMainTex("Noise Strength MainTex", Float) = 0
        _SpeedDirNoiseMainTex("Speed Dir Noise MainTex", Vector) = (0,0,0,0)
        _AlphaMask("Alpha Mask", 2D) = "white" {}
        _SpeedDirAlphaMask("Speed Dir Alpha Mask", Vector) = (0,0,0,0)
        _NoiseAlphaMask("Noise Alpha Mask", 2D) = "white" {}
        _NoiseAlphaMaskStrength("Noise Alpha Mask Strength", Float) = 0
        _SpeedDirNoiseAlphaMask("Speed Dir Noise Alpha Mask", Vector) = (0,0,0,0)
        _SubAlphaMask("Sub Alpha Mask", 2D) = "white" {}
        _SpeedDirSubAlphaMask("Speed Dir Sub Alpha Mask", Vector) = (0,0,0,0)
        [Enum(UnityEngine.Rendering.CullMode)]_CullMode("CullMode", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull [_CullMode]

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _Tint_01;
                float4 _Tint_02;
                float4 _MainTex_ST;
                float2 _SpeedDirMainTex;
                float _NoiseStrengthMainTex;
                float4 _NoiseMainTex_ST;
                float2 _SpeedDirNoiseMainTex;
                float4 _AlphaMask_ST;
                float2 _SpeedDirAlphaMask;
                float _NoiseAlphaMaskStrength;
                float4 _NoiseAlphaMask_ST;
                float2 _SpeedDirNoiseAlphaMask;
                float4 _SubAlphaMask_ST;
                float2 _SpeedDirSubAlphaMask;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_NoiseMainTex);
            SAMPLER(sampler_NoiseMainTex);
            TEXTURE2D(_AlphaMask);
            SAMPLER(sampler_AlphaMask);
            TEXTURE2D(_NoiseAlphaMask);
            SAMPLER(sampler_NoiseAlphaMask);
            TEXTURE2D(_SubAlphaMask);
            SAMPLER(sampler_SubAlphaMask);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 uv2 : TEXCOORD1;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 uv2 : TEXCOORD1;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.color = input.color;
                output.uv = input.uv;
                output.uv2 = input.uv2;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float controlMoveMainTexByU = input.uv2.w;
                float2 appendMain = float2(0.0, controlMoveMainTexByU);
                float2 uvMainTex = input.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                float2 pannerMain = _Time.y * _SpeedDirMainTex + uvMainTex;
                float2 uvNoiseMain = input.uv * _NoiseMainTex_ST.xy + _NoiseMainTex_ST.zw;
                float2 pannerNoiseMain = _Time.y * _SpeedDirNoiseMainTex + uvNoiseMain;
                float noiseMain = SAMPLE_TEXTURE2D(_NoiseMainTex, sampler_NoiseMainTex, pannerNoiseMain).r;
                float2 mainSampleUV = appendMain + pannerMain + (_NoiseStrengthMainTex * noiseMain);
                float4 mainSample = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, mainSampleUV);
                half4 tint = lerp(_Tint_01, _Tint_02, mainSample);
                half3 emission = (tint * input.color).rgb;

                float controlMoveAlphaByU = input.uv2.z;
                float2 appendAlpha = float2(0.0, controlMoveAlphaByU);
                float2 uvAlphaMask = input.uv * _AlphaMask_ST.xy + _AlphaMask_ST.zw;
                float2 pannerAlpha = _Time.y * _SpeedDirAlphaMask + uvAlphaMask;
                float2 pannerNoiseAlpha = _Time.y * _SpeedDirNoiseAlphaMask + uvAlphaMask;
                float4 noiseAlphaSample = _NoiseAlphaMaskStrength
                    * SAMPLE_TEXTURE2D(_NoiseAlphaMask, sampler_NoiseAlphaMask, pannerNoiseAlpha);
                float2 alphaSampleUV = (float4(appendAlpha, 0.0, 0.0) + float4(pannerAlpha, 0.0, 0.0) + noiseAlphaSample).rg;
                float2 uvSubAlpha = input.uv * _SubAlphaMask_ST.xy + _SubAlphaMask_ST.zw;
                float2 pannerSubAlpha = _Time.y * _SpeedDirSubAlphaMask + uvSubAlpha;
                half alpha = (
                    SAMPLE_TEXTURE2D(_AlphaMask, sampler_AlphaMask, alphaSampleUV)
                    * SAMPLE_TEXTURE2D(_SubAlphaMask, sampler_SubAlphaMask, pannerSubAlpha)
                    * input.color.a
                ).r;

                return half4(emission, alpha);
            }
            ENDHLSL
        }
    }

    Fallback Off
}
