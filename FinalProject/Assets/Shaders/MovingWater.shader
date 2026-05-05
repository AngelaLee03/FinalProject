Shader "Custom/MovingWaterr"
{
    Properties
    {
        _MainTex ("Water Texture", 2D) = "white" {}
        _Tint ("Water Tint", Color) = (0.2, 0.6, 1.0, 1.0)

        _ScrollSpeedX ("Scroll Speed X", Float) = 0.05
        _ScrollSpeedY ("Scroll Speed Y", Float) = 0.03

        _WaveStrength ("Wave Strength", Float) = 0.03
        _WaveFrequency ("Wave Frequency", Float) = 12.0
        _WaveSpeed ("Wave Speed", Float) = 2.0
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Tint;

            float _ScrollSpeedX;
            float _ScrollSpeedY;
            float _WaveStrength;
            float _WaveFrequency;
            float _WaveSpeed;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.position = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float time = _Time.y;

                float2 uv = i.uv;

                uv.x += time * _ScrollSpeedX;
                uv.y += time * _ScrollSpeedY;

                uv.x += sin((i.uv.y * _WaveFrequency) + time * _WaveSpeed) * _WaveStrength;
                uv.y += cos((i.uv.x * _WaveFrequency) + time * _WaveSpeed) * _WaveStrength;

                fixed4 col = tex2D(_MainTex, uv);

                col *= _Tint;

                return col;
            }

            ENDCG
        }
    }
}