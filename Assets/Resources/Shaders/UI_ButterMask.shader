Shader "UI/ButterMask"
{
    Properties
    {
        _MainTex ("Butter Pan Texture", 2D) = "white" {}
        _MaskTex ("Mask Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
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
        }

        Cull Off
        ZWrite Off
        Lighting Off
        Blend SrcAlpha OneMinusSrcAlpha

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
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            sampler2D _MaskTex;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 버터가 발린 프라이팬 텍스처 색상
                fixed4 butter = tex2D(_MainTex, i.uv) * _Color;

                // RenderTexture 알파 (드래그로 칠한 영역)
                fixed4 mask = tex2D(_MaskTex, i.uv);

                // 알파값이 있는 부분만 버터 이미지 보이게
                butter.a *= mask.a;

                return butter;
            }
            ENDCG
        }
    }
}