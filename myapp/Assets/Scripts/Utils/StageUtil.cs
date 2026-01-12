using UnityEngine;

namespace Game.Utils
{
    /// <summary>
    /// ステージ管理ユーティリティ
    /// </summary>
    public class StageUtil
    {
        /// <summary>
        /// 共通ステージシーンのスクリプトがあるゲームオブジェクトを返す
        /// </summary>
        public static GameObject GetCommonScriptGameObject()
        {
            GameObject obj = GameObject.Find("Common/Script");
            return obj;
        }

    }
}