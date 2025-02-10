// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "as_waterShader"
{
	Properties
	{
		[NoScaleOffset]_NormalMap0("NormalMap0", 2D) = "bump" {}
		[Header(Refraction)]
		_ChromaticAberration("Chromatic Aberration", Range( 0 , 0.3)) = 0.1
		[NoScaleOffset]_NormalMap1("NormalMap1", 2D) = "bump" {}
		_PanSpeed0XY("PanSpeed0 X,Y", Vector) = (0.25,0.23,0,0)
		_BaseColorAlpha("BaseColor-Alpha", Color) = (0,0,0,0)
		_PanSpeed1XY("PanSpeed1 X,Y", Vector) = (0.47,-0.31,0,0)
		_Refraction("Refraction", Range( 0 , 1)) = 0.25
		_Gloss("Gloss", Range( 0 , 1)) = 0.98
		_Tiling("Tiling", Vector) = (2,2,0,0)
		_NormalPower("NormalPower", Range( 0 , 1)) = 0
		_Tesselation("Tesselation", Range( 1 , 150)) = 3
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
		[Header(Forward Rendering Options)]
		[ToggleOff] _GlossyReflections("Reflections", Float) = 1.0
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Off
		GrabPass{ }
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "Tessellation.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 4.6
		#pragma shader_feature _GLOSSYREFLECTIONS_OFF
		#pragma multi_compile _ALPHAPREMULTIPLY_ON
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPos;
			float3 worldPos;
			float3 viewDir;
			INTERNAL_DATA
		};

		uniform sampler2D _NormalMap0;
		uniform float2 _PanSpeed0XY;
		uniform float2 _Tiling;
		uniform sampler2D _NormalMap1;
		uniform float2 _PanSpeed1XY;
		uniform float _NormalPower;
		uniform float4 _BaseColorAlpha;
		uniform float _Gloss;
		uniform sampler2D _GrabTexture;
		uniform float _ChromaticAberration;
		uniform float _Refraction;
		uniform float _Tesselation;

		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			return UnityDistanceBasedTess( v0.vertex, v1.vertex, v2.vertex, 0.0,8.0,_Tesselation);
		}

		void vertexDataFunc( inout appdata_full v )
		{
			float2 uv_TexCoord5 = v.texcoord.xy * _Tiling;
			float2 panner4 = ( 1.0 * _Time.y * _PanSpeed0XY + uv_TexCoord5);
			float2 panner6 = ( 1.0 * _Time.y * _PanSpeed1XY + uv_TexCoord5);
			float3 lerpResult25 = lerp( float3(0,0,1) , BlendNormals( UnpackNormal( tex2Dlod( _NormalMap0, float4( panner4, 0, 0.0) ) ) , UnpackNormal( tex2Dlod( _NormalMap1, float4( panner6, 0, 0.0) ) ) ) , _NormalPower);
			float3 break28 = lerpResult25;
			float4 appendResult27 = (float4(break28.x , break28.y , break28.z , 0.0));
			v.vertex.xyz += ( appendResult27 * 0.05 ).xyz;
		}

		inline float4 Refraction( Input i, SurfaceOutputStandard o, float indexOfRefraction, float chomaticAberration ) {
			float3 worldNormal = o.Normal;
			float4 screenPos = i.screenPos;
			#if UNITY_UV_STARTS_AT_TOP
				float scale = -1.0;
			#else
				float scale = 1.0;
			#endif
			float halfPosW = screenPos.w * 0.5;
			screenPos.y = ( screenPos.y - halfPosW ) * _ProjectionParams.x * scale + halfPosW;
			#if SHADER_API_D3D9 || SHADER_API_D3D11
				screenPos.w += 0.00000000001;
			#endif
			float2 projScreenPos = ( screenPos / screenPos.w ).xy;
			float3 worldViewDir = normalize( UnityWorldSpaceViewDir( i.worldPos ) );
			float3 refractionOffset = ( ( ( ( indexOfRefraction - 1.0 ) * mul( UNITY_MATRIX_V, float4( worldNormal, 0.0 ) ) ) * ( 1.0 / ( screenPos.z + 1.0 ) ) ) * ( 1.0 - dot( worldNormal, worldViewDir ) ) );
			float2 cameraRefraction = float2( refractionOffset.x, -( refractionOffset.y * _ProjectionParams.x ) );
			float4 redAlpha = tex2D( _GrabTexture, ( projScreenPos + cameraRefraction ) );
			float green = tex2D( _GrabTexture, ( projScreenPos + ( cameraRefraction * ( 1.0 - chomaticAberration ) ) ) ).g;
			float blue = tex2D( _GrabTexture, ( projScreenPos + ( cameraRefraction * ( 1.0 + chomaticAberration ) ) ) ).b;
			return float4( redAlpha.r, green, blue, redAlpha.a );
		}

		void RefractionF( Input i, SurfaceOutputStandard o, inout half4 color )
		{
			#ifdef UNITY_PASS_FORWARDBASE
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			color.rgb = color.rgb + Refraction( i, o, refract( i.viewDir , -ase_worldViewDir , ( 1.0 - _Refraction ) ).x, _ChromaticAberration ) * ( 1 - color.a );
			color.a = 1;
			#endif
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Normal = float3(0,0,1);
			float2 uv_TexCoord5 = i.uv_texcoord * _Tiling;
			float2 panner4 = ( 1.0 * _Time.y * _PanSpeed0XY + uv_TexCoord5);
			float2 panner6 = ( 1.0 * _Time.y * _PanSpeed1XY + uv_TexCoord5);
			float3 lerpResult25 = lerp( float3(0,0,1) , BlendNormals( UnpackNormal( tex2D( _NormalMap0, panner4 ) ) , UnpackNormal( tex2D( _NormalMap1, panner6 ) ) ) , _NormalPower);
			o.Normal = lerpResult25;
			o.Albedo = _BaseColorAlpha.rgb;
			o.Smoothness = _Gloss;
			float temp_output_13_4 = _BaseColorAlpha.a;
			o.Alpha = temp_output_13_4;
			o.Normal = o.Normal + 0.00001 * i.screenPos * i.worldPos;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha finalcolor:RefractionF fullforwardshadows exclude_path:deferred vertex:vertexDataFunc tessellate:tessFunction 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.6
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 screenPos : TEXCOORD2;
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.viewDir = IN.tSpace0.xyz * worldViewDir.x + IN.tSpace1.xyz * worldViewDir.y + IN.tSpace2.xyz * worldViewDir.z;
				surfIN.worldPos = worldPos;
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				surfIN.screenPos = IN.screenPos;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17800
1920;19;1920;1009;198.1046;479.4248;1.407633;True;True
Node;AmplifyShaderEditor.Vector2Node;18;-1426.951,-236.5354;Float;False;Property;_Tiling;Tiling;9;0;Create;True;0;0;False;0;2,2;8,6;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;5;-1061.365,-201.5165;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;10;-1063.566,82.0836;Float;False;Property;_PanSpeed1XY;PanSpeed1 X,Y;6;0;Create;True;0;0;False;0;0.47,-0.31;-0.05,-0.3;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;9;-1080.465,-88.21642;Float;False;Property;_PanSpeed0XY;PanSpeed0 X,Y;4;0;Create;True;0;0;False;0;0.25,0.23;0.1,0.2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;4;-696.9644,-197.4164;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;6;-686.5646,-32.31655;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;1;-400.5,-228.1736;Inherit;True;Property;_NormalMap0;NormalMap0;0;1;[NoScaleOffset];Create;True;0;0;False;0;-1;None;03695c63955a55240bedc20a1f3929e1;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-399.2646,28.78356;Inherit;True;Property;_NormalMap1;NormalMap1;3;1;[NoScaleOffset];Create;True;0;0;False;0;-1;None;03695c63955a55240bedc20a1f3929e1;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;24;24.01068,54.64478;Float;False;Property;_NormalPower;NormalPower;10;0;Create;True;0;0;False;0;0;0.092;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.BlendNormalsNode;3;-4.064592,-66.11639;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector3Node;26;238.9613,-139.54;Float;False;Constant;_Vector0;Vector 0;10;0;Create;True;0;0;False;0;0,0,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LerpOp;25;347.3588,37.35145;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;14;-384.4752,778.5636;Float;False;Property;_Refraction;Refraction;7;0;Create;True;0;0;False;0;0.25;0.293;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;21;-379.6549,271.1752;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.BreakToComponentsNode;28;532.3983,249.7957;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.NegateNode;22;-64.71048,290.7311;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;30;911.2867,514.7777;Float;False;Constant;_Float0;Float 0;11;0;Create;True;0;0;False;0;0.05;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;27;872.6436,280.3934;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;48;-500.2576,576.8564;Float;False;Tangent;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.OneMinusNode;40;30.12984,686.3187;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;37;857.0486,673.5367;Float;False;Property;_Tesselation;Tesselation;11;0;Create;True;0;0;False;0;3;46;1;150;0;1;FLOAT;0
Node;AmplifyShaderEditor.DitheringNode;49;1294.919,294.7276;Inherit;False;0;False;3;0;FLOAT;0;False;1;SAMPLER2D;;False;2;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;13;98.63489,-310.5164;Float;False;Property;_BaseColorAlpha;BaseColor-Alpha;5;0;Create;True;0;0;False;0;0,0,0,0;0.4407179,0.5073529,0.3767842,0.684;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RefractOpVec;20;267.5289,279.6352;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-70.3243,201.9157;Float;False;Property;_Gloss;Gloss;8;0;Create;True;0;0;False;0;0.98;0.99;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceBasedTessNode;38;1251.748,691.3368;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;8;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;1100.73,372.0956;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.WorldNormalVector;23;-499.9732,431.451;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1563.198,82.77764;Float;False;True;-1;6;ASEMaterialInspector;0;0;Standard;as_waterShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;True;False;Off;1;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0;True;True;0;True;Transparent;;Transparent;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;2;47.17;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;5;-1;1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.3;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;5;0;18;0
WireConnection;4;0;5;0
WireConnection;4;2;9;0
WireConnection;6;0;5;0
WireConnection;6;2;10;0
WireConnection;1;1;4;0
WireConnection;2;1;6;0
WireConnection;3;0;1;0
WireConnection;3;1;2;0
WireConnection;25;0;26;0
WireConnection;25;1;3;0
WireConnection;25;2;24;0
WireConnection;28;0;25;0
WireConnection;22;0;21;0
WireConnection;27;0;28;0
WireConnection;27;1;28;1
WireConnection;27;2;28;2
WireConnection;40;0;14;0
WireConnection;49;0;13;4
WireConnection;20;0;48;0
WireConnection;20;1;22;0
WireConnection;20;2;40;0
WireConnection;38;0;37;0
WireConnection;29;0;27;0
WireConnection;29;1;30;0
WireConnection;0;0;13;0
WireConnection;0;1;25;0
WireConnection;0;4;11;0
WireConnection;0;8;20;0
WireConnection;0;9;13;4
WireConnection;0;10;49;0
WireConnection;0;11;29;0
WireConnection;0;14;38;0
ASEEND*/
//CHKSM=A290D87412A01514BEE431D90A384AD0EB0DFC3A