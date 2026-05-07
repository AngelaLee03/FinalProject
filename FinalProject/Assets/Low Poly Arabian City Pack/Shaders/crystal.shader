// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.35 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.35;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:4013,x:33488,y:32700,varname:node_4013,prsc:2|emission-2127-OUT,alpha-7175-OUT;n:type:ShaderForge.SFN_Color,id:1304,x:32821,y:32853,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_1304,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.8068964,c3:0,c4:1;n:type:ShaderForge.SFN_Tex2d,id:4862,x:32821,y:32656,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_4862,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:fdbfbfbf26c7f524cb4c22527f825466,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:1472,x:33073,y:32696,varname:node_1472,prsc:2|A-4862-RGB,B-1304-RGB;n:type:ShaderForge.SFN_ViewVector,id:457,x:32279,y:33152,varname:node_457,prsc:2;n:type:ShaderForge.SFN_Negate,id:6441,x:32473,y:33039,varname:node_6441,prsc:2|IN-457-OUT;n:type:ShaderForge.SFN_Reflect,id:3116,x:32642,y:33127,varname:node_3116,prsc:2|A-6441-OUT,B-8882-OUT;n:type:ShaderForge.SFN_NormalVector,id:8882,x:32473,y:33221,prsc:2,pt:False;n:type:ShaderForge.SFN_Cubemap,id:5354,x:32821,y:33043,ptovrint:False,ptlb:Reflection,ptin:_Reflection,varname:node_5354,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,cube:3c775a3f62779094390b037591a6da5c,pvfc:2|DIR-3116-OUT;n:type:ShaderForge.SFN_Multiply,id:2186,x:33067,y:32991,varname:node_2186,prsc:2|A-5354-RGB,B-1445-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1445,x:32821,y:33236,ptovrint:False,ptlb:Reflection Force,ptin:_ReflectionForce,varname:node_1445,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:7175,x:33083,y:32835,varname:node_7175,prsc:2|A-4862-A,B-1304-A;n:type:ShaderForge.SFN_Add,id:2127,x:33311,y:32742,varname:node_2127,prsc:2|A-1472-OUT,B-2186-OUT;proporder:1304-4862-5354-1445;pass:END;sub:END;*/

Shader "Custom/Crystal" {
    Properties {
        _Color ("Color", Color) = (1,0.8068964,0,1)
        _MainTex ("MainTex", 2D) = "white" {}
        _Reflection ("Reflection", Cube) = "_Skybox" {}
        _ReflectionForce ("Reflection Force", Float ) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 
            #pragma target 2.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform samplerCUBE _Reflection;
            uniform float _ReflectionForce;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 emissive = ((_MainTex_var.rgb*_Color.rgb)+(texCUBE(_Reflection,reflect((-1*viewDirection),i.normalDir)).rgb*_ReflectionForce));
                float3 finalColor = emissive;
                return fixed4(finalColor,(_MainTex_var.a*_Color.a));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
