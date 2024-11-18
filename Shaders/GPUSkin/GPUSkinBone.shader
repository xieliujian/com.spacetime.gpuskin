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

        [Enum(UnityEngine.Rendering.BlendMode)]_SrcBlend("SrcBlend", Float) = 1.0      // One
        [Enum(UnityEngine.Rendering.BlendMode)]_DstBlend("DstBlend", Float) = 0.0      // Zero
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", Int) = 4   // LEqual
        [Enum(On, 1, Off, 0)] _ZWrite("ZWrite", Float) = 1        // On
        [Enum(On, 1, Off, 0)] _ZClip("ZClip", Float) = 1          // On

        _AlphaClip("AlphaClip", Float) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            Tags {"LightMode" = "UniversalForward"}

            Blend [_SrcBlend][_DstBlend]
            ZClip [_ZClip]
            ZTest [_ZTest]
            ZWrite [_ZWrite]

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5
            #pragma multi_compile_instancing
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
            #include "GPUSkinBoneCommon.hlsl"
            #include "GPUSkinBoneInclude.hlsl"
            #include "GPUSkinBoneForwardPass.hlsl"    

            ENDHLSL
        }
    }
}
