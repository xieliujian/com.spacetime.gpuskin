﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ST.GPUSkin
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public struct GPUSkinInfo
    {
        public string name;
        public int startPixelIndex;
        public int totalPixelCount;
        public int frameCount;
        public float frameRate;
        public float length;
    }

    /// <summary>
    /// 
    /// </summary>
    public class GPUSkinBoneInfo : ScriptableObject
    {
        public Mesh mesh;
        public GPUSkinInfo[] infoList;
        public int texWidth;
        public int texHeight;
        public Texture2D texture;

        public int boneCount;
    }
}

