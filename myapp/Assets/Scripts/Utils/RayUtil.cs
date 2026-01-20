using UnityEngine;

namespace Game.Utils
{
    /// <summary>
    /// Ray関連のユーティリティ
    /// </summary>
    public class RayUtil
    {
        /// <summary>
        /// 実際の法線を取得する
        /// 
        /// コライダーの接触時に、面同士の合算された法線が返ってくることがあるので、その時のために必要。
        /// </summary>
        public static void GetTrueNormal(
            ContactPoint contact, out bool trueResult, out Vector3 trueNormal
        )
        {
            trueResult = false;
            trueNormal = Vector3.zero;

            // 接点
            Vector3 point = contact.point;

            // Rayを発射する方向
            Vector3 dir = -contact.normal;

            // 余白
            float space = 0.01f;

            // Rayを発射する位置
            Vector3 origin = point - (dir * space);

            // Rayの距離
            float distance = space * 2f;

            Ray ray = new Ray(origin, dir);

            if (Physics.Raycast(ray, out RaycastHit hit, distance))
            {
                trueResult = true;
                trueNormal = hit.normal;
            }
        }

    }
}