// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/_ThroughWall"
{
	Properties
	{
		_NormalMap ("Normal Map", 2D) = "white" {}
		_RimFactor("Rim Factor", Range(0, 1)) = 1
		_RimPower("Rim Power", Range(0, 1)) = 0.3
		_RimCutoff("Rim Cutoff", Range(0, 1)) = 0.7
		_RimColor ("Rim Color", Color) = (1,0,0,1)
	}
	Subshader
	{
		Tags{"Queue" = "Geometry+2" "IgnoreProjector" = "true" "RenderType" = "Opaque"}
		Pass
		{
		Tags{"LightMode" = "ForwardBase"}
			Blend SrcAlpha OneMinusSrcAlpha 
			Stencil
			{
				Ref 1
				Comp equal
				Pass keep
			}
			ZTest Always
			ColorMask 0

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			struct vertexInput
			{
				float4 vertex : POSITION;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
			};

			vertexOutput vert(vertexInput v)
			{
				vertexOutput o; UNITY_INITIALIZE_OUTPUT(vertexOutput, o); // d3d11 requires initialization
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}

			half4 frag(vertexOutput i): COLOR
			{
				return half4(0, 0, 0, 0); 
			}
			ENDCG
		}
		Pass
		{
			Tags{"LightMode" = "ForwardBase"}
			Blend SrcAlpha OneMinusSrcAlpha
			Stencil
			{
				Ref 1
				Comp equal
				Pass keep
			}
			ZTest GEqual
			ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"
			#include "CGLighting.cginc"
			
			uniform sampler2D _NormalMap;
			uniform float4 _NormalMap_ST;

			uniform half4 _RimColor;
			uniform float _RimFactor;
			uniform float _RimCutoff;
			uniform float _RimPower;
			
			struct vertexInput
			{
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				float4 texcoord : TEXCOORD0;

				float4 tangent : TANGENT;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;

				float4 normalWorld : TEXCOORD1;
				float4 tangentWorld : TEXCOORD2;
				float3 binormalWorld : TEXCOORD3;
				float4 normalTexCoord: TEXCOORD4;
				float4 posWorld	: TEXCOORD5;
			};

			vertexOutput vert(vertexInput v)
			{
				vertexOutput o; UNITY_INITIALIZE_OUTPUT(vertexOutput, o); // d3d11 requires initialization
				o.pos = UnityObjectToClipPos(v.vertex);

				o.normalWorld	= float4(UnityObjectToWorldNormal(v.normal.xyz), v.normal.w);
				o.tangentWorld	= float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);
				o.binormalWorld	= float3(normalize(cross(o.normalWorld.xyz,  o.tangentWorld.xyz) * v.tangent.w));
				o.binormalWorld *=	unity_WorldTransformParams.w;
				o.normalTexCoord.xy = (v.texcoord.xy * _NormalMap_ST.xy + _NormalMap_ST.zw);
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);

				return o;
			}

			half4 frag(vertexOutput i): COLOR
			{
				float3 normalWorldAtPixel = WorldNormalFromNormalMap(_NormalMap,i.normalTexCoord.xy,i.tangentWorld.xyz,i.binormalWorld.xyz,i.normalWorld.xyz);
				float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				float attenuation = 1;
				float3 V = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
				float4 rim = RimLight(normalWorldAtPixel,
														V,
														_RimColor.rgb,
														_RimCutoff,
														_RimPower);
				float4 finalColor = rim;
				return finalColor;
			}

			ENDCG
		}
	}
}