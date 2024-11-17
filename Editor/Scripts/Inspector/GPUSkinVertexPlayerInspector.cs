using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace ST.GPUSkin
{
    /// <summary>
    /// 
    /// </summary>
    [CustomEditor(typeof(GPUSkinVertexPlayer), true)]
    public class GPUSkinVertexPlayerInspector : Editor
    {
        /// <summary>
        /// 
        /// </summary>
        List<GPUSkinInfoDB> m_Infos = new List<GPUSkinInfoDB>();
        
        /// <summary>
        /// 
        /// </summary>
        GPUSkinVertexPlayer m_Target
        {
            get { return target as GPUSkinVertexPlayer; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnInspectorGUI()
        {
            DrawBaseInspector();
            DrawPlayBtn();
        }

        /// <summary>
        /// 
        /// </summary>
        void DrawBaseInspector()
        {
            m_Target.meshRenderer = EditorGUILayout.ObjectField("MeshRender", m_Target.meshRenderer, 
                typeof(MeshRenderer), true) as MeshRenderer;

            m_Target.infoDB = EditorGUILayout.ObjectField("GPUSkinVertexInfoDB", m_Target.infoDB,
                typeof(GPUSkinVertexInfoDB), false) as GPUSkinVertexInfoDB;
        }

        /// <summary>
        /// 绘制播放按钮
        /// </summary>
        void DrawPlayBtn()
        {
            if (GPUSkinEditorUtils.DrawHeader("Play Animation"))
            {
                EditorGUILayout.BeginVertical("Box", GUILayout.MinHeight(10f));

                m_Infos.Clear();
                if (m_Target != null)
                {
                    m_Target.GetInfos(m_Infos);
                }

                for (int i = 0; i < m_Infos.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    GPUSkinInfoDB info = m_Infos[i];

                    string actionName = info.name;

                    GUILayout.Label(actionName, GUILayout.Width(180));
                    GUILayout.Label(info.length.ToString(), GUILayout.Width(100));

                    if (GUILayout.Button("Play"))
                    {
                        m_Target.Play(actionName);
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();
            }

        }
    }
}
