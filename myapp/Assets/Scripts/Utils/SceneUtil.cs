using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Utils
{
    /// <summary>
    /// シーン管理ユーティリティ
    /// </summary>
    public class SceneUtil
    {
        /// <summary>
        /// 特定のシーン以外をアンロード
        /// </summary>
        public static void UnloadAllExcept(string sceneName)
        {
            Scene keep = SceneManager.GetSceneByName(sceneName);

            if (!keep.isLoaded)
            {
                Debug.LogError($"Scene {sceneName} is not loaded");
                return;
            }

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene s = SceneManager.GetSceneAt(i);
                if (s != keep)
                {
                    SceneManager.UnloadSceneAsync(s);
                }
            }
        }
    }
}