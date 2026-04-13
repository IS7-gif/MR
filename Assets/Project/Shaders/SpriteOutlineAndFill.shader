Shader "Custom/SpriteOutlineAndFill"
{
    Properties
    {
        [PerRendererData] [HideInInspector] _MainTex("Sprite Texture", 2D) = "white" {}
        _FillEnabled("Fill Enabled", Range(0, 1)) = 0
        _FillColor("Fill Color", Color) = (1, 1, 1, 1)
        _FillReplace("Fill Replace", Range(0, 1)) = 0
        _OutlineEnabled("Outline Enabled", Range(0, 1)) = 0
        _OutlineColor("Outline Color", Color) = (1, 1, 1, 1)
        _OutlineWidth("Outline Width", Range(0, 50)) = 2
        _AlphaCutoff("Alpha Cutoff", Range(0, 1)) = 0.999
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
            float4 _FillColor;
            float4 _OutlineColor;
            float _FillEnabled;
            float _FillReplace;
            float _OutlineEnabled;
            float _OutlineWidth;
            float _AlphaCutoff;
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
            float4 color : COLOR;
        };

        Varyings vert(Attributes input)
        {
            Varyings output;
            output.positionCS = TransformObjectToHClip(input.positionOS);
            output.uv = TRANSFORM_TEX(input.uv, _MainTex);
            output.color = input.color;
            return output;
        }

        float SampleSpriteAlpha(float2 uv)
        {
            return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).a;
        }

        float GetAlphaCutoff()
        {
            return saturate(_AlphaCutoff);
        }

        float IsAlphaInside(float alpha)
        {
            float alphaCutoff = GetAlphaCutoff();

            if (alphaCutoff <= 0.0)
                return alpha > 0.0 ? 1.0 : 0.0;

            return alpha >= alphaCutoff ? 1.0 : 0.0;
        }

        float SampleOpaqueMask(float2 uv)
        {
            return IsAlphaInside(SampleSpriteAlpha(uv));
        }

        float IsInsideSprite(float alpha)
        {
            return IsAlphaInside(alpha);
        }

        float IsFillEnabled()
        {
            return step(0.5, _FillEnabled);
        }

        float IsOutlineEnabled()
        {
            return step(0.5, _OutlineEnabled);
        }

        float4 GetFilledSpriteColor(float4 spriteSample, float4 vertexColor)
        {
            float4 multiplyColor = float4(spriteSample.rgb * _FillColor.rgb, _FillColor.a);
            float4 replaceColor = _FillColor;
            float fillReplace = step(0.5, _FillReplace);
            float4 filledColor = lerp(multiplyColor, replaceColor, fillReplace);
            filledColor *= vertexColor;
            return filledColor;
        }

        float GetOutlineMask(float2 uv)
        {
            float outlineWidth = max(_OutlineWidth, 0.0);

            if (outlineWidth <= 0.001)
                return 0.0;

            float2 uvDx = ddx(uv);
            float2 uvDy = ddy(uv);
            float maxAlpha = 0.0;
            float maxSteps = ceil(outlineWidth);
            const int directionCount = 16;
            const int maxStepCount = 50;

            [unroll]
            for (int stepIndex = 1; stepIndex <= maxStepCount; stepIndex++)
            {
                float enabled = stepIndex <= maxSteps ? 1.0 : 0.0;
                float distancePixels = min((float)stepIndex, outlineWidth);

                [unroll]
                for (int directionIndex = 0; directionIndex < directionCount; directionIndex++)
                {
                    float angle = 6.28318530718 * ((float)directionIndex / (float)directionCount);
                    float2 direction = float2(cos(angle), sin(angle));
                    float2 sampleUv = uv + uvDx * direction.x * distancePixels + uvDy * direction.y * distancePixels;
                    float sampleAlpha = SampleOpaqueMask(sampleUv) * enabled;
                    maxAlpha = max(maxAlpha, sampleAlpha);
                }
            }

            return maxAlpha;
        }

        float4 frag(Varyings input) : SV_Target
        {
            float4 spriteSample = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
            float4 spriteColor = spriteSample * input.color;
            float fillEnabled = IsFillEnabled();
            float outlineEnabled = IsOutlineEnabled();
            float spriteOpaqueMask = IsInsideSprite(spriteSample.a);

            if (fillEnabled > 0.5 && spriteOpaqueMask > 0.5)
                return GetFilledSpriteColor(spriteSample, input.color);

            if (spriteSample.a > 0.0)
                return spriteColor;

            if (outlineEnabled <= 0.5)
                return 0.0;

            float outlineMask = GetOutlineMask(input.uv);
            float4 outlineColor = _OutlineColor * input.color;
            outlineColor.a *= outlineMask;
            return outlineColor;
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