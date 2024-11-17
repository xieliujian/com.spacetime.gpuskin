Shader "SpaceTime/Character/GPUSkinVertex"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _VertexAnimTex ("Vertex Animation Tex", 2D) = "white" {}
        _VertexCount ("Vertex Count", Float) = 0
        _CurFrame ("Current Frame", Float) = 0
        _CurFramePixelIndex ("Current Frame Pixel Index", Float) = 0
		_CurFrameCount("Frame Count", Float) = 30
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        Pass
        {
            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM

            #pragma vertex GPUSkinVertexVert
            #pragma fragment GPUSkinVertexFrag
            #pragma target 4.5
            #pragma multi_compile_instancing

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
            #include "GPUSkinVertexCommon.hlsl"
            #include "GPUSkinVertexInclude.hlsl"
			#include "GPUSkinVertexForwardPass.hlsl"

            ENDHLSL
        }
    }
}
