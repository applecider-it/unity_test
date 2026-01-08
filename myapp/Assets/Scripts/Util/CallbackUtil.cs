using UnityEngine;
using System.Collections;

using Game.GameSystem;

namespace Game.Util
{
    /// <summary>
    /// コールバック関連管理ユーティリティ
    /// </summary>
    public class CallbackUtil
    {
        /// <summary>
        /// 指定秒数経過後にコールバックを呼ぶ
        /// </summary>
        public static IEnumerator CallAfterSeconds(float seconds, System.Action callback)
        {
            yield return new WaitForSeconds(seconds);
            callback?.Invoke();
        }

    }
}