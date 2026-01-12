using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Game.Characters;
using Game.Systems;
using Game.Utils;

namespace Game.Objects
{
    [System.Serializable]
    public class SceneConnectorInfoClass
    {
        public Vector3 startPosition;
        public string sceneName;
    }

    /// <summary>
    /// シーン接続管理
    /// </summary>
    public class SceneConnector : MonoBehaviour
    {
        [SerializeField] private SceneConnectorInfoClass info;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                string name = "Scenes/" + info.sceneName + "Scene";
                Debug.Log("Player が入ってきた。" + name + " " + info);

                StaticData.SceneConnectorInfo = info;

                SceneUtil.UnloadAllExcept("Scenes/CommonScene");
                SceneManager.LoadScene(name, LoadSceneMode.Additive);
            }
        }
    }
}