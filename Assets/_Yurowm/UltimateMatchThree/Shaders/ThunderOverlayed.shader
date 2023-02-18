Shader "Particles/Thunder Blended Overlayed" {
Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
	_Amplitude ("Amplitude", Float) = 0.1
	_FracAmplitude ("FracAmplitude", Float) = 0.1
	_White ("White", Float) = 0.1
	_Speed ("Speed", Float) = 0.1
	_TilingSpeed ("TilingSpeed", Float) = 1
	_ColorSpeed ("ColorSpeed", Float) = 1
	_Octave ("Octave", Float) = 0.4
	_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
	Blend SrcAlpha OneMinusSrcAlpha
	ColorMask RGB
	Cull Off Lighting Off ZWrite Off

	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_particles
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _TintColor;
			fixed _Amplitude;
			fixed _FracAmplitude;
			fixed _Speed;
			fixed _Octave;
			fixed _TilingSpeed;
			fixed _ColorSpeed;
			fixed _White;
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				#ifdef SOFTPARTICLES_ON
				float4 projPos : TEXCOORD2;
				#endif
			};
			
			float4 _MainTex_ST;

			float rand(float2 co){
			    return sin(dot(co, float2(12.9898, 5.233)));
			}

			float frac_rand(float2 co){
				return frac(sin(dot(co, float2(125.9898, 568.233))) * 43758.5453);
			}

			float3 hsv_to_rgb(float3 HSV)
	        {
	                float3 RGB = HSV.z;
	       
	                   float var_h = HSV.x * 6;
	                   float var_i = floor(var_h);   // Or ... var_i = floor( var_h )
	                   float var_1 = HSV.z * (1.0 - HSV.y);
	                   float var_2 = HSV.z * (1.0 - HSV.y * (var_h-var_i));
	                   float var_3 = HSV.z * (1.0 - HSV.y * (1-(var_h-var_i)));
	                   if      (var_i == 0) { RGB = float3(HSV.z, var_3, var_1); }
	                   else if (var_i == 1) { RGB = float3(var_2, HSV.z, var_1); }
	                   else if (var_i == 2) { RGB = float3(var_1, HSV.z, var_3); }
	                   else if (var_i == 3) { RGB = float3(var_1, var_2, HSV.z); }
	                   else if (var_i == 4) { RGB = float3(var_3, var_1, HSV.z); }
	                   else                 { RGB = float3(HSV.z, var_1, var_2); }
	       
	           return (RGB);
	        }
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				#ifdef SOFTPARTICLES_ON
				o.projPos = ComputeScreenPos (o.vertex);
				COMPUTE_EYEDEPTH(o.projPos.z);
				#endif
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				o.texcoord.x += _Time.z * _TilingSpeed;
				float timeX = (_Time.x + o.vertex.y * _Octave) * _Speed;
				float timeY = (_Time.x + o.vertex.x * _Octave) * _Speed;

				o.vertex.x += frac_rand(float2(timeX, timeX)) * _FracAmplitude - _FracAmplitude/2;
				o.vertex.y += frac_rand(float2(timeY, timeY)) * _FracAmplitude - _FracAmplitude/2;
				
				o.vertex.x += rand(float2(timeX, timeY)) * _Amplitude;
				o.vertex.y += rand(float2(timeY, timeX)) * _Amplitude;
				
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			sampler2D_float _CameraDepthTexture;
			float _InvFade;
			
			fixed4 frag (v2f i) : SV_Target
			{
				#ifdef SOFTPARTICLES_ON
				float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
				float partZ = i.projPos.z;
				float fade = saturate (_InvFade * (sceneZ-partZ));
				i.color.a *= fade;
				#endif

				fixed4 tex = tex2D(_MainTex, i.texcoord);
				float3 c = hsv_to_rgb(float3(frac(_Time.x * _ColorSpeed), 1, 1));
				c += _White;
				fixed4 color = i.color * _TintColor;
				color.xyz *= c;
				if (tex.r + tex.g + tex.b > 1.5)
					tex.rgb = 1.0 - 2 * (1.0 - color.rgb) * (1.0 - tex.rgb);
				else 
					tex.rgb = tex.rgb * 2 * color.rgb;

				tex.a *= color.a;
				
				UNITY_APPLY_FOG(i.fogCoord, tex);
				return tex;
			}
			ENDCG 
		}
	}	
}
}