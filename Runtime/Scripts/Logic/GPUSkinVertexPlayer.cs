using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ST.GPUSkin
{
    /// <summary>
    /// 
    /// </summary>
    [ExecuteInEditMode]
    public partial class GPUSkinVertexPlayer : GPUSkinPlayerBase
    {
        /// <summary>
        /// 
        /// </summary>
        public GPUSkinVertexInfoDB infoDB;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override GPUSkinInfoDB[] GetInfoList()
        {
            if (infoDB == null)
                return null;

            return infoDB.infoList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="currentInfo"></param>
        /// <param name="block"></param>
        protected override void OnSetPropertyBlock(MeshRenderer renderer, GPUSkinInfoDB currentInfo, MaterialPropertyBlock block)
        {
            if (renderer == null || block == null)
                return;

            renderer.GetPropertyBlock(block);
            block.SetFloat(GPUSkinDefine.s_GpuSkin_Shader_Common_CurFrame, m_LastFrameIndex);
            block.SetFloat(GPUSkinDefine.s_GpuSkin_Shader_Common_CurFramePixelIndex, currentInfo.startPixelIndex);
            block.SetFloat(GPUSkinDefine.s_GpuSkin_Shader_Common_CurFrameCount, currentInfo.frameCount);
            renderer.SetPropertyBlock(block);
        }
    }
}

