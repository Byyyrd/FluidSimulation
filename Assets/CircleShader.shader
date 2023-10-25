Shader "Hidden/CircleShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Center("center",vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType"="Transparent"
        }
        // No culling or depth
        Cull Off ZWrite Off ZTest Always 
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float3 center;
            float radius;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 position : TEXCOORD1;
                fixed4 color : COLOR0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.position = v.vertex;
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            sampler2D _MainTex;
            float4 _Center;
            fixed4 frag (v2f i) : SV_Target
            {
                if(distance(_Center.xy,i.position.xy) > radius){
                    i.color.a = 0;
                }
                return i.color;
            }
            ENDCG
        }
    }
}
