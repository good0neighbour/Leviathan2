Shader "Leviathan/sh_toonOutline"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Color ("ShadowColour", Color) = (0.5,0.5,0.5,1)
        [Space(16)]
        _RimLightColour ("RimLightColour", Color) = (1,1,1,1)
        _RimLightWidth ("RimLightWidth", Range(0,1)) = 0.9
        _RimLightAppearing ("RimLightAppearing", Range(0,5)) = 1
        _RimLightStrength ("RimLightStrength", Range(0,10)) = 1
        [Space(16)]
        _OutlineColour ("OutlineColour", Color) = (0,0,0,1)
        _OutlineWidth ("OustlineWidth", Range(0,0.1)) = 0.05
        _OutlineMaxWidth ("OutlineMaxWidth", Range(0,0.5)) = 0.05
        [Space(16)]
        _DissolveTexture ("DissolveTexture", 2D) = "white" {}
        _DissolveAmount ("DissolveAmount", Range(0,1)) = 0
        [HDR]_DissolveOutlineColour ("DissolveOutlineColour", Color) = (1,1,1,1)
        _DissolveOutlineWidth ("DissolveOutlineWidth", Range(0,0.5)) = 0.2
        [Space(16)]
        _StealthTexture ("StealthTexture", 2D) = "white" {}
        _StealthWidth ("StealthWidth", Range(1,20)) = 5
        [Space(16)]
        _Cutoff ("Cutoff", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        LOD 200



        // 1 pass ¿Ü°û¼± ·»´õ¸µ

        cull front
        CGPROGRAM
        #pragma surface surf NoLight noshadow noambient noforwardadd nolightmap novertexlight vertex:vert alpha:fade
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _DissolveTexture;

        struct Input
        {
            fixed2 uv_MainTex;
        };

        fixed4 _OutlineColour;
        fixed _OutlineWidth;
        fixed _OutlineMaxWidth;
        fixed _DissolveAmount;
        fixed4 _DissolveOutlineColour;
        fixed _DissolveOutlineWidth;

        void vert(inout appdata_full tV)
        {
            tV.vertex.xyz = tV.vertex.xyz + tV.normal.xyz * min(_OutlineMaxWidth, _OutlineWidth * UnityObjectToClipPos(tV.vertex).w);
        }

        void surf (Input IN, inout SurfaceOutput tOutput)
        {
            fixed4 tDissolveTexture = tex2D (_DissolveTexture, fixed2(IN.uv_MainTex.x + _Time.x, IN.uv_MainTex.y + _Time.x));
            tOutput.Alpha = step(_DissolveAmount, tDissolveTexture.r);
            tOutput.Emission = _DissolveOutlineColour.rgb * (1 - step(_DissolveAmount + _DissolveOutlineWidth, tDissolveTexture.r));
        }

        inline fixed4 LightingNoLight(SurfaceOutput tOutput, fixed3 tLightDir, fixed3 tViewDir, fixed tAtten)
        {
            fixed4 tResult;
            tResult.rgb = _OutlineColour.rgb;
            tResult.a = tOutput.Alpha;
            return tResult;
        }
        ENDCG



        // 2 pass ±íÀÌ°ª ±â·Ï¿ë

        zwrite on
        cull back
        colormask 0
        CGPROGRAM
        #pragma surface surf NoCaculation noshadow noambient noforwardadd nolightmap novertexlight
        #pragma target 3.0

        struct Input
        {
            fixed2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput tOutput)
        {
        }

        inline fixed4 LightingNoCaculation(SurfaceOutput tOutput, fixed3 tLightDir, fixed3 tViewDir, fixed tAtten)
        {
            return fixed4(0,0,0,0);
        }
        ENDCG



        // 3 pass º»Ã¼ ·»´õ¸µ

        zwrite off
        CGPROGRAM
        #pragma surface surf ToonShade noambient noforwardadd nolightmap novertexlight alpha:fade
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _DissolveTexture;
        sampler2D _StealthTexture;

        struct Input
        {
            fixed2 uv_MainTex;
        };

        fixed4 _Color;
        fixed4 _RimLightColour;
        fixed _RimLightWidth;
        fixed _RimLightAppearing;
        fixed _RimLightStrength;
        fixed _DissolveAmount;
        fixed4 _DissolveOutlineColour;
        fixed _DissolveOutlineWidth;
        fixed _StealthWidth;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutput tOutput)
        {
            fixed4 tMainTexture;

            // Albedo
            tMainTexture.rgb = tex2D (_MainTex, IN.uv_MainTex).rgb;
            tOutput.Albedo = tMainTexture.rgb;

            // Dissolve
            tMainTexture.a = tex2D (_DissolveTexture, fixed2(IN.uv_MainTex.x + _Time.x, IN.uv_MainTex.y + _Time.x)).r;
            tOutput.Alpha = step(_DissolveAmount, tMainTexture.a);

            // DissolveOutline
            tOutput.Emission = _DissolveOutlineColour.rgb * (1 - step(_DissolveAmount + _DissolveOutlineWidth, tMainTexture.a)) * tOutput.Alpha;

            _DissolveOutlineColour.a = tex2D (_StealthTexture, fixed2(IN.uv_MainTex.x + _Time.x * 0.1, IN.uv_MainTex.y - _Time.x)).r * 10;
        }

        fixed4 LightingToonShade(SurfaceOutput tOutput, fixed3 tLightDir, fixed3 tViewDir, fixed tAtten)
        {
            fixed4 tResult;
            fixed2 tTemp;

            // Ä«Å÷ ·»´õ¸µ
            tTemp.x = step(0.2, dot(tLightDir, tOutput.Normal));
            tResult.rgb = tOutput.Albedo * (tTemp.x + _Color * (1 - tTemp.x));
            
            // ¸²¶óÀÌÆ® Ä«Å÷ ·»´õ¸µ ¹öÀü
            tTemp.x = 1 - dot(tViewDir, tOutput.Normal);
            tTemp.y = step(_RimLightWidth, tTemp.x) * pow(saturate(-dot(tLightDir, tViewDir)), _RimLightAppearing);
            tResult.rgb = tResult.rgb * (1 + tTemp.y * _RimLightColour.rgb * _RimLightStrength);

            // Åõ¸íÈ­ ½Ã
            tTemp.y = pow(tTemp.x, _StealthWidth) * _DissolveOutlineColour.a;
            tResult.rgb = tResult.rgb * tOutput.Alpha + _DissolveOutlineColour.rgb * (1 - tOutput.Alpha);

            tResult.a = saturate(tOutput.Alpha + tTemp.y);
            return tResult;
        }
        ENDCG
    }
    FallBack "Legacy Shaders/Transparent/Cutout/VertexLit"
}
