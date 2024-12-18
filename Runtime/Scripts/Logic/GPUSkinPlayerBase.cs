﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


namespace ST.GPUSkin
{
    /// <summary>
    /// 
    /// </summary>
    [ExecuteInEditMode]
    public abstract partial class GPUSkinPlayerBase : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        GPUSkinInfoDB[] m_Infos;

        /// <summary>
        /// 
        /// </summary>
        Dictionary<string, int> m_ActionNameToIndexDic = new Dictionary<string, int>(10);

        /// <summary>
        /// 
        /// </summary>
        float m_LastFrameTime;

        /// <summary>
        /// 
        /// </summary>
        GPUSkinInfoDB m_CurrentPlayInfo;

        /// <summary>
        /// 
        /// </summary>
        MaterialPropertyBlock m_Block;

        /// <summary>
        /// 
        /// </summary>
        protected float m_LastFrameIndex;

        /// <summary>
        /// 
        /// </summary>
        protected int m_AnimationIndex;

        /// <summary>
        /// 
        /// </summary>
        public MeshRenderer meshRenderer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="currentInfo"></param>
        /// <param name="block"></param>
        protected abstract void OnSetPropertyBlock(MeshRenderer renderer, GPUSkinInfoDB currentInfo, MaterialPropertyBlock block);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract GPUSkinInfoDB[] GetInfoList();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        public void GetInfos(List<GPUSkinInfoDB> list)
        {
            if (list == null)
                return;

            list.Clear();

            if (m_Infos != null)
            {
                list.AddRange(m_Infos);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionName"></param>
        public void Play(string actionName)
        {
            if (m_Infos == null || string.IsNullOrEmpty(actionName))
                return;

            if (!m_ActionNameToIndexDic.TryGetValue(actionName, out int index))
                return;

            Play(index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void Play(int index)
        {
            m_AnimationIndex = index;

            if (m_Infos == null || !GetInfo(m_AnimationIndex, out m_CurrentPlayInfo))
            {
                m_AnimationIndex = -1;
            }

            OnSetPropertyBlock(meshRenderer, m_CurrentPlayInfo, m_Block);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int RandomPlay()
        {
            int index = Random.Range(0, m_Infos.Length);
            Play(index);
            return index;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="gpuSkinInfo"></param>
        /// <returns></returns>
        public bool GetInfo(int index, out GPUSkinInfoDB gpuSkinInfo)
        {
            gpuSkinInfo = default;

            if (m_Infos == null || index < 0 || index >= m_Infos.Length)
            {
                return false;
            }

            gpuSkinInfo = m_Infos[index];
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Init()
        {
            m_LastFrameTime = Time.time;
            m_LastFrameIndex = Random.Range(0, 100);

            if (m_Block == null)
            {
                m_Block = new MaterialPropertyBlock();
            }

            InitAnimationNameList();
            Play(0);
        }

        /// <summary>
        /// 
        /// </summary>
        void OnEnable()
        {
            Init();
        }

        /// <summary>
        /// 
        /// </summary>
        protected void InitAnimationNameList()
        {
            m_Infos = GetInfoList();
            m_ActionNameToIndexDic.Clear();
            if (m_Infos == null)
            {
                return;
            }

            for (int i = 0; i < m_Infos.Length; i++)
            {
                GPUSkinInfoDB info = m_Infos[i];
                m_ActionNameToIndexDic[info.name] = i;
            }
        }
    }
}

