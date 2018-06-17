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
			#pragma shader_feature _USENORMAL_OFF _USENORMAL_ON 
			#pragma shader_feature _LIGHTING_OFF _LIGHTING_VERT _LIGHTING_FRAG
			#pragma shader_feature _AMBIENTMODE_OFF _AMBIENTMODE_ON
			#pragma shader_feature _RIM_OFF _RIM_ON

			#include "UnityCG.cginc"
			#include "CGLighting.cginc"
			
			uniform half4 _MainColor;
			uniform sampler2D _MainTexture;
			uniform float4 _MainTexture_ST;

			uniform sampler2D _NormalMap;
			uniform float4 _NormalMap_ST;

			uniform float _Diffuse;
			uniform float4 _LightColor0; 

			//
			uniform sampler2D _SpecularMap;
			uniform float _SpecularFactor;
			uniform float _SpecularPower;

			#if _AMBIENTMODE_ON
			uniform float _AmbientFactor;
			#endif

			#if _RIM_ON
			uniform half4 _RimColor;
			uniform float _RimFactor;
			uniform float _RimCutoff;
			uniform float _RimPower;
			#endif
			
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
