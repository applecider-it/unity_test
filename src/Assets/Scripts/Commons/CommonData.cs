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
        [Tooltip("地面の法線の逆向きのVelocityの強さ")][SerializeField] private float groundStick = 2f;
        [Tooltip("地面判定を維持するフレーム数")][SerializeField] private int groundKeep = 4;

        [Header("Input")]
        [Tooltip("開始直後にLookを無視する時間")][SerializeField] float lookIgnoreTime = 0.2f;

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
        public int GroundKeep { get => groundKeep; }
        public float GroundStick { get => groundStick; }

        public float LookIgnoreTime { get => lookIgnoreTime; }

        public GameObject Player { get => GameObject.Find(playerPath); }
        public GameObject Camera { get => GameObject.Find(cameraPath); }
    }
}