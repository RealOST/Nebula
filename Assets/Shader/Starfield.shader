Shader "Unlit/3DStarfield"
{
    Properties
    {
        _MainTex ("Background", 2D) = "black" {}
        _LayerCount ("Layer Count", Range(1, 8)) = 4
        _StarDensity ("Star Density", Range(0, 1)) = 0.3
        _BaseSpeed ("Base Speed", Range(0, 2)) = 0.5
        _DepthScale ("Depth Scale", Range(0, 5)) = 1.2
        _SizeVariation ("Size Variation", Range(0, 1)) = 0.5
        _TwinkleSpeed ("Twinkle Speed", Range(0, 2)) = 0.3
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            int _LayerCount;
            float _StarDensity;
            float _BaseSpeed;
            float _DepthScale;
            float _SizeVariation;
            float _TwinkleSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float random(float2 seed)
            {
                return frac(sin(dot(seed, float2(12.9898,78.233))) * 43758.5453);
            }

            float3 renderStarLayer(float2 uv, float depth, float time)
            {
                // 生成随机星位
                float2 grid = floor(uv * 100 * (1 + depth * 3));
                float rnd = random(grid + depth);
                if(rnd > _StarDensity) return float3(0,0,0);

                // 计算动态属性
                float speed = _BaseSpeed * pow(_DepthScale, depth);
                float size = lerp(0.002, 0.02, rnd) * (1 + _SizeVariation * sin(time * _TwinkleSpeed + rnd*10));
                float brightness = lerp(0.3, 1.0, 1 - depth) * (0.8 + 0.2 * sin(time * 2 + rnd*5));

                // 运动偏移
                float2 offset = float2(
                    frac(uv.x + time * speed * (0.5 + rnd)), 
                    frac(uv.y + time * speed * (0.3 + rnd*0.7))
                );

                // 透视变换
                float2 starPos = (frac(offset * 10) - 0.5) * 2;
                starPos *= 1 + depth * 2; // 近大远小

                // 绘制星体
                float dist = length(starPos);
                float star = smoothstep(size*1.2, size*0.8, dist);
                star *= smoothstep(1.0, 0.8, dist*2); // 辉光效果

                // 颜色混合
                float3 color = lerp(
                    float3(0.4, 0.5, 1.0), 
                    float3(1.0, 0.9, 0.8), 
                    pow(1 - depth, 3)
                ) * brightness;

                return color * star;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float time = _Time.y;
                float2 uv = i.uv * 2 - 1; // 转换到[-1,1]坐标系

                // 多层混合
                float3 color = tex2D(_MainTex, i.uv).rgb;
                for(int j = 0; j < _LayerCount; j++)
                {
                    float depth = (float)j / _LayerCount;
                    color += renderStarLayer(i.uv, depth, time);
                }

                return fixed4(color, 1.0);
            }
            ENDCG
        }
    }
}