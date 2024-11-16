using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;


namespace ST.GPUSkin
{
    /// <summary>
    /// 
    /// </summary>
    public class GPUSkinTool
    {
        #region Defines

        /// <summary>
        /// 
        /// </summary>
        public static string GPU_SKIN_DIR
        {
            get { return "/GPUSkin/"; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected static string MESH_NAME
        {
            get { return "Mesh.asset"; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected static string TEX_NAME
        {
            get { return "Tex.asset"; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected static string INFO_NAME
        {
            get { return "Info.asset"; }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string PREFAB_NAME
        {
            get { return "GpuSkin"; }
        }

        #endregion
    }
}

