using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Game.Characters;

namespace Game.Stages
{
    /// <summary>
    /// シーン接続管理
    /// </summary>
    public class SceneConnector : MonoBehaviour
    {
        [SerializeField] private Vector3 startPosition;
        [SerializeField] private string sceneName;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                string name = "Scenes/" + sceneName + "Scene";
                Debug.Log("Player が入ってきた。" + name + " " + startPosition);

                SceneManager.LoadScene(name);
            }
        }
    }
}