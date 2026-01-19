Shader "Universal Render Pipeline/Custom/CrackOverlay_UV1Only_Glow_EmitBase"
{
    Properties
    {
        _MainTex ("Base (RGB) A=SM", 2D) = "white" {}
        _BumpMap ("Base Normal", 2D) = "bump" {}
        _CrackMask ("Crack Mask (R)", 2D) = "gray" {}
        _CrackNormal ("Crack Normal", 2D) = "bump" {}

        _Color ("Tint", Color) = (1,1,1,1)
        _CrackDarken ("Crack Darken", Range(0,1)) = 0.4
        _CrackSmoothMul ("Crack Smoothness Mult", Range(0,1)) = 0.5

        _Metallic ("Metallic", Range(0,1)) = 0
        _Smoothness ("Base Smoothness", Range(0,1)) = 0.5

        // 既存マテリアルのEmissionを引き継ぐ
        [HDR]_EmissionColor ("Emission Color", Color) = (0,0,0,0)
        _EmissionMap ("Emission Map", 2D) = "black" {}

        // ひび発光
        [HDR]_CrackGlowColor ("Crack Glow Color", Color) = (1,0.7,0.3,1)
        _CrackGlowStrength ("Glow Strength", Range(0,10)) = 2
        _GlowTightness ("Glow Tightness", Range(0.5,8)) = 3
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalPipeline"
        }
        LOD 300

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float4 tangentWS : TEXCOORD3;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_BumpMap);
            SAMPLER(sampler_BumpMap);
            TEXTURE2D(_CrackMask);
            SAMPLER(sampler_CrackMask);
            TEXTURE2D(_CrackNormal);
            SAMPLER(sampler_CrackNormal);
            TEXTURE2D(_EmissionMap);
            SAMPLER(sampler_EmissionMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float _CrackDarken;
                float _CrackSmoothMul;
                float _Metallic;
                float _Smoothness;
                float4 _EmissionColor;
                float4 _CrackGlowColor;
                float _CrackGlowStrength;
                float _GlowTightness;
            CBUFFER_END

            inline float3 BlendNormalsAdditive(float3 nBase, float3 nAdd, float mask)
            {
                // Tangent空間の簡易加算ブレンド
                float3 n = nBase + (nAdd - float3(0,0,1)) * mask;
                return normalize(n);
            }

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.normalWS = normalInput.normalWS;
                output.tangentWS = float4(normalInput.tangentWS.xyz, input.tangentOS.w);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // UV（_STは使わずUV1を共通使用）
                float2 uv = input.uv;

                // サンプリング
                half4 baseC = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv) * _Color;
                half mask = SAMPLE_TEXTURE2D(_CrackMask, sampler_CrackMask, uv).r;

                // Albedo：ひび部分を暗く
                half3 crackDark = baseC.rgb * (1.0 - _CrackDarken);
                half3 albedo = lerp(baseC.rgb, crackDark, mask);

                // Normal：加算ブレンド
                float3 nBase = UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, uv));
                float3 nCrack = UnpackNormal(SAMPLE_TEXTURE2D(_CrackNormal, sampler_CrackNormal, uv));
                float3 normalTS = BlendNormalsAdditive(nBase, nCrack, mask);

                // Tangent to World space
                float3 bitangentWS = cross(input.normalWS, input.tangentWS.xyz) * input.tangentWS.w;
                float3x3 tangentToWorld = float3x3(input.tangentWS.xyz, bitangentWS, input.normalWS);
                float3 normalWS = TransformTangentToWorld(normalTS, tangentToWorld, true);

                // Smoothness/Metallic
                half smoothness = lerp(_Smoothness, _Smoothness * _CrackSmoothMul, mask);
                half metallic = _Metallic;

                // Emission：ベースEmission + ひびEmission（加算）
                half3 baseEmi = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, uv).rgb * _EmissionColor.rgb;
                half glowM = saturate(pow(max(mask, 1e-4), _GlowTightness));
                half3 crackEmi = _CrackGlowColor.rgb * (_CrackGlowStrength * glowM);
                half3 emission = baseEmi + crackEmi;

                // URP のライティング計算
                InputData inputData;
                inputData.positionWS = input.positionWS;
                inputData.normalWS = normalWS;
                inputData.viewDirectionWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
                inputData.shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                inputData.fogCoord = ComputeFogFactor(input.positionCS.z);
                inputData.vertexLighting = half3(0, 0, 0);
                inputData.bakedGI = SAMPLE_GI(input.uv, SampleSH(normalWS), normalWS);
                inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
                inputData.shadowMask = half4(1, 1, 1, 1);

                SurfaceData surfaceData;
                surfaceData.albedo = albedo;
                surfaceData.metallic = metallic;
                surfaceData.specular = half3(0.0h, 0.0h, 0.0h);
                surfaceData.smoothness = smoothness;
                surfaceData.normalTS = normalTS;
                surfaceData.emission = emission;
                surfaceData.occlusion = 1.0;
                surfaceData.alpha = 1.0;
                surfaceData.clearCoatMask = 0.0h;
                surfaceData.clearCoatSmoothness = 0.0h;

                half4 color = UniversalFragmentPBR(inputData, surfaceData);
                color.rgb = MixFog(color.rgb, inputData.fogCoord);
                return color;
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

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            float4 GetShadowPositionHClip(Attributes input)
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

            Varyings ShadowPassVertex(Attributes input)
            {
                Varyings output;
                output.positionCS = GetShadowPositionHClip(input);
                return output;
            }

            half4 ShadowPassFragment(Varyings input) : SV_TARGET
            {
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

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings DepthOnlyVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                return output;
            }

            half4 DepthOnlyFragment(Varyings input) : SV_TARGET
            {
                return 0;
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Lit"
}
