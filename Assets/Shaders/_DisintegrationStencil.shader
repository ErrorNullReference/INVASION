
Shader "Custom/_DisintegrationStencil"
{
	Properties 
	{
		_MainTex("Noise", 2D) = "white" {}
		_Duration("Duration", Float) = 10
		_T("T", Float) = 0
		_Outline("Outline", Range(0,0.9)) = 0.9
		_OutlineColor("OutlineColor", color) = (1,1,1,1)
	}
	Subshader
	{
		Tags{"Queue" = "Geometry" "IgnoreProjector" = "true" "RenderType" = "Opaque" }

		Pass
		{
				Name "Disintegration"
				Tags{ "LightMode" = "ForwardBase" }
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
				uniform float _Duration;
				uniform float _T;
				uniform half4 _OutlineColor;
				
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

					float t = _T / _Duration;
					clamp(0,1,t);

					float val = (noiseColor.r + noiseColor.g + noiseColor.b) / 3;

					if(val < t )
						discard;
					return _OutlineColor;
				}
				ENDCG
		}

		Pass
		{
				Name "Disintegration"
				Tags{ "LightMode" = "ForwardBase" }
				Blend SrcAlpha OneMinusSrcAlpha
				BlendOp Add
				Stencil
				{
					Ref 2
					Comp Always
					Pass replace
				}

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
					float3 normal : NORMAL;
					
				};

				struct vertexOutput
				{
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				uniform float _Duration;
				uniform float _T;
				uniform float _Outline;
				
				vertexOutput vert(vertexInput v)
				{
					vertexOutput o; 
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
					o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
					return o;
				}

				half4 frag(vertexOutput i): COLOR
				{
					half4 noiseColor = tex2D(_MainTex, i.uv);

					float t = _T / _Duration;
					clamp(0,1,t);

					float val = (noiseColor.r + noiseColor.g + noiseColor.b) / 3;

					if(val < t / (1 - _Outline))
						discard;
					return half4(0,0,0,0);
				}
				ENDCG
		}

	}
}
