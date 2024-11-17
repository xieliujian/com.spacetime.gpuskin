Shader "SpaceTime/Character/GPUSkinBone"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BoneAnimTex ("Bone Animation Tex", 2D) = "white" {}
        _BoneCount ("Bone Count", Float) = 0
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

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5
            #pragma multi_compile_instancing

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
            #include "GPUSkinBoneCommon.hlsl"
            #include "GPUSkinBoneInclude.hlsl"
            #include "GPUSkinBoneForwardPass.hlsl"    

            ENDHLSL
        }
    }
}
