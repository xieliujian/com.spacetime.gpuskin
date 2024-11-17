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
        public const string ST_GAME_MODE_DEFINE = "ST_GAME_MODE";


        /// <summary>
        /// 
        /// </summary>
        public static readonly int GPUSKIN_SHADER_COMMON_CUR_FRAME_ID = Shader.PropertyToID("_CurFrame");
        public static readonly int GPUSKIN_SHADER_COMMON_CUR_FRAME_PIXEL_INDEX_ID = Shader.PropertyToID("_CurFramePixelIndex");
        public static readonly int GPUSKIN_SHADER_COMMON_CUR_FRAME_COUNT_ID = Shader.PropertyToID("_CurFrameCount");

        /// <summary>
        /// 
        /// </summary>
        public static readonly int GPUSKIN_SHADER_VERTEX_ANIM_TEX_ID = Shader.PropertyToID("_VertexAnimTex");
        public static readonly int GPUSKIN_SHADER_VERTEX_ANIM_TEX_WIDTH_ID = Shader.PropertyToID("_VertexAnimTexWidth");
        public static readonly int GPUSKIN_SHADER_VERTEX_ANIM_TEX_HEIGHT_ID = Shader.PropertyToID("_VertexAnimTexHeight");
        public static readonly int GPUSKIN_SHADER_VERTEX_COUNT_ID = Shader.PropertyToID("_VertexCount");

        /// <summary>
        /// 
        /// </summary>
        public static readonly int GPUSKIN_SHADER_BONE_ANIM_TEX_ID = Shader.PropertyToID("_BoneAnimTex");
        public static readonly int GPUSKIN_SHADER_BONE_ANIM_TEX_WIDTH_ID = Shader.PropertyToID("_BoneAnimTexWidth");
        public static readonly int GPUSKIN_SHADER_BONE_ANIM_TEX_HEIGHT_ID = Shader.PropertyToID("_BoneAnimTexHeight");
        public static readonly int GPUSKIN_SHADER_BONE_COUNT_ID = Shader.PropertyToID("_BoneCount");
    }
}

