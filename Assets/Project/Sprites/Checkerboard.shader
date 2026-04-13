Shader "Custom/2D Checkerboard"
{
    Properties
    {
        [PerRendererData] [HideInInspector] _MainTex("Sprite Texture", 2D) = "white" {}
        _CellSize("Cell Size", Float) = 1
        _ColorA("Color A", Color) = (1, 1, 1, 1)
        _ColorB("Color B", Color) = (0, 0, 0, 1)
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);

        CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            float4 _ColorA;
            float4 _ColorB;
            float _CellSize;
        CBUFFER_END

        struct Attributes
        {
            float3 positionOS : POSITION;
            float2 uv : TEXCOORD0;
            float4 color : COLOR;
        };

        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float2 uv : TEXCOORD0;
            float2 patternPos : TEXCOORD1;
            float4 color : COLOR;
        };

        float2 GetObjectScale()
        {
            float2 scale;
            scale.x = length(float3(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0]));
            scale.y = length(float3(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1]));
            return scale;
        }

        Varyings vert(Attributes input)
        {
            Varyings output;
            float2 objectScale = GetObjectScale();
            output.positionCS = TransformObjectToHClip(input.positionOS);
            output.uv = TRANSFORM_TEX(input.uv, _MainTex);
            output.patternPos = input.positionOS.xy * objectScale;
            output.color = input.color;
            return output;
        }

        float GetCheckerValue(float2 patternPos)
        {
            float cellSize = max(_CellSize, 0.0001);
            float2 cellCoords = floor(patternPos / cellSize);
            return frac((cellCoords.x + cellCoords.y) * 0.5) * 2.0;
        }

        float4 frag(Varyings input) : SV_Target
        {
            float checker = GetCheckerValue(input.patternPos);
            float4 checkerColor = checker < 0.5 ? _ColorA : _ColorB;
            float4 spriteSample = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
            float4 finalColor = checkerColor;
            finalColor.rgb *= spriteSample.rgb * input.color.rgb;
            finalColor.a *= spriteSample.a * input.color.a;
            return finalColor;
        }
        ENDHLSL

        Pass
        {
            Name "Universal2D"
            Tags { "LightMode" = "Universal2D" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDHLSL
        }

        Pass
        {
            Name "SpriteUnlit"
            Tags { "LightMode" = "SRPDefaultUnlit" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDHLSL
        }
    }

    FallBack "Sprites/Default"
}
