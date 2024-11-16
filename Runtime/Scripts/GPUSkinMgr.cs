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
        public static GPUSkinMgr instance
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
        /// 
        /// </summary>
        public void OnUpdate()
        {

        }
    }
}

