Shader "MyShader/ScrollingBackground" {
    Properties{
        _MainTex("FirstLayer", 2D) = "" {}
        _SecondTex("SecondLayer", 2D) = "" {}
        _ThirdTex("ThirdLayer", 2D) = "" {}
        _FourthTex("FourthLayer", 2D) = "" {}
        _Speed("speed for each layer", Vector) = (0,0,0,0)
        _Bias("bias for input", Range(0,10)) = 0
    }
        SubShader
        {
            Pass
            {
                ZWrite off
                Blend SrcAlpha OneMinusSrcAlpha

                CGPROGRAM
                #include "UnityCG.cginc"
                #pragma vertex vert
                #pragma fragment frag

                fixed4 _Color;
                sampler2D _MainTex;
                sampler2D _SecondTex;
                sampler2D _ThirdTex;
                sampler2D _FourthTex;

                float4 _MainTex_ST;
                float4 _SecondTex_ST;
                float4 _ThirdTex_ST;
                float4 _FourthTex_ST;

                float4 _Speed;
                float  _Bias;

                struct a2v
                {
                    float4 vertex : POSITION;
                    float4 texcoord : TEXCOORD0;
                };

                struct v2f
                {
                    float4 pos : SV_POSITION;
                    float4 uv : TEXCOORD0;
                    float4 uv2 : TEXCOORD1;
                };

                v2f vert(a2v v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    //µÈÍ¬ÓÚ o.uv.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                    o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex) + frac(float2(_Speed.x,0.0) * _Bias);
                    o.uv.zw = TRANSFORM_TEX(v.texcoord, _SecondTex) + frac(float2(_Speed.y,0.0) * _Bias);
                    o.uv2.xy = TRANSFORM_TEX(v.texcoord, _ThirdTex) + frac(float2(_Speed.z,0.0) * _Bias);
                    o.uv2.zw = TRANSFORM_TEX(v.texcoord, _FourthTex) + frac(float2(_Speed.w,0.0) * _Bias);

                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 firstLayer = tex2D(_MainTex,i.uv.xy);
                    fixed4 secondLayer = tex2D(_SecondTex, i.uv.zw);
                    fixed4 thirdLayer = tex2D(_ThirdTex, i.uv2.xy);
                    fixed4 fourthLayer = tex2D(_FourthTex, i.uv2.zw);
                    fixed4 c = lerp(firstLayer, secondLayer, secondLayer.a);
                    c = lerp(c, thirdLayer, thirdLayer.a);
                    c = lerp(c, fourthLayer, fourthLayer.a);

                    return c;
                }

                ENDCG
            }
        }
}