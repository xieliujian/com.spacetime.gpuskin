using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.GPUSkin
{
    /// <summary>
    /// 
    /// </summary>
    [ExecuteInEditMode]
    public partial class GPUSkinBonePlayer : GPUSkinPlayerBase
    {
        /// <summary>
        /// 
        /// </summary>
        public GPUSkinBoneInfoDB info;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override GPUSkinInfoDB[] GetInfoList()
        {
            if (info == null)
                return null;

            return info.infoList;
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
            block.SetFloat(GPUSkinDefine.GPUSKIN_SHADER_COMMON_CUR_FRAME_ID, m_LastFrameIndex);
            block.SetFloat(GPUSkinDefine.GPUSKIN_SHADER_COMMON_CUR_FRAME_PIXEL_INDEX_ID, currentInfo.startPixelIndex);
            block.SetFloat(GPUSkinDefine.GPUSKIN_SHADER_COMMON_CUR_FRAME_COUNT_ID, currentInfo.frameCount);
            renderer.SetPropertyBlock(block);
        }
    }
}

