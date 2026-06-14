Shader "Universal Render Pipeline/Custom/OreSparkleLitTransparent"
{
    Properties
    {
        [MainTexture] _MainTex ("Base (RGB)", 2D) = "white" {}
        [MainColor] _Color ("Tint", Color) = (1,1,1,1)
        _Metallic ("Metallic", Range(0,1)) = 0
        _Smoothness ("Smoothness", Range(0,1)) = 0.55

        [HDR]_SparkleColor ("Sparkle Color", Color) = (1,1,1,1)
        _SparkleIntensity ("Sparkle Intensity", Range(0,8)) = 2.2
        _SparkleInterval ("Sparkle Interval (sec)", Range(0.2,8)) = 2
        _SparkleDuration ("Sparkle Duration (sec)", Range(0.05,2)) = 0.35
        _SparkleWidth ("Sparkle Width", Range(0.01,0.7)) = 0.12
        _SparkleSharpness ("Sparkle Sharpness", Range(0.5,8)) = 2.5
        _SparkleDirection ("Sparkle Direction (XY)", Vector) = (1, 1, 0, 0)

        [Header(Transparency)]
        _Alpha ("Alpha", Range(0, 1)) = 1
        _Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
        }
        LOD 300

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "OreSparkleLitCore.hlsl"

            OreSparkleVaryings vert(OreSparkleAttributes input)
            {
                return OreSparkleVert(input);
            }

            half4 frag(OreSparkleVaryings input) : SV_Target
            {
                return OreSparkleLitFragment(input, _Alpha, 0.0h);
            }
            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Back

            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            #pragma target 2.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            #include "OreSparkleLitCore.hlsl"

            struct ShadowAttributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct ShadowVaryings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 GetShadowPositionHClip(ShadowAttributes input)
            {
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);

                float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _MainLightPosition.xyz));

                #if UNITY_REVERSED_Z
                    positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
                #else
                    positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
                #endif

                return positionCS;
            }

            ShadowVaryings ShadowPassVertex(ShadowAttributes input)
            {
                ShadowVaryings output;
                output.positionCS = GetShadowPositionHClip(input);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                return output;
            }

            half4 ShadowPassFragment(ShadowVaryings input) : SV_TARGET
            {
                half alpha = OreSparkleSampleAlpha(input.uv) * _Alpha;
                clip(alpha - _Cutoff);
                return 0;
            }
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }

            ZWrite On
            ColorMask 0
            Cull Back

            HLSLPROGRAM
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment
            #pragma target 2.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "OreSparkleLitCore.hlsl"

            struct DepthAttributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct DepthVaryings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            DepthVaryings DepthOnlyVertex(DepthAttributes input)
            {
                DepthVaryings output = (DepthVaryings)0;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                return output;
            }

            half4 DepthOnlyFragment(DepthVaryings input) : SV_TARGET
            {
                half alpha = OreSparkleSampleAlpha(input.uv) * _Alpha;
                clip(alpha - _Cutoff);
                return 0;
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Lit"
}
