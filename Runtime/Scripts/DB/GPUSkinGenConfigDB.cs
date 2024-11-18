
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace ST.GPUSkin
{
    /// <summary>
    /// 
    /// </summary>
    public enum GPUSkinGenBakeType
    {
        // ��������
        BoneBake,

        // �������
        VertexBake,
    }

    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = CONFIG_FILE_NAME, menuName = "SpaceTime/GPUSkin/Config")]
    public class GPUSkinGenConfigDB : ScriptableObject
    {
        /// <summary>
        /// 
        /// </summary>
        const string ALPHATEST_ON_KEYWORD = "_ALPHATEST_ON";

        /// <summary>
        /// 
        /// </summary>
        static readonly int SRC_BLEND_ID = Shader.PropertyToID("_SrcBlend");
        static readonly int DST_BLEND_ID = Shader.PropertyToID("_DstBlend");
        static readonly int ALPHA_CLIP_ID = Shader.PropertyToID("_AlphaClip");
        static readonly int ZTEST_ID = Shader.PropertyToID("_ZTest");
        static readonly int ZWRITE_ID = Shader.PropertyToID("_ZWrite");
        static readonly int ZCLIP_ID = Shader.PropertyToID("_ZClip");

        /// <summary>
        /// 
        /// </summary>
        const string CONFIG_FILE_NAME = "Config.asset";

        /// <summary>
        /// 
        /// </summary>
        [Header("ʹ�ù������໹�Ƕ������")]
        public GPUSkinGenBakeType type = GPUSkinGenBakeType.VertexBake;

        /// <summary>
        /// 
        /// </summary>
        [Header("�Զ���RenderQueue")]
        public bool isCustomRenderQueue = false;

        /// <summary>
        /// 
        /// </summary>
        public int renderQueue = 2000;

        /// <summary>
        /// �Ƿ����renderQueue����
        /// </summary>
        [Header("�Ƿ�RenderQueue���ݲ�����������")]
        public bool isRenderQueueIncrease = false;

        /// <summary>
        /// 
        /// </summary>
        [Header("�Ƿ�AlphaBlend")]
        public bool isAlphaBlend = false;

        /// <summary>
        /// 
        /// </summary>
        [Header("�Ƿ�AlphaClip")]
        public bool isAlphaClip = false;

        /// <summary>
        /// 
        /// </summary>
        public float alphaClip = 0.5f;

        /// <summary>
        /// 
        /// </summary>
        [Header("�Ƿ�����ZTest")]
        public bool isSetZTest = false;

        /// <summary>
        /// 
        /// </summary>
        public UnityEngine.Rendering.CompareFunction zTest = UnityEngine.Rendering.CompareFunction.LessEqual;

        /// <summary>
        /// 
        /// </summary>
        [Header("�Ƿ�����ZWrite")]
        public bool isSetZWrite = false;

        /// <summary>
        /// 
        /// </summary>
        public bool zWrite;

        /// <summary>
        /// 
        /// </summary>
        [Header("�Ƿ�����ZClip")]
        public bool isSetZClip = false;

        /// <summary>
        /// 
        /// </summary>
        public bool zClip;

        /// <summary>
        /// 
        /// </summary>
        [Header("�Ƿ���Ⱦ����SortObjects")]
        public bool isRenderSort = false;

        /// <summary>
        /// 
        /// </summary>
        public int renderSortIndexOffset = 0;

        /// <summary>
        /// 
        /// </summary>
        public bool IsBoneBake()
        {
            return type == GPUSkinGenBakeType.BoneBake;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="matPath"></param>
        /// <returns></returns>
        public void RefreshMat(Material mat, int index)
        {
            if (mat == null)
                return;

            if (isCustomRenderQueue)
            {
                mat.renderQueue = renderQueue;
            }

            if (isRenderQueueIncrease)
            {
                mat.renderQueue = renderQueue + index;
            }

            if (isAlphaBlend)
            {
                mat.SetFloat(SRC_BLEND_ID, (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetFloat(DST_BLEND_ID, (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            }

            if (isAlphaClip)
            {
                mat.EnableKeyword(ALPHATEST_ON_KEYWORD);
            }
            else
            {
                mat.DisableKeyword(ALPHATEST_ON_KEYWORD);
            }

            mat.SetFloat(ALPHA_CLIP_ID, Mathf.Clamp01(alphaClip));

            if (isSetZTest)
            {
                mat.SetFloat(ZTEST_ID, (float)zTest);
            }

            if (isSetZWrite)
            {
                mat.SetFloat(ZWRITE_ID, zWrite ? 1 : 0);
            }

            if (isSetZClip)
            {
                mat.SetFloat(ZCLIP_ID, zClip ? 1 : 0);
            }

#if UNITY_EDITOR

            AssetDatabase.SaveAssetIfDirty(mat);
#endif
        }
    }
}

