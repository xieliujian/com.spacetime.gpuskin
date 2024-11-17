using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ST.GPUSkin
{
    /// <summary>
    /// 
    /// </summary>
    public class GPUSkinBoneInfoDB : ScriptableObject
    {
        /// <summary>
        /// 
        /// </summary>
        public Mesh mesh;

        /// <summary>
        /// 
        /// </summary>
        public GPUSkinInfoDB[] infoList;

        /// <summary>
        /// 
        /// </summary>
        public int texWidth;

        /// <summary>
        /// 
        /// </summary>
        public int texHeight;

        /// <summary>
        /// 
        /// </summary>
        public Texture2D texture;

        /// <summary>
        /// 
        /// </summary>
        public int boneCount;
    }
}


