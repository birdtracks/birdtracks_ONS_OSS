Shader "Custom/CircularFauxShadow"
{
    Properties
    {
        _ShadowColor ("Shadow Color", Color) = (0, 0, 0, 0.5)
        _Radius ("Shadow Radius", Range(0.1, 2.0)) = 1.0
        _EdgeSharpness ("Edge Sharpness", Range(1.0, 10.0)) = 3.0
        _InnerOpacity ("Inner Opacity", Range(0.0, 1.0)) = 0.8
        _OuterFalloff ("Outer Falloff", Range(0.1, 2.0)) = 0.5
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            float4 _ShadowColor;
            float _Radius;
            float _EdgeSharpness;
            float _InnerOpacity;
            float _OuterFalloff;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * 2 - 1; // Convert UVs to -1 to 1 range
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // Calculate distance from center in UV space
                float distFromCenter = length(i.uv);
                
                // Create base circular mask
                float circleMask = 1 - smoothstep(_Radius - _OuterFalloff, _Radius, distFromCenter);
                
                // Create enhanced edge falloff
                float edgeFactor = pow(1 - distFromCenter / _Radius, _EdgeSharpness);
                
                // Combine inner and outer opacity
                float finalOpacity = lerp(_InnerOpacity, 0, pow(distFromCenter / _Radius, _EdgeSharpness));
                finalOpacity *= circleMask;
                
                // Apply radial gradient for more realistic shadow falloff
                float radialGradient = 1 - pow(distFromCenter / _Radius, 2);
                finalOpacity *= radialGradient;
                
                // Create final color with modified alpha
                float4 finalColor = _ShadowColor;
                finalColor.a *= finalOpacity;
                
                return finalColor;
            }
            ENDHLSL
        }
    }
}