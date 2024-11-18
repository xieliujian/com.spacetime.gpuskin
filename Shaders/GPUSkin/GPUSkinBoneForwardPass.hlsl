

#ifndef _GPU_SKIN_BONE_FORWARD_PASS_
#define _GPU_SKIN_BONE_FORWARD_PASS_

struct Attributes
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
    float2 uv2 : TEXCOORD1;
    float2 uv3 : TEXCOORD2;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float2 uv : TEXCOORD0;
    float4 vertex : SV_POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

Varyings vert (Attributes v)
{
    Varyings o;

    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_TRANSFER_INSTANCE_ID(v, o);

    o.vertex = TransformObjectToHClip(v.vertex.xyz);
    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    
    uint bone0Index = (uint)v.uv2.x;
    uint bone1Index = (uint)v.uv2.y;
    float bone0Weight = v.uv3.x;
    float bone1Weight = v.uv3.y;
    uint curFrame = (uint)UNITY_ACCESS_INSTANCED_PROP(Props, _CurFrame);
    uint curFramePixelIndex = (uint)UNITY_ACCESS_INSTANCED_PROP(Props, _CurFramePixelIndex);
    uint curFrameCount = (uint)UNITY_ACCESS_INSTANCED_PROP(Props, _CurFrameCount);

    uint realCurFrame = (curFrame + g_GpuSkinFrameIndex) % curFrameCount;

    float4x4 meshToLocal0 = GetMeshToLocalMatrix(bone0Index, realCurFrame, curFramePixelIndex,
        _BoneAnimTex_TexelSize, _BoneCount, _BoneAnimTex);
    float4x4 meshToLocal1 = GetMeshToLocalMatrix(bone1Index, realCurFrame, curFramePixelIndex,
        _BoneAnimTex_TexelSize, _BoneCount, _BoneAnimTex);
    float4 localPos0 = mul(meshToLocal0, v.vertex);
    float4 localPos1 = mul(meshToLocal1, v.vertex);

    float4 finalPos = localPos0 * bone0Weight + localPos1 * bone1Weight;
    float4 curVertex = TransformObjectToHClip(finalPos.xyz);

    o.vertex = curVertex;

    return o;
}

float4 frag (Varyings i) : SV_Target
{
    float4 col = tex2D(_MainTex, i.uv);

#if _ALPHATEST_ON
    clip(col.a - _AlphaClip);
#endif

    return col;
}

#endif