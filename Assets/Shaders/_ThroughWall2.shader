// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/_ThroughWall2"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_PixelDimension ("PixelDimension", Range(0,1)) = 1
	}
	Subshader 
	{
		Tags{"Queue" = "Geometry+2" "IgnoreProjector" = "true" "RenderType" = "Opaque" "LightMode" = "ForwardBase"}
		Pass
		{ 
			Blend SrcAlpha OneMinusSrcAlpha 
			Stencil
			{
				Ref 1
				Comp NotEqual
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

			uniform half4 _Color;
			uniform float _PixelDimension;
			
			struct vertexOutput
			{
				float2 uv : TEXCOORD0;
			};

			vertexOutput vert(float4 vertex : POSITION, float2 uv : TEXCOORD0, out float4 outpos : SV_POSITION)
			{
				vertexOutput o;
				o.uv = uv;
				outpos = UnityObjectToClipPos(vertex);
				return o;
			}

			half4 frag(vertexOutput i, UNITY_VPOS_TYPE screenPos : VPOS): COLOR
			{
				float2 pos = screenPos.xy;
				screenPos.xy = floor(screenPos * _PixelDimension) * 0.5;
				float checker = -frac(screenPos.x + screenPos.y);

				clip(checker);

				return _Color;
			}

			ENDCG
		}
	}
}