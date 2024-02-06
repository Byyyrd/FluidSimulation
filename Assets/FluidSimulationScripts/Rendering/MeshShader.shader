Shader "Custom/InstancedColor" {
     Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
    }
    
    SubShader {
       Tags
        {
            "Queue" = "Transparent"
            "RenderType"="Transparent"
        }
        // No culling or depth
        Cull Off ZWrite Off ZTest Always 
        Blend SrcAlpha OneMinusSrcAlpha


        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
            }; 

            float4 _Colors[1023];   // Max instanced batch size.

            v2f vert(appdata_t i, uint instanceID: SV_InstanceID) {
                // Allow instancing.
                UNITY_SETUP_INSTANCE_ID(i);

                v2f o;
                o.vertex = UnityObjectToClipPos(i.vertex);
                o.color = float4(1, 1, 1, 1);
                o.uv = i.uv;
                // If instancing on (it should be) assign per-instance color.
                #ifdef UNITY_INSTANCING_ENABLED
                    o.color = _Colors[instanceID];
                #endif

                return o;
            }
            sampler2D _MainTex;
            fixed4 frag(v2f i) : SV_Target {
                return i.color * tex2D(_MainTex, i.uv);
            }

            ENDCG
        }
    }
}