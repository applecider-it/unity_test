using UnityEngine;

using Game.Utils;
using Game.Systems;

namespace Game.Commons
{
    /// <summary>
    /// 固定の共通データ
    /// </summary>
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

        [Header("Audio")]
        [Tooltip("BGM情報")][SerializeField] private BGMInfo[] bgmInfoList;

        // このクラスのインスタンス
        private static CommonData instance = null;

        /// <summary>
        /// 共通データを返す
        /// </summary>
        public static CommonData GetInstance()
        {
            if (instance == null)
            {
                Debug.Log("CommonData: GetInstance: Create");
                GameObject obj = StageUtil.GetCommonScriptGameObject();
                instance = obj.GetComponent<CommonData>();
            }
            return instance;
        }

        /// <summary>
        /// 名前から、BGMInfoを返す。
        /// </summary>
        public BGMInfo GetBGMInfo(string bgmName)
        {
            foreach (var info in bgmInfoList)
            {
                if (info.Name == bgmName)
                {
                    return info;
                }
            }

            return null;
        }

        // getter

        public LayerMask GroundLayer { get => groundLayer; }
        public GameObject PKFire { get => pkFire; }
        public float LookIgnoreTime { get => lookIgnoreTime; }
        public GameObject Player { get => GameObject.Find(playerPath); }
        public GameObject Camera { get => GameObject.Find(cameraPath); }
    }
}