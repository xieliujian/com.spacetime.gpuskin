using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ST.GPUSkin
{
    /// <summary>
    /// 
    /// </summary>
    public class GPUSkinEditorUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dirAssetPath"></param>
        /// <returns></returns>
        public static GameObject GetDirModleGameObject(string dirAssetPath)
        {
            List<string> objAssetPathList = GetDirSubFileAssetPaths(dirAssetPath);

            foreach (string objAssetPath in objAssetPathList)
            {
                if (!objAssetPath.ToLower().EndsWith(".fbx") || objAssetPath.ToLower().EndsWith(".meta"))
                {
                    continue;
                }

                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(objAssetPath);

                if (!objAssetPath.Contains("_") && obj != null)
                {
                    return obj;
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dirAssetPath"></param>
        /// <returns></returns>
        public static bool IsOneMesh(string dirAssetPath)
        {
            GameObject obj = GetDirModleGameObject(dirAssetPath);

            Animator animator = obj.GetComponent<Animator>();
            SkinnedMeshRenderer[] smrs = obj.GetComponentsInChildren<SkinnedMeshRenderer>();

            if (animator == null || smrs.Length != 1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dirAssetPath"></param>
        /// <returns></returns>
        public static List<Material> GetDirMaterials(string dirAssetPath)
        {
            List<Material> mats = new List<Material>();
            List<string> objAssetPathList = GetDirSubFilePathList(dirAssetPath + "/mat/", true, "mat");

            for (int i = 0; i < objAssetPathList.Count; i++)
            {
                string objAssetPath = objAssetPathList[i];
                if (!objAssetPath.ToLower().EndsWith(".mat") || objAssetPath.ToLower().EndsWith(".meta") || !objAssetPath.Contains("_"))
                {
                    continue;
                }

                objAssetPath = GetAssetPathFromFullPath(objAssetPath);
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(objAssetPath);

                var maintex = mat.GetTexture("_BaseMap");
                if (maintex != null)
                {
                    if (mat != null && !mat.name.Contains("__preview__"))
                    {
                        mats.Add(mat);
                    }
                }
            }

            return mats;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dirAssetPath"></param>
        /// <returns></returns>
        public static List<AnimationClip> GetDirAnimationClips(string dirAssetPath)
        {
            List<AnimationClip> clips = new List<AnimationClip>();
            List<string> objAssetPathList = GetDirSubFilePathList(dirAssetPath + "/animation/", true, "fbx");

            for (int i = 0; i < objAssetPathList.Count; i++)
            {
                string objAssetPath = objAssetPathList[i];
                if (!objAssetPath.ToLower().EndsWith(".fbx") || objAssetPath.ToLower().EndsWith(".meta") || !objAssetPath.Contains("_"))
                {
                    continue;
                }

                objAssetPath = GetAssetPathFromFullPath(objAssetPath);
                Object[] objs = AssetDatabase.LoadAllAssetsAtPath(objAssetPath);
                foreach (Object obj in objs)
                {
                    AnimationClip clip = obj as AnimationClip;
                    if (clip != null && !clip.name.Contains("__preview__"))
                    {
                        clips.Add(clip);
                    }
                }
            }

            return clips;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="animationName"></param>
        /// <param name="frameRate"></param>
        /// <param name="frameCount"></param>
        public static void BakeAnimatorFrame(Animator animator, string animationName, float frameRate, int frameCount)
        {
            animator.StopPlayback();
            animator.Play(animationName);

            animator.recorderStartTime = 0;

            float recorderStopTime = 1.0f / frameRate * frameCount;
            animator.recorderStopTime = recorderStopTime;

            animator.StartRecording(frameCount);
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

            for (int i = 0; i < frameCount; ++i)
            {
                animator.Update(1.0f / frameRate);
            }

            animator.StopRecording();
            animator.StartPlayback();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="path"></param>
        public static void CreateAsset(UnityEngine.Object asset, string path)
        {
            string dirPath = AssetsPath2ABSPath(GetDirPath(path));
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            SafeRemoveAsset(path);
            AssetDatabase.CreateAsset(asset, path);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dstMeshPath"></param>
        /// <param name="dstTexPath"></param>
        /// <param name="dstInfoPath"></param>
        /// <param name="shadername"></param>
        /// <param name="isbonebake"></param>
        /// <param name="srcpath"></param>
        /// <param name="dstpath"></param>
        /// <param name="dstSrcPath"></param>
        /// <param name="prefabname"></param>
        public static void CreatePrefab(string dstMeshPath, string dstTexPath, string dstInfoPath, string shadername, bool isbonebake,
            string srcpath, string dstpath, string dstSrcPath, string prefabname)
        {
            var matlist = GetDirMaterials(srcpath);
            if (matlist == null)
                return;

            var shader = Shader.Find(shadername);
            if (shader == null)
                return;

            var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(dstMeshPath);
            if (mesh == null)
                return;

            var animtex = AssetDatabase.LoadAssetAtPath<Texture>(dstTexPath);
            if (animtex == null)
                return;

            var configPath = srcpath + GPUSkinGenConfigDB.CONFIG_FILE_NAME;
            var config = AssetDatabase.LoadAssetAtPath<GPUSkinGenConfigDB>(configPath);
            if (config == null)
                return;

            // 清理dstPath路径所有的材质和prefab
            ClearPrefabPathAllMat(dstSrcPath);
            ClearPrefabPathAllPrefab(dstSrcPath);

            int index = 0;
            for (int i = 0; i < matlist.Count; i++)
            {
                var mat = matlist[i];
                if (mat == null)
                    continue;

                var maintex = mat.GetTexture("_BaseMap");
                if (maintex == null)
                    continue;

                index++;

                // mat
                var matpath = dstpath + "/mat" + index + ".mat";
                var newMat = CreatePrefabMat(shader, maintex, isbonebake, animtex, matpath);
                config.RefreshMat(newMat, index);

                // prefab
                var prefabpath = dstpath + "/" + prefabname + index + ".prefab";
                CreatePrefab(srcpath, mesh, matpath, isbonebake, dstInfoPath, prefabpath);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool DrawHeader(string text) 
        {
            return DrawHeader(text, text, false, false); 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="key"></param>
        /// <param name="forceOn"></param>
        /// <param name="minimalistic"></param>
        /// <returns></returns>
        static public bool DrawHeader(string text, string key, bool forceOn, bool minimalistic)
        {
            bool state = EditorPrefs.GetBool(key, true);

            if (!minimalistic) GUILayout.Space(3f);
            if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
            GUILayout.BeginHorizontal();
            GUI.changed = false;

            if (minimalistic)
            {
                if (state) text = "\u25BC" + (char)0x200a + text;
                else text = "\u25BA" + (char)0x200a + text;

                GUILayout.BeginHorizontal();
                GUI.contentColor = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.7f) : new Color(0f, 0f, 0f, 0.7f);
                if (!GUILayout.Toggle(true, text, "PreToolbar2", GUILayout.MinWidth(20f))) state = !state;
                GUI.contentColor = Color.white;
                GUILayout.EndHorizontal();
            }
            else
            {
                text = "<b><size=11>" + text + "</size></b>";
                if (state) text = "\u25BC " + text;
                else text = "\u25BA " + text;
                if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) state = !state;
            }

            if (GUI.changed) EditorPrefs.SetBool(key, state);

            if (!minimalistic) GUILayout.Space(2f);
            GUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;
            if (!forceOn && !state) GUILayout.Space(3f);
            return state;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dirAssetPath"></param>
        /// <returns></returns>
        static List<string> GetDirSubFileAssetPaths(string dirAssetPath)
        {
            List<string> result = new List<string>();
            string[] fileAbsPaths = Directory.GetFiles(dirAssetPath);

            foreach (string fileAbsPath in fileAbsPaths)
            {
                string fileAssetPath = ABSPath2AssetsPath(fileAbsPath);
                result.Add(fileAssetPath);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="absPath"></param>
        /// <returns></returns>
        static string ABSPath2AssetsPath(string absPath)
        {
            string assetRootPath = System.IO.Path.GetFullPath(Application.dataPath);
            return "Assets" + System.IO.Path.GetFullPath(absPath).Substring(assetRootPath.Length).Replace("\\", "/");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shader"></param>
        /// <param name="maintex"></param>
        /// <param name="isbonebake"></param>
        /// <param name="animtex"></param>
        /// <param name="matpath"></param>
        static Material CreatePrefabMat(Shader shader, Texture maintex, bool isbonebake, Texture animtex, string matpath)
        {
            Material newmat = new Material(shader);
            if (newmat == null)
                return null;

            newmat.SetTexture("_MainTex", maintex);

            if (isbonebake)
            {
                newmat.SetTexture("_BoneAnimTex", animtex);
            }
            else
            {
                newmat.SetTexture("_VertexAnimTex", animtex);
            }

            newmat.enableInstancing = true;

            AssetDatabase.CreateAsset(newmat, matpath);

            return newmat;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcpath"></param>
        /// <param name="mesh"></param>
        /// <param name="matpath"></param>
        /// <param name="isbonebake"></param>
        /// <param name="dstInfoPath"></param>
        /// <param name="prefabpath"></param>
        static void CreatePrefab(string srcpath, Mesh mesh, string matpath, bool isbonebake, string dstInfoPath, string prefabpath)
        {
            GameObject obj = GPUSkinEditorUtils.GetDirModleGameObject(srcpath);
            if (obj == null)
                return;

            SkinnedMeshRenderer[] smrs = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
            if (smrs.Length != 1)
                return;

            SkinnedMeshRenderer smr = obj.GetComponentInChildren<SkinnedMeshRenderer>();
            if (smr == null)
                return;

            var localPos = smr.transform.localPosition;
            var localRot = smr.transform.localRotation;
            var localScale = smr.transform.localScale;

            GameObject newGo = new GameObject(obj.name);
            GameObject skinGo = new GameObject("mesh");
            if (newGo != null && skinGo != null)
            {
                skinGo.transform.SetParent(newGo.transform);
                skinGo.transform.localPosition = localPos;
                skinGo.transform.localRotation = localRot;
                skinGo.transform.localScale = localScale;

                var meshfilter = skinGo.AddComponent<MeshFilter>();
                if (meshfilter != null)
                {
                    meshfilter.sharedMesh = mesh;
                }

                var meshRender = skinGo.AddComponent<MeshRenderer>();
                if (meshRender != null)
                {
                    var newmat = AssetDatabase.LoadAssetAtPath<Material>(matpath);
                    if (newmat != null)
                    {
                        meshRender.sharedMaterial = newmat;
                    }
                }

                if (meshRender != null)
                {
                    if (isbonebake)
                    {
                        var assetInfo = AssetDatabase.LoadAssetAtPath<GPUSkinBoneInfoDB>(dstInfoPath);
                        if (assetInfo != null)
                        {
                            var script = newGo.AddComponent<GPUSkinBonePlayer>();
                            if (script != null)
                            {
                                script.meshRenderer = meshRender;
                                script.infoDB = assetInfo;
                            }
                        }
                    }
                    else
                    {
                        var assetInfo = AssetDatabase.LoadAssetAtPath<GPUSkinVertexInfoDB>(dstInfoPath);
                        if (assetInfo != null)
                        {
                            var script = newGo.AddComponent<GPUSkinVertexPlayer>();
                            if (script != null)
                            {
                                script.meshRenderer = meshRender;
                                script.infoDB = assetInfo;
                            }
                        }
                    }
                }

                PrefabUtility.SaveAsPrefabAsset(newGo, prefabpath);

                GameObject.DestroyImmediate(newGo);
            }
        }

        /// <summary>
        /// 清理dstPath路径所有的材质和prefab
        /// </summary>
        /// <param name="dstpath"></param>
        static void ClearPrefabPathAllMat(string dstpath)
        {
            List<string> objAssetPathList = GetDirSubFilePathList(dstpath, true, "mat");

            for (int i = 0; i < objAssetPathList.Count; i++)
            {
                string objAssetPath = objAssetPathList[i];
                objAssetPath = GetAssetPathFromFullPath(objAssetPath);

                SafeRemoveAsset(objAssetPath);
            }
        }

        /// <summary>
        /// 清理dstPath路径所有的材质和prefab
        /// </summary>
        /// <param name="dstpath"></param>
        static void ClearPrefabPathAllPrefab(string dstpath)
        {
            List<string> objAssetPathList = GetDirSubFilePathList(dstpath, true, "prefab");

            for (int i = 0; i < objAssetPathList.Count; i++)
            {
                string objAssetPath = objAssetPathList[i];
                objAssetPath = GetAssetPathFromFullPath(objAssetPath);

                SafeRemoveAsset(objAssetPath);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetsPath"></param>
        /// <returns></returns>
        static string AssetsPath2ABSPath(string assetsPath)
        {
            string assetRootPath = System.IO.Path.GetFullPath(Application.dataPath);
            return assetRootPath.Substring(0, assetRootPath.Length - 6) + assetsPath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetsPath"></param>
        static void SafeRemoveAsset(string assetsPath)
        {
            Debug.Log("SafeRemoveAsset " + assetsPath);
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(assetsPath);

            if (obj != null)
            {
                AssetDatabase.DeleteAsset(assetsPath);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="absOrAssetsPath"></param>
        /// <returns></returns>
        static string GetDirPath(string absOrAssetsPath)
        {
            string name = absOrAssetsPath.Replace("\\", "/");
            int lastIndex = name.LastIndexOf("/");
            return name.Substring(0, lastIndex + 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        static string GetAssetPathFromFullPath(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
                return string.Empty;

            string assetsFolder = "Assets";
            int index = fullPath.IndexOf(assetsFolder);
            if (index < 0)
                return fullPath;

            return fullPath.Substring(index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dirABSPath"></param>
        /// <param name="isRecursive"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        static List<string> GetDirSubFilePathList(string dirABSPath, bool isRecursive = true, string suffix = "")
        {
            List<string> pathList = new List<string>();
            DirectoryInfo di = new DirectoryInfo(dirABSPath);

            if (!di.Exists)
            {
                return pathList;
            }

            FileInfo[] files = di.GetFiles();
            foreach (FileInfo fi in files)
            {
                if (!string.IsNullOrEmpty(suffix))
                {
                    if (!fi.FullName.EndsWith(suffix, System.StringComparison.CurrentCultureIgnoreCase))
                    {
                        continue;
                    }
                }
                pathList.Add(fi.FullName);
            }

            if (isRecursive)
            {
                DirectoryInfo[] dirs = di.GetDirectories();
                foreach (DirectoryInfo d in dirs)
                {
                    if (d.Name.Contains(".svn"))
                    {
                        continue;
                    }
                    pathList.AddRange(GetDirSubFilePathList(d.FullName, isRecursive, suffix));
                }
            }

            return pathList;
        }
    }
}

