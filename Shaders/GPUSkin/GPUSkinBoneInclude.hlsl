

#ifndef _GPU_SKIN_BONE_INCLUDE
#define _GPU_SKIN_BONE_INCLUDE

sampler2D _MainTex;
float4 _MainTex_ST;
sampler2D _BoneAnimTex;
float4 _BoneAnimTex_TexelSize;
float _BoneCount;

uniform int g_GpuSkinFrameIndex;

UNITY_INSTANCING_BUFFER_START(Props)
UNITY_DEFINE_INSTANCED_PROP(float, _CurFrame)
UNITY_DEFINE_INSTANCED_PROP(float, _CurFramePixelIndex)
UNITY_DEFINE_INSTANCED_PROP(float, _CurFrameCount)
UNITY_INSTANCING_BUFFER_END(Props)

#endif