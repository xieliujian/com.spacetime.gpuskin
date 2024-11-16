using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.GPUSkin
{
    /// <summary>
    /// 
    /// </summary>
    [ExecuteInEditMode]
    public class GPUSkinBonePlayer : GPUSkinPlayerBase
    {
        /// <summary>
        /// 
        /// </summary>
        public GPUSkinBoneInfo info;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override GPUSkinInfo[] GetInfoList()
        {
            if (info == null)
                return null;

            return info.infoList;
        }

#if !SOUL_ENGINE

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
                renderer.sharedMaterial.SetTexture("_BoneAnimTex", info.texture);
                renderer.sharedMaterial.SetFloat("_BoneAnimTexWidth", info.texWidth);
                renderer.sharedMaterial.SetFloat("_BoneAnimTexHeight", info.texHeight);
                renderer.sharedMaterial.SetFloat("_BoneCount", info.boneCount);
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
        protected override void OnUpdateSkin(MeshRenderer renderer, GPUSkinInfo currentInfo, MaterialPropertyBlock block)
        {
            OnSetPropertyBlock(renderer, currentInfo, block);
        }

#endif

        /// <summary>
        /// 
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="currentInfo"></param>
        /// <param name="block"></param>
        protected override void OnSetPropertyBlock(MeshRenderer renderer, GPUSkinInfo currentInfo, MaterialPropertyBlock block)
        {
            if (renderer == null || block == null)
                return;

            renderer.GetPropertyBlock(block);
            block.SetFloat(STR_CUR_FRAME, m_LastFrameIndex);
            block.SetFloat(STR_CUR_FRAME_PIXEL_INDEX, currentInfo.startPixelIndex);
            block.SetFloat(STR_CUR_FRAME_COUNT, currentInfo.frameCount);
            renderer.SetPropertyBlock(block);
        }
    }
}

