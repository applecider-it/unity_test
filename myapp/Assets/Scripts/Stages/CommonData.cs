using UnityEngine;

using Game.Utils;

namespace Game.Stages
{
    public class CommonData : MonoBehaviour
    {
        [Header("Character")]
        [Tooltip("地面と判断するためのマスク")][SerializeField] private LayerMask groundLayer;
        [Tooltip("PKファイアー")][SerializeField] private GameObject pkFire;

        [Header("Input")]
        [SerializeField] float lookIgnoreTime = 0.2f; // 開始直後にLookを無視する時間

        [Header("GameObjects")]
        [Tooltip("プレイヤーのヒエラルキーパス")][SerializeField] private string playerPath;
        [Tooltip("カメラのヒエラルキーパス")][SerializeField] private string cameraPath;

        // このクラスのインスタンス
        private static CommonData instance = null;

        /// <summary>
        /// 共通データを返す
        /// </summary>
        public static CommonData getCommonData()
        {
            if (instance == null)
            {
                Debug.Log("getCommonData: SetIncetance");
                GameObject obj = StageUtil.GetCommonScriptGameObject();
                instance = obj.GetComponent<CommonData>();
                Debug.Log(instance);
            }
            return instance;
        }

        // getter

        public LayerMask GroundLayer { get => groundLayer; }
        public GameObject PKFire { get => pkFire; }
        public float LookIgnoreTime { get => lookIgnoreTime; }
        public GameObject Player { get => GameObject.Find(playerPath); }
        public GameObject Camera { get => GameObject.Find(cameraPath); }
    }
}