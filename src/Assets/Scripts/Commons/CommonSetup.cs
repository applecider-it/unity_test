using UnityEngine;
using UnityEngine.SceneManagement;

using Game.Stages;

namespace Game.Commons
{
    /// <summary>
    /// 共通シーンのセットアップ
    /// </summary>
    public class CommonSetup : MonoBehaviour
    {
        [SerializeField] GameStages firstStage;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            if (firstStage != GameStages.None)
            {
                string name = "Scenes/" + firstStage.ToString() + "Scene";

                Debug.Log("Load First Stage: " + name);

                SceneManager.LoadScene(name, LoadSceneMode.Additive);
            }
        }
    }
}