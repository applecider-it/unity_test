using UnityEngine;

using Game.Utils;
using Game.Stages;

namespace Game.Commons
{
    public class CommonStatus : MonoBehaviour
    {
        // このクラスのインスタンス
        private static CommonStatus instance = null;

        // シーン接続管理
        private SceneConnectorInfo nextSceneConnectorInfo = null;

        /// <summary>
        /// 共通ステータスを返す
        /// </summary>
        public static CommonStatus getCommonStatus()
        {
            if (instance == null)
            {
                Debug.Log("getCommonStatus: SetIncetance");
                GameObject obj = StageUtil.GetCommonScriptGameObject();
                instance = obj.GetComponent<CommonStatus>();
                Debug.Log(instance);
            }
            return instance;
        }

        // getter setter

        public SceneConnectorInfo NextSceneConnectorInfo
        {
            get => nextSceneConnectorInfo;
            set => nextSceneConnectorInfo = value;
        }
    }
}