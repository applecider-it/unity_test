using UnityEngine;
using System.Collections.Generic;

namespace Game.Characters.RigidbodyCharacterControllerParts
{
    /// <summary>
    /// 水中判定処理
    /// </summary>
    public class WaterParts
    {
        HashSet<Collider> colliders = new HashSet<Collider>();
        private string name;

        // コンストラクタ
        public WaterParts(string argName)
        {
            name = argName;
        }

        /// <summary>
        /// トリガー開始時
        /// </summary>
        public void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Water")
            {
                colliders.Add(other);
                Debug.Log(name + " が、水(" + other.name + ") に入った。cnt: " + colliders.Count);
            }
        }

        /// <summary>
        /// トリガー終了時
        /// </summary>
        public void OnTriggerExit(Collider other)
        {
            if (other.tag == "Water")
            {
                colliders.Remove(other);
                Debug.Log(name + " が、水(" + other.name + ") から出た。cnt: " + colliders.Count);
            }
        }

        /// <summary>
        /// 指定のポイントが、水のコライダーのどれかに含まれるか返す。
        /// </summary>
        public bool PointCheck(Vector3 p)
        {
            foreach (var col in colliders)
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
            return colliders.Count > 0;
        }
    }
}