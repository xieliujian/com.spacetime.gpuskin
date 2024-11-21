

#ifndef _GPU_SKIN_VERTEX_COMMON_
#define _GPU_SKIN_VERTEX_COMMON_

float3 GetVertexPos(uint vertexIndex, uint curFrame, uint curFramePixelIndex,
    float4 vertexAnimTex_TexelSize, float srcVertexCount, sampler2D vertexAnimTex)
{
    uint vertexAnimTexWidth = (uint)vertexAnimTex_TexelSize.z;
    uint vertexCount = (uint)srcVertexCount;

    uint pixelIndex = curFramePixelIndex + vertexCount * curFrame + vertexIndex;
    float4 uv = float4(	(pixelIndex % vertexAnimTexWidth + 0.5) * vertexAnimTex_TexelSize.x,
                        (pixelIndex / vertexAnimTexWidth + 0.5) * vertexAnimTex_TexelSize.y, 0, 0);
    float3 vertexPos =  tex2Dlod(vertexAnimTex, uv).xyz;

    return vertexPos;
}

#endif