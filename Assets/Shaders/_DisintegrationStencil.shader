
Shader "Custom/_DisintegrationStencil"
{
	Properties 
	{
		_MainTex("Noise", 2D) = "white" {}
		_Duration("Duration", Float) = 10
		_T("T", Float) = 10
		_Active("Active", int) = 0
	}
	Subshader
	{
		Tags{"Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Transparent" }

		Pass
		{
				Name "Disintegration"
				Tags{ "LightMode" = "ForwardBase" "Customtag" = "test" }
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
				};

				struct vertexOutput
				{
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				uniform half4 _OutlineColor;
				uniform half4 _DisColor;
				uniform half4 _Color;
				uniform float _Duration;
				uniform float _T;
				uniform float _Cutoff;
				uniform int _Active;

				vertexOutput vert(vertexInput v)
				{
					vertexOutput o; 
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;

					return o;
				}

				half4 frag(vertexOutput i): COLOR
				{
					if(_Active == 0)
						discard;
						
					half4 noiseColor = tex2D(_MainTex, i.uv);

					float t = _T / _Duration;
					clamp(0,1,t);

					float val = (noiseColor.r + noiseColor.g + noiseColor.b) / 3;

					if(val >= t)
						discard;
					return half4(0,0,0,0);
				}
				ENDCG
		}

	}
}
