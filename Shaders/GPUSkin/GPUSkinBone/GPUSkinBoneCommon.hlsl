

#ifndef _GPU_SKIN_BONE_COMMON_
#define _GPU_SKIN_BONE_COMMON_

float4x4 GetMeshToLocalMatrix(uint boneIndex, uint curFrame, uint curFramePixelIndex,
    float4 boneAnimTex_TexelSize, float srcBoneCount, sampler2D boneAnimTex)
{
    uint boneAnimTexWidth = (uint)boneAnimTex_TexelSize.z;
    uint boneCount = (uint)srcBoneCount;

    uint pixelIndex = curFramePixelIndex + (boneCount * curFrame + boneIndex) * 4;
    float4 rowUV = float4(	(pixelIndex % boneAnimTexWidth + 0.5) * boneAnimTex_TexelSize.x,
                            (pixelIndex / boneAnimTexWidth + 0.5) * boneAnimTex_TexelSize.y, 0, 0);
    float4 row0 =  tex2Dlod(boneAnimTex, rowUV);

    pixelIndex = pixelIndex + 1;
    rowUV = float4(	(pixelIndex % boneAnimTexWidth + 0.5) * boneAnimTex_TexelSize.x,
                    (pixelIndex / boneAnimTexWidth + 0.5) * boneAnimTex_TexelSize.y, 0, 0);
    float4 row1 =  tex2Dlod(boneAnimTex, rowUV);

    pixelIndex = pixelIndex + 1;
    rowUV = float4(	(pixelIndex % boneAnimTexWidth + 0.5) * boneAnimTex_TexelSize.x,
                    (pixelIndex / boneAnimTexWidth + 0.5) * boneAnimTex_TexelSize.y, 0, 0);
    float4 row2 =  tex2Dlod(boneAnimTex, rowUV);

    pixelIndex = pixelIndex + 1;
    rowUV = float4(	(pixelIndex % boneAnimTexWidth + 0.5) * boneAnimTex_TexelSize.x,
                    (pixelIndex / boneAnimTexWidth + 0.5) * boneAnimTex_TexelSize.y, 0, 0);
    float4 row3 =  tex2Dlod(boneAnimTex, rowUV);

    return float4x4(row0, row1, row2, row3);
}

#endif