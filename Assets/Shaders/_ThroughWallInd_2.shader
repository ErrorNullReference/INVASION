// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/_ThroughWallInd_2"
{
	Properties
	{
		_Cutoff("Cutoff", Range(0, 1)) = 0.7
		_Color("Color", Color) = (1,0,0,1)  
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
				Pass Replace
			}
			ZTest GEqual
			ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"
			#include "CGLighting.cginc"
			
			uniform float _Cutoff;
			
			struct vertexInput
			{
				float4 vertex : POSITION;
				float4 normal : NORMAL;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
			};

			vertexOutput vert(vertexInput v)
			{
				vertexOutput o; 
				o.pos = UnityObjectToClipPos(v.vertex);

				float3 norm = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
				o.pos.xy += norm * _Cutoff;

				return o;
			}

			half4 frag(vertexOutput i): COLOR
			{
				return half4(0,0,0,0);
			}

			ENDCG
		}

		Pass
		{ 
			Blend SrcAlpha OneMinusSrcAlpha 
			Stencil
			{
				Ref 1
				Comp Equal
				Pass Zero
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

			struct vertexInput
			{
				float4 vertex : POSITION;
				float4 normal : NORMAL;
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
				return half4(0,0,0,0);
			}

			ENDCG
		}

		Pass
		{ 
			Blend SrcAlpha OneMinusSrcAlpha 
			Stencil
			{
				Ref 1
				Comp Equal
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
			uniform float _Cutoff;
			
			struct vertexInput
			{
				float4 vertex : POSITION;
				float4 normal : NORMAL;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
			};

			vertexOutput vert(vertexInput v)
			{
				vertexOutput o; 
				o.pos = UnityObjectToClipPos(v.vertex);

				float3 norm = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
				o.pos.xy += norm * _Cutoff;

				return o;
			}

			half4 frag(vertexOutput i): COLOR
			{
				return _Color;
			}

			ENDCG
		}
	}
}