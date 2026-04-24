using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ST.GPUSkin
{
    /// <summary>
    /// 
    /// </summary>
    public class GPUSkinDefine
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly string s_GpuSkin_GameModeDefine = "ST_GAME_MODE";

        /// <summary>
        /// 
        /// </summary>
        public static readonly int s_GpuSkin_Shader_Common_CurFrame = Shader.PropertyToID("_CurFrame");
        public static readonly int s_GpuSkin_Shader_Common_CurFramePixelIndex = Shader.PropertyToID("_CurFramePixelIndex");
        public static readonly int s_GpuSkin_Shader_Common_CurFrameCount = Shader.PropertyToID("_CurFrameCount");
        public static readonly int s_GpuSkin_Shader_Common_GlobalFrameIndex = Shader.PropertyToID("g_GpuSkinFrameIndex");

        /// <summary>
        /// 
        /// </summary>
        public static readonly int s_GpuSkin_Shader_Vertex_AnimTex = Shader.PropertyToID("_VertexAnimTex");
        public static readonly int s_GpuSkin_Shader_Vertex_AnimTexWidth = Shader.PropertyToID("_VertexAnimTexWidth");
        public static readonly int s_GpuSkin_Shader_Vertex_AnimTexHeight = Shader.PropertyToID("_VertexAnimTexHeight");
        public static readonly int s_GpuSkin_Shader_Vertex_Count = Shader.PropertyToID("_VertexCount");

        /// <summary>
        /// 
        /// </summary>
        public static readonly int s_GpuSkin_Shader_Bone_AnimTex = Shader.PropertyToID("_BoneAnimTex");
        public static readonly int s_GpuSkin_Shader_Bone_AnimTexWidth = Shader.PropertyToID("_BoneAnimTexWidth");
        public static readonly int s_GpuSkin_Shader_Bone_AnimTexHeight = Shader.PropertyToID("_BoneAnimTexHeight");
        public static readonly int s_GpuSkin_Shader_Bone_Count = Shader.PropertyToID("_BoneCount");
    }
}

