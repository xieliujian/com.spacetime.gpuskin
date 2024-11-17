using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ST.GPUSkin
{
    /// <summary>
    /// 
    /// </summary>
    public abstract partial class GPUSkinPlayerBase
    {

#if !ST_GAME_MODE

        /// <summary>
        /// 
        /// </summary>
        void OnValidate()
        {
            MeshFilter mf = gameObject.GetComponentInChildren<MeshFilter>();
            meshRenderer = gameObject.GetComponentInChildren<MeshRenderer>();

            if (mf != null && meshRenderer != null)
            {
                OnEditorValidate(mf, meshRenderer);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void Update()
        {
            if (m_AnimationIndex < 0 || meshRenderer == null)
                return;

            float frameTime = 1.0f / m_CurrentPlayInfo.frameRate;

            if (Time.time - m_LastFrameTime >= frameTime)
            {
                m_LastFrameTime = Time.time;
                m_LastFrameIndex = (m_LastFrameIndex + 1) % m_CurrentPlayInfo.frameCount;

                OnUpdateSkin(meshRenderer, m_CurrentPlayInfo, m_Block);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mf"></param>
        /// <param name="renderer"></param>
        protected abstract void OnEditorValidate(MeshFilter mf, MeshRenderer renderer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="currentInfo"></param>
        /// <param name="block"></param>
        protected abstract void OnUpdateSkin(MeshRenderer renderer, GPUSkinInfoDB currentInfo, MaterialPropertyBlock block);
#endif
    }
}

