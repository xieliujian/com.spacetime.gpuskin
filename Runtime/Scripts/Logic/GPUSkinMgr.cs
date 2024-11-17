using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ST.GPUSkin
{
    /// <summary>
    /// 
    /// </summary>
    public class GPUSkinMgr
    {
        /// <summary>
        /// 
        /// </summary>
        static GPUSkinMgr s_Instance;

        /// <summary>
        /// 
        /// </summary>
        public static GPUSkinMgr S
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new GPUSkinMgr();
                }

                return s_Instance;
            }
        }

        /// <summary>
        /// .
        /// </summary>
        const int FRAME_RATE = 30;

        /// <summary>
        /// .
        /// </summary>
        float m_LastFrameTime;

        /// <summary>
        /// 全局帧索引
        /// </summary>
        int m_GlobalFrameIndex;

        /// <summary>
        /// 
        /// </summary>
        public void OnUpdate()
        {
            RefreshGlobalFrameIndex();
        }

        /// <summary>
        /// 刷新shader的全局帧索引
        /// </summary>
        void RefreshGlobalFrameIndex()
        {
            float frameTime = 1.0f / FRAME_RATE;
            if (Time.time - m_LastFrameTime >= frameTime)
            {
                m_LastFrameTime = Time.time;
                Shader.SetGlobalInt(GPUSkinDefine.GPUSKIN_SHADER_COMMON_GLOBAL_FRAME_INDEX_ID, m_GlobalFrameIndex++);
            }
        }
    }
}

