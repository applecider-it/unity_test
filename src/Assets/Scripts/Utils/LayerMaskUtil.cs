using UnityEngine;

namespace Game.Utils
{
    /// <summary>
    /// Layer用ユーティリティ
    /// </summary>
    public class LayerMaskUtil
    {
        /// <summary>
        /// ゲームオブジェクトがLayermaskに含まれるか返す。
        /// </summary>
        public static bool checkLayerMask(GameObject obj, LayerMask groundLayer)
        {
            return (groundLayer.value & (1 << obj.layer)) != 0;
        }

    }
}