using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ST.GPUSkin
{
    /// <summary>
    /// 
    /// </summary>
    public partial class GPUSkinBonePlayer
    {

#if !ST_GAME_MODE

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mf"></param>
        /// <param name="renderer"></param>
        protected override void OnEditorValidate(MeshFilter mf, MeshRenderer renderer)
        {
            if (m_InfoDB == null)
                return;

            InitMaterial(renderer);
            InitAnimationNameList();
            PlayAnimValidate();
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

        /// <summary>
        /// 
        /// </summary>
        void PlayAnimValidate()
        {
            var infoList = m_InfoDB.infoList;
            if (infoList == null)
            {
                Play(-1);
                return;
            }

            int infoNum = infoList.Length;
            if (m_AnimationIndex >= infoNum)
            {
                Play(infoNum - 1);
            }
            else if (m_AnimationIndex < 0)
            {
                Play(0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mat"></param>
        void InitMaterial(MeshRenderer renderer)
        {
            Material mat = renderer.sharedMaterial;
            if (mat != null)
                return;

            mat.SetTexture(GPUSkinDefine.GPUSKIN_SHADER_BONE_ANIM_TEX_ID, m_InfoDB.texture);
            mat.SetFloat(GPUSkinDefine.GPUSKIN_SHADER_BONE_ANIM_TEX_WIDTH_ID, m_InfoDB.texWidth);
            mat.SetFloat(GPUSkinDefine.GPUSKIN_SHADER_BONE_ANIM_TEX_HEIGHT_ID, m_InfoDB.texHeight);
            mat.SetFloat(GPUSkinDefine.GPUSKIN_SHADER_BONE_COUNT_ID, m_InfoDB.boneCount);
        }
#endif
    }
}

