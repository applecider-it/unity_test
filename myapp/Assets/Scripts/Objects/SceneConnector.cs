using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Game.Characters;
using Game.Stages;
using Game.Utils;
using Game.Commons;

namespace Game.Objects
{
    /// <summary>
    /// シーン接続管理
    /// </summary>
    public class SceneConnector : MonoBehaviour
    {
        [SerializeField] private SceneConnectorInfo info;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                LoadNextStage();
            }
        }

        /// <summary>
        /// 現在のステージのアンロード。
        /// 次のステージのロード。
        /// 接続情報を残す。
        /// </summary>
        void LoadNextStage()
        {
            string name = "Scenes/" + info.sceneName + "Scene";
            Debug.Log("Player が入ってきた。" + name + " " + info);

            CommonStatus cs = CommonStatus.getCommonStatus();
            cs.NextSceneConnectorInfo = info;

            SceneUtil.UnloadAllExcept("Scenes/CommonScene");
            SceneManager.LoadScene(name, LoadSceneMode.Additive);
        }
    }
}