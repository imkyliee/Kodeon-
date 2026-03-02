Shader "URP/CelShading/FinalCelShaded_URP"
{
    Properties
    {
        _MainTex ("Albedo", 2D) = "white" {}
        _BumpMap("Normal Map", 2D) = "bump" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
        _Antialiasing("Band Smoothing", Float) = 5.0
        _Glossiness("Glossiness/Shininess", Float) = 400
        _Fresnel("Fresnel/Rim Amount", Range(0, 1)) = 0.5
        _OutlineSize("Outline Size", Float) = 0.01
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }

        // =====================================================
        // MAIN TOON PASS
        // =====================================================
        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode"="UniversalForward" }

            Stencil
            {
                Ref 1
                Comp Always
                Pass Replace
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float4 tangentOS  : TANGENT;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS    : TEXCOORD0;
                float3 viewDirWS   : TEXCOORD1;
                float2 uv          : TEXCOORD2;
                float3 positionWS  : TEXCOORD3;
                float4 tangentWS   : TEXCOORD4;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_BumpMap);
            SAMPLER(sampler_BumpMap);

            float4 _Color;
            float _Antialiasing;
            float _Glossiness;
            float _Fresnel;

            Varyings vert (Attributes v)
            {
                Varyings o;

                VertexPositionInputs posInputs = GetVertexPositionInputs(v.positionOS.xyz);
                VertexNormalInputs normInputs = GetVertexNormalInputs(v.normalOS, v.tangentOS);

                o.positionHCS = posInputs.positionCS;
                o.positionWS = posInputs.positionWS;
                o.normalWS = normInputs.normalWS;
                o.tangentWS = float4(normInputs.tangentWS, v.tangentOS.w);
                o.viewDirWS = GetWorldSpaceViewDir(posInputs.positionWS);
                o.uv = v.uv;

                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float3 normalTS = UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, i.uv));

                float3 bitangent = cross(i.normalWS, i.tangentWS.xyz) * i.tangentWS.w;
                float3x3 TBN = float3x3(i.tangentWS.xyz, bitangent, i.normalWS);

                float3 normalWS = normalize(mul(normalTS, TBN));
                float3 viewDir = normalize(i.viewDirWS);

                Light mainLight = GetMainLight();
                float3 lightDir = normalize(mainLight.direction);

                float diffuse = dot(normalWS, lightDir);
                float delta = fwidth(diffuse) * _Antialiasing;
                float diffuseSmooth = smoothstep(0, delta, diffuse);

                float3 halfVec = normalize(lightDir + viewDir);
                float specular = pow(saturate(dot(normalWS, halfVec)) * diffuseSmooth, _Glossiness);
                float specularSmooth = smoothstep(0, 0.01 * _Antialiasing, specular);

                float rim = 1 - saturate(dot(normalWS, viewDir));
                rim *= pow(diffuse, 0.3);
                float fresnelSize = 1 - _Fresnel;
                float rimSmooth = smoothstep(fresnelSize, fresnelSize * 1.1, rim);

                float3 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv).rgb * _Color.rgb;

                float3 color = albedo *
                    ((diffuseSmooth + specularSmooth + rimSmooth) * mainLight.color +
                     SampleSH(normalWS));

                return float4(color, 1);
            }

            ENDHLSL
        }

        // =====================================================
        // OUTLINE PASS (INVERTED HULL)
        // =====================================================
        Pass
        {
            Name "Outline"
            Tags { "LightMode"="SRPDefaultUnlit" }

            Cull Front
            ZWrite Off

            Stencil
            {
                Ref 1
                Comp NotEqual
                Pass Replace
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float _OutlineSize;
            float4 _OutlineColor;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            Varyings vert (Attributes v)
            {
                Varyings o;

                float3 normalWS = TransformObjectToWorldNormal(v.normalOS);
                float3 posWS = TransformObjectToWorld(v.positionOS.xyz);
                posWS += normalWS * _OutlineSize;

                o.positionHCS = TransformWorldToHClip(posWS);
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                return _OutlineColor;
            }

            ENDHLSL
        }
    }
}