Shader "Paleontology/UI/Layered Rock"
{
    Properties
    {
        _RockTex ("Weathered rock", 2D) = "white" {}
        _SedimentTex ("Sediment", 2D) = "white" {}
        _FractureTex ("Fresh fracture", 2D) = "white" {}
        _SedimentAmount ("Sediment coverage", Range(0, 1)) = 0.68
        _FractureWidth ("Fracture width", Range(0.04, 0.26)) = 0.20
        _ThinCover ("Thin patch transparency", Range(0, 1)) = 0
        _ThinCoverArea ("Thin patch area (center XY, radii ZW)", Vector) = (0.5, 0.5, 0.5, 0.5)
        [HideInInspector] _DigMask ("Remaining cells", 2D) = "white" {}
        [PerRendererData] _MainTex ("UI texture", 2D) = "white" {}
        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        Cull Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend One OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            sampler2D _RockTex, _SedimentTex, _FractureTex, _DigMask;
            float4 _DigMask_TexelSize, _ClipRect, _ThinCoverArea;
            half _SedimentAmount, _FractureWidth, _ThinCover;

            struct Attributes
            {
                float4 position : POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Interpolators
            {
                float4 position : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                float2 localPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Interpolators Vert(Attributes input)
            {
                Interpolators output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                output.position = UnityObjectToClipPos(input.position);
                output.localPosition = input.position.xy;
                output.uv = input.uv;
                output.color = input.color;
                return output;
            }

            float Hash(float2 p)
            {
                p = frac(p * float2(0.1031, 0.11369));
                p += dot(p, p.yx + 19.19);
                return frac((p.x + p.y) * p.x);
            }

            float Noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);
                return lerp(lerp(Hash(i), Hash(i + float2(1, 0)), f.x),
                            lerp(Hash(i + float2(0, 1)), Hash(i + 1), f.x), f.y);
            }

            fixed4 Frag(Interpolators input) : SV_Target
            {
                float2 uv = input.uv;
                float2 grid = uv * _DigMask_TexelSize.zw;
                // Displace the contour at two scales while keeping the center of each cell stable.
                float2 contour = float2(Noise(grid * 1.8), Noise(grid * 1.8 + 13.7)) - 0.5;
                float2 grit = float2(Noise(grid * 6.4 + 7.2), Noise(grid * 6.4 + 37.0)) - 0.5;
                float2 maskUV = (grid + 0.58 * contour + 0.23 * grit) * _DigMask_TexelSize.xy;
                half remaining = tex2D(_DigMask, maskUV).r;

                half top = smoothstep(0.52, 0.64, remaining);
                half baseEdge = 0.52 - _FractureWidth;
                half fracture = smoothstep(baseEdge - 0.055, baseEdge + 0.055, remaining);
                half shadow = 0.48 * smoothstep(0.065, baseEdge, remaining);

                float patches = 0.8 * Noise(uv * float2(8, 5) + 4.1)
                              + 0.2 * Noise(uv * float2(27, 18) + 9.0);
                half sediment = _SedimentAmount * smoothstep(0.27, 0.70, patches);
                half3 rock = tex2D(_RockTex, uv).rgb;
                half3 silt = tex2D(_SedimentTex, uv).rgb;
                half3 surface = lerp(rock, silt, sediment);

                half towardLight = tex2D(_DigMask, maskUV + float2(-0.23, 0.33) * _DigMask_TexelSize.xy).r;
                half rimLight = clamp(0.72 + 1.35 * (remaining - towardLight), 0.42, 1.22);
                half3 brokenRock = tex2D(_FractureTex, uv).rgb * rimLight;

                // Sparse deposits let the underlying bones show through the intact surface.
                float2 offsetFromFinds = (uv - _ThinCoverArea.xy) / max(_ThinCoverArea.zw, 0.001);
                half nearFinds = 1.0 - smoothstep(0.42, 1.0, dot(offsetFromFinds, offsetFromFinds));
                half thinness = (1.0 - smoothstep(0.22, 0.55, patches)) * nearFinds;
                half coverage = input.color.a * (1.0 - _ThinCover * thinness);
                half opacity = max(fracture, shadow) * coverage;
                #ifdef UNITY_UI_CLIP_RECT
                opacity *= UnityGet2DClipping(input.localPosition, _ClipRect);
                #endif
                #ifdef UNITY_UI_ALPHACLIP
                clip(opacity - 0.001);
                #endif
                half3 color = lerp(brokenRock, surface, top) * fracture * input.color.rgb * coverage;
                #ifdef UNITY_UI_CLIP_RECT
                color *= UnityGet2DClipping(input.localPosition, _ClipRect);
                #endif
                return half4(color, opacity);
            }
            ENDCG
        }
    }
}
