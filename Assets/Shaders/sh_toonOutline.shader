Shader "Leviathan/sh_toonOutline"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _DeepShadowColour ("ShadowColour", Color) = (0.5,0.5,0.5,1)
        [Space(16)]
        _RimLightColour ("RimLightColour", Color) = (1,1,1,1)
        _RimLightWidth ("RimLightWidth", Range(0,1)) = 0.9
        _RimLightAppearing ("RimLightAppearing", Range(0,5)) = 1
        _RimLightStrength ("RimLightStrength", Range(0,10)) = 1
        [Space(16)]
        _OutlineColour ("OutlineColour", Color) = (0,0,0,1)
        _OutlineWidth ("OustlineWidth", Range(0,0.1)) = 0.05
        _OutlineMaxWidth ("OutlineMaxWidth", Range(0,0.1)) = 0.05
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        // ¿Ü°û¼± ·»´õ¸µ
        cull front
        CGPROGRAM
        #pragma surface surf NoLight noshadow noambient noforwardadd nolightmap novertexlight vertex:vert
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            fixed2 uv_MainTex;
        };

        fixed4 _OutlineColour;
        fixed _OutlineWidth;
        fixed _OutlineMaxWidth;

        struct NoLightOutput
        {
            fixed3 Albedo;
            fixed3 Normal;
            fixed3 Emission;
            fixed Alpha;
        };

        void vert(inout appdata_full tV)
        {
            tV.vertex.xyz = tV.vertex.xyz + tV.normal.xyz * min(_OutlineMaxWidth, _OutlineWidth * UnityObjectToClipPos(tV.vertex).w);
        }

        void surf (Input IN, inout NoLightOutput tOutput)
        {
        }

        inline fixed4 LightingNoLight(NoLightOutput tOutput, fixed3 tLightDir, fixed3 tViewDir, fixed tAtten)
        {
            fixed4 tResult = _OutlineColour;
            return tResult;
        }
        ENDCG

        // º»Ã¼ ·»´õ¸µ
        cull off
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf ToonShade noambient noforwardadd nolightmap novertexlight

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            fixed2 uv_MainTex;
        };

        fixed4 _DeepShadowColour;
        fixed4 _RimLightColour;
        fixed _RimLightWidth;
        fixed _RimLightAppearing;
        fixed _RimLightStrength;

        struct ToonShadeOutput
        {
            fixed3 Albedo;
            fixed3 Normal;
            fixed3 Emission;
            fixed Alpha;
        };

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout ToonShadeOutput tOutput)
        {
            fixed4 tMainTexture = tex2D (_MainTex, IN.uv_MainTex);
            tOutput.Albedo = tMainTexture.rgb;
        }

        fixed4 LightingToonShade(ToonShadeOutput tOutput, fixed3 tLightDir, fixed3 tViewDir, fixed tAtten)
        {
            fixed4 tResult;
            fixed2 tTemp;

            // Ä«Å÷ ·»´õ¸µ
            tTemp.x = step(0.2, dot(tLightDir, tOutput.Normal));
            tResult.rgb = tOutput.Albedo * (tTemp.x + _DeepShadowColour * (1 - tTemp.x));
            
            // ¸²¶óÀÌÆ® Ä«Å÷ ·»´õ¸µ ¹öÀü
            tTemp.y = step(_RimLightWidth, 1 - dot(tViewDir, tOutput.Normal)) * pow(saturate(-dot(tLightDir, tViewDir)), _RimLightAppearing);
            tResult.rgb = tResult.rgb * (1 + tTemp.y * _RimLightColour.rgb * _RimLightStrength);

            tResult.a = 1.0;
            return tResult;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
