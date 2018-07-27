// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/_StencilWriter"
{
	Properties 
	{
	}
	Subshader
	{
		Tags{"Queue" = "Geometry+1" "IgnoreProjector" = "true" "RenderType" = "Opaque"}
		Pass
		{
			Tags{"LightMode" = "ForwardBase"}
			Blend SrcAlpha OneMinusSrcAlpha

			Stencil
			{
				Ref 1
				Comp always
				Pass replace
			}
			ColorMask 0

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"
			#include "CGLighting.cginc"
			
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
				vertexOutput o; 
				o.pos = UnityObjectToClipPos(v.vertex);

				return o;
			}

			half4 frag(vertexOutput i): COLOR
			{
				return half4(0, 0, 0, 0);
			}
			ENDCG
		}
	}
}
