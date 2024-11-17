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
    public class GPUSkinVertexTool : GPUSkinTool
    {
        #region Defines

        /// <summary>
        /// 
        /// </summary>
        const int FRAME_RAME_MULTI = 1;

        /// <summary>
        /// 
        /// </summary>
        static string GPU_SKIN_VERTEX_DIR
        {
            get { return "/GPUSkinVertex/"; }
        }

        /// <summary>
        /// 
        /// </summary>
        static string SHADER_NAME
        {
            get { return "Character/GPUSkinVertex"; }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="clips"></param>
        /// <returns></returns>
        public static GPUSkinVertexInfoDB CreateGPUSkinVertexInfo(GameObject obj, List<AnimationClip> clips)
        {
            if (obj == null || clips == null || clips.Count == 0)
            {
                return null;
            }

            obj = GameObject.Instantiate(obj);

            Animator animator = obj.GetComponent<Animator>();
            SkinnedMeshRenderer[] smrs = obj.GetComponentsInChildren<SkinnedMeshRenderer>();

            if (animator == null || smrs.Length != 1)
            {
                GameObject.DestroyImmediate(obj);
                return null;
            }

            AnimatorController controller = GenerateAnimatorController(clips);
            animator.runtimeAnimatorController = controller;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

            GPUSkinVertexInfoDB info = ScriptableObject.CreateInstance<GPUSkinVertexInfoDB>();
            info.mesh = CreateNewMesh(smrs[0]);
            CreateAnimationInfo(info, clips);

            BakeAnimation2Tex(animator, smrs[0], info);
            GameObject.DestroyImmediate(obj);

            return info;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="smr"></param>
        /// <param name="animationInfo"></param>
        static void BakeAnimation2Tex(Animator animator, SkinnedMeshRenderer smr, GPUSkinVertexInfoDB animationInfo)
        {
            int texWidth = animationInfo.texWidth;
            int texHeight = animationInfo.texHeight;

            Mesh newMesh = new Mesh();

            foreach (GPUSkinInfoDB info in animationInfo.infoList)
            {
                GPUSkinEditorUtils.BakeAnimatorFrame(animator, info.name, info.frameRate, info.frameCount);

                for (int frameIndex = 0; frameIndex < info.frameCount; ++frameIndex)
                {
                    float playbackTime = (frameIndex) * (1.0f / info.frameRate);

                    // 防止生成警告
                    if (frameIndex == info.frameCount - 1)
                    {
                        playbackTime = (frameIndex - 1) * (1.0f / info.frameRate);
                    }

                    //Debug.Log("BakeAnimation2Tex Vertex : " + info.name + " : " + playbackTime);

                    animator.playbackTime = playbackTime;
                    animator.Update(0);

                    smr.BakeMesh(newMesh);
                    Vector3[] newMeshVertexes = newMesh.vertices;

                    for (int vertexIndex = 0; vertexIndex < newMeshVertexes.Length; ++vertexIndex)
                    {
                        int pixelCol = (info.startPixelIndex + (frameIndex * newMeshVertexes.Length + vertexIndex)) % texWidth;
                        int pixelRow = (info.startPixelIndex + (frameIndex * newMeshVertexes.Length + vertexIndex)) / texWidth;
                        Vector3 pos = newMeshVertexes[vertexIndex];
                        animationInfo.texture.SetPixel(pixelCol, pixelRow, new Color(pos.x, pos.y, pos.z, 0));
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="clips"></param>
        static void CreateAnimationInfo(GPUSkinVertexInfoDB info, List<AnimationClip> clips)
        {
            int vertexCount = info.mesh.vertexCount;
            int texWidth = 0;
            int texHeight = 0;
            int pixelCount = 0;

            for (int i = 0; i < clips.Count; ++i)
            {
                AnimationClip clip = clips[i];
                int frameCount = (int)(clip.length * clip.frameRate * FRAME_RAME_MULTI);
                pixelCount += frameCount * vertexCount;
            }

            if (!GetTexture2DSize(pixelCount, ref texWidth, ref texHeight))
            {
                return;
            }

            // GPUSkinVertexAnimationInfo info = new GPUSkinVertexAnimationInfo();
            info.texWidth = texWidth;
            info.texHeight = texHeight;
            info.vertexCount = vertexCount;
            info.texture = new Texture2D(texWidth, texHeight, TextureFormat.RGBAHalf, false);
            info.texture.filterMode = FilterMode.Point;
            info.texture.anisoLevel = 0;

            int totalPixelCount = 0;

            info.infoList = new GPUSkinInfoDB[clips.Count];
            for (int i = 0; i < clips.Count; ++i)
            {
                AnimationClip clip = clips[i];
                int frameCount = (int)(clip.length * clip.frameRate * FRAME_RAME_MULTI);
                pixelCount = frameCount * vertexCount;

                GPUSkinInfoDB animationInfo = new GPUSkinInfoDB();
                animationInfo.name = clip.name;
                animationInfo.startPixelIndex = totalPixelCount;
                animationInfo.totalPixelCount = pixelCount;
                animationInfo.frameRate = clip.frameRate * FRAME_RAME_MULTI;
                animationInfo.frameCount = frameCount;
                animationInfo.length = clip.length;

                info.infoList[i] = animationInfo;
                totalPixelCount += pixelCount;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="smr"></param>
        /// <returns></returns>
        static Mesh CreateNewMesh(SkinnedMeshRenderer smr)
        {
            Mesh newMesh = Object.Instantiate(smr.sharedMesh);
            newMesh.boneWeights = null;

            return newMesh;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clips"></param>
        /// <returns></returns>
        static AnimatorController GenerateAnimatorController(List<AnimationClip> clips)
        {
            AnimatorController controller = new AnimatorController();
            controller.AddLayer("Action");

            for (int i = 0; i < clips.Count; ++i)
            {
                string clipName = clips[i].name;
                int layerIndex = 0;
                string stateName = clipName;

                AnimatorControllerLayer layer = controller.layers[layerIndex];
                UnityEditor.Animations.AnimatorState state = layer.stateMachine.AddState(stateName);
                state.motion = clips[i];
            }

            return controller;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pixelCount"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        static bool GetTexture2DSize(int pixelCount, ref int width, ref int height)
        {
            width = (int)(Mathf.Sqrt(pixelCount) + 1);
            width = (width % 2 == 0) ? width : width + 1;
            height = width;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        [MenuItem("Assets/Art/Char/Tools/CreateGPUSkinVertexBake")]
        static void ToolsGPUSkinVertexBake()
        {
            //string dirAssetPath = AssetDatabase.GetAssetPath(Selection.activeObject) + "/";

            string dirAssetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            dirAssetPath = Path.GetDirectoryName(dirAssetPath) + "/";

            GPUSkinVertexBake(dirAssetPath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dirAssetPath"></param>
        public static void GPUSkinVertexBake(string dirAssetPath)
        {
            GameObject obj = GPUSkinEditorUtils.GetDirModleGameObject(dirAssetPath);
            List<AnimationClip> clips = GPUSkinEditorUtils.GetDirAnimationClips(dirAssetPath);
            GPUSkinVertexInfoDB info = CreateGPUSkinVertexInfo(obj, clips);

            string outputDir = dirAssetPath + GPU_SKIN_VERTEX_DIR;

            GPUSkinEditorUtils.CreateAsset(info.mesh, outputDir + "Mesh.asset");
            GPUSkinEditorUtils.CreateAsset(info.texture, outputDir + "Tex.asset");
            GPUSkinEditorUtils.CreateAsset(info, outputDir + "Info.asset");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcpath"></param>
        /// <param name="dstpath"></param>
        public static void GPUSkinVertexBake(string srcpath, string dstpath)
        {
            GameObject obj = GPUSkinEditorUtils.GetDirModleGameObject(srcpath);
            List<AnimationClip> clips = GPUSkinEditorUtils.GetDirAnimationClips(srcpath);
            GPUSkinVertexInfoDB info = CreateGPUSkinVertexInfo(obj, clips);

            if (obj == null || clips == null || info == null)
                return;

            string outputSrcDir = dstpath + GPU_SKIN_DIR;
            var outputDir = outputSrcDir.ToLower();

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            string dstMeshPath = outputDir + MESH_NAME;
            dstMeshPath = dstMeshPath.ToLower();

            string dstTexPath = outputDir + TEX_NAME;
            dstTexPath = dstTexPath.ToLower();

            string dstInfoPath = outputDir + INFO_NAME;
            dstInfoPath = dstInfoPath.ToLower();

            GPUSkinEditorUtils.CreateAsset(info.mesh, dstMeshPath);
            GPUSkinEditorUtils.CreateAsset(info.texture, dstTexPath);
            GPUSkinEditorUtils.CreateAsset(info, dstInfoPath);

            string prefabname = (obj.name + "_" + PREFAB_NAME).ToLower();

            var shadername = SHADER_NAME;

            GPUSkinEditorUtils.CreatePrefab(dstMeshPath, dstTexPath, dstInfoPath, shadername, false, srcpath, outputDir, outputSrcDir, prefabname);
        }
    }
}

