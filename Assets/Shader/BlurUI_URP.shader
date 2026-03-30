Shader "Custom/UI/Blur URP"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1, 1, 1, 1)
        _Blur ("Blur Strength", Range(0, 8)) = 2
        _TextureSampleAdd ("Texture Sample Add", Vector) = (0, 0, 0, 0)
        _UIMaskSoftnessX ("Mask Softness X", Float) = 1
        _UIMaskSoftnessY ("Mask Softness Y", Float) = 1

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _CullMode ("Cull Mode", Float) = 0
        _ColorMask ("Color Mask", Float) = 15
        _ClipRect ("Clip Rect", Vector) = (-32767, -32767, 32767, 32767)

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
            "RenderPipeline" = "UniversalPipeline"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull [_CullMode]
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "UIBlur"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 2.5

            #pragma multi_compile _ UNITY_UI_CLIP_RECT
            #pragma multi_compile _ UNITY_UI_ALPHACLIP

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                half4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 mask : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                half4 _TextureSampleAdd;
                float4 _ClipRect;
                float _UIMaskSoftnessX;
                float _UIMaskSoftnessY;
                half _Blur;
            CBUFFER_END

            Varyings Vert(Attributes v)
            {
                Varyings o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                float4 vPosition = TransformObjectToHClip(v.vertex.xyz);
                o.positionCS = vPosition;

                float2 pixelSize = vPosition.w;
                pixelSize /= abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

                float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
                o.uv = v.texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
                o.mask = half4(
                    v.vertex.xy * 2 - clampedRect.xy - clampedRect.zw,
                    0.25 / (0.25 * half2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy)));

                o.color = v.color * _Color;
                return o;
            }

            // Single-pass 2D Gaussian-ish blur (3x3 kernel, strength scales sample radius).
            half4 Frag(Varyings i) : SV_Target
            {
                half blurPx = max(_Blur, (half)0.0001);
                float2 ts = _MainTex_TexelSize.xy * blurPx;

                half4 c =
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + float2(-ts.x, -ts.y)) * 0.0625h +
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + float2(0, -ts.y)) * 0.125h +
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + float2(ts.x, -ts.y)) * 0.0625h +
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + float2(-ts.x, 0)) * 0.125h +
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv) * 0.25h +
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + float2(ts.x, 0)) * 0.125h +
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + float2(-ts.x, ts.y)) * 0.0625h +
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + float2(0, ts.y)) * 0.125h +
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + float2(ts.x, ts.y)) * 0.0625h;

                c += _TextureSampleAdd;
                c *= i.color;

                #if defined(UNITY_UI_CLIP_RECT)
                half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(i.mask.xy)) * i.mask.zw);
                c *= m.x * m.y;
                #endif

                #if defined(UNITY_UI_ALPHACLIP)
                clip(c.a - 0.001h);
                #endif

                return c;
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/InternalErrorShader"
}
