

#ifndef _GPU_SKIN_VERTEX_FORWARD_PASS_
#define _GPU_SKIN_VERTEX_FORWARD_PASS_

struct Attributes
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
    float2 uv2 : TEXCOORD1;
    float2 uv3 : TEXCOORD2;
    uint index : SV_VertexID;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float2 uv : TEXCOORD0;
    float4 vertex : SV_POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

Varyings GPUSkinVertexVert(Attributes v)
{
    Varyings o;

    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_TRANSFER_INSTANCE_ID(v, o);

    o.vertex = TransformObjectToHClip(v.vertex.xyz);
    o.uv = TRANSFORM_TEX(v.uv, _MainTex);

    uint curFrame = (uint)UNITY_ACCESS_INSTANCED_PROP(Props, _CurFrame);
    uint curFramePixelIndex = (uint)UNITY_ACCESS_INSTANCED_PROP(Props, _CurFramePixelIndex);
    uint curFrameCount = (uint)UNITY_ACCESS_INSTANCED_PROP(Props, _CurFrameCount);

    uint realCurFrame = (curFrame + g_GpuSkinFrameIndex) % curFrameCount;

    float3 vertexPos = GetVertexPos(v.index, realCurFrame, curFramePixelIndex, 
        _VertexAnimTex_TexelSize, _VertexCount, _VertexAnimTex);
    o.vertex = TransformObjectToHClip(vertexPos);

    return o;
}

float4 GPUSkinVertexFrag(Varyings i) : SV_Target
{
    float4 col = tex2D(_MainTex, i.uv);

#if _ALPHATEST_ON
    clip(col.a - _AlphaClip);
#endif

    return col;
}

#endif