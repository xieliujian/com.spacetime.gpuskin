using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ST.GPUSkin
{
    /// <summary>
    /// 
    /// </summary>
    public partial class GPUSkinVertexPlayer
    {

#if !ST_GAME_MODE

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mf"></param>
        /// <param name="renderer"></param>
        protected override void OnEditorValidate(MeshFilter mf, MeshRenderer renderer)
        {
            if (info == null)
                return;

            if (renderer.sharedMaterial != null)
            {
                renderer.sharedMaterial.SetTexture(GPUSkinDefine.GPUSKIN_SHADER_VERTEX_ANIM_TEX_ID, info.texture);
                renderer.sharedMaterial.SetFloat(GPUSkinDefine.GPUSKIN_SHADER_VERTEX_ANIM_TEX_WIDTH_ID, info.texWidth);
                renderer.sharedMaterial.SetFloat(GPUSkinDefine.GPUSKIN_SHADER_VERTEX_ANIM_TEX_HEIGHT_ID, info.texHeight);
                renderer.sharedMaterial.SetFloat(GPUSkinDefine.GPUSKIN_SHADER_VERTEX_COUNT_ID, info.vertexCount);
            }

            InitAnimationNameList();

            if (info.infoList == null)
            {
                Play(-1);
                return;
            }

            if (m_AnimationIndex >= info.infoList.Length)
            {
                Play(info.infoList.Length - 1);
            }
            else if (m_AnimationIndex < 0)
            {
                Play(0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="currentInfo"></param>
        /// <param name="block"></param>
        protected override void OnUpdateSkin(MeshRenderer renderer, GPUSkinInfoDB currentInfo, MaterialPropertyBlock block)
        {
            OnSetPropertyBlock(renderer, currentInfo, block);
        }
#endif
    }
}
