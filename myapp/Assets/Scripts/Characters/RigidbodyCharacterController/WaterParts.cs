using UnityEngine;
using System.Collections.Generic;

namespace Game.Characters.RigidbodyCharacterControllerParts
{
    /// <summary>
    /// 水中判定処理
    /// </summary>
    public class WaterParts
    {
        HashSet<Collider> inside = new HashSet<Collider>();

        /// <summary>
        /// トリガー開始時
        /// </summary>
        public void OnTriggerEnter(Collider other)
        {
            Debug.Log("入った: " + other.tag);
            if (other.tag == "Water")
            {
                inside.Add(other);
            }
        }

        /// <summary>
        /// トリガー終了時
        /// </summary>
        public void OnTriggerExit(Collider other)
        {
            Debug.Log("出た: " + other.tag);
            if (other.tag == "Water")
            {
                inside.Remove(other);
            }
        }

        /// <summary>
        /// 指定のポイントが、水のコライダーのどれかに含まれるか返す。
        /// </summary>
        public bool PointCheck(Vector3 p)
        {
            foreach (var col in inside)
            {
                if (col == null) continue;

                Vector3 closest = col.ClosestPoint(p);

                // 内部なら、完全に一致する (unityの仕様上完全には一致しないので誤差を容認する)
                if ((closest - p).sqrMagnitude < 0.0001f)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 水のコライダーのどれかに入っているか。
        /// </summary>
        public bool InsideCheck()
        {
            return inside.Count > 0;
        }
    }
}