// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/_Disintegration"
{
	Properties 
	{
		_MainTex("Noise", 2D) = "white" {}
		_OutlineColor("Outline Color", color) = (1,0,0,1)
		_Duration("Duration", Float) = 10
		_DebugTime("Time", Range(0, 10)) = 0
	}
	Subshader
	{
		Tags{"Queue" = "Geometry-2" "IgnoreProjector" = "true" "RenderType" = "Opaque"}
		//ZWrite Off

		Pass
		{
			Stencil
			{
				Ref 2
				Comp Always
				Pass Replace
			}
				Tags{"LightMode" = "ForwardBase"}
				Blend SrcAlpha OneMinusSrcAlpha
				BlendOp Add

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 3.0

				#include "UnityCG.cginc"
				#include "CGLighting.cginc"
				
				struct vertexInput
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct vertexOutput
				{
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				uniform half4 _OutlineColor;
				uniform float _Duration;
				uniform float _DebugTime;
				
				vertexOutput vert(vertexInput v)
				{
					vertexOutput o; 
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;

					return o;
				}

				half4 frag(vertexOutput i): COLOR
				{
					half4 noiseColor = tex2D(_MainTex, i.uv);

					float t = _DebugTime / _Duration;
					float val = (noiseColor.r + noiseColor.g + noiseColor.b) / 3;

					if(val >= t)
						discard;

					float c = val / t;
					if(c < t)
						c = 0;
					return lerp(half4(0,0,0,0), _OutlineColor, c);
				}
				ENDCG
		}

	}
}
