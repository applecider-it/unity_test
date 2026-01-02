using UnityEngine;

namespace Game.Util
{
    /// <summary>
    /// Physics系の判定を Sceneビューに可視化するためのデバッグ用ユーティリティ
    /// </summary>
    public static class PhysicsDebugUtil
    {
        /// <summary>
        /// SphereCast のデバッグ表示
        /// 「開始球 → 移動方向 → 終点球」を描画する
        /// </summary>
        /// <param name="origin">SphereCastの開始位置</param>
        /// <param name="radius">球の半径</param>
        /// <param name="direction">Castする方向</param>
        /// <param name="distance">Castする距離</param>
        /// <param name="color">描画色</param>
        /// <param name="drawEndSphere">終点の球を描画するか</param>
        public static void DrawSphereCast(
            Vector3 origin,
            float radius,
            Vector3 direction,
            float distance,
            Color color,
            bool drawEndSphere = true
        )
        {
            // 念のため方向ベクトルを正規化
            direction.Normalize();

            // Cast方向を線で描画
            Debug.DrawRay(origin, direction * distance, color);

            // 開始位置の球を描画
            DrawWireSphere(origin, radius, color);

            // 終点位置の球を描画（任意）
            if (drawEndSphere)
            {
                Vector3 end = origin + direction * distance;
                DrawWireSphere(end, radius, color);
            }
        }

        /// <summary>
        /// CheckSphere / OverlapSphere 用のデバッグ表示
        /// 球を1つ描くだけ
        /// </summary>
        /// <param name="center">球の中心座標</param>
        /// <param name="radius">球の半径</param>
        /// <param name="color">描画色</param>
        public static void DrawCheckSphere(
            Vector3 center,
            float radius,
            Color color
        )
        {
            DrawWireSphere(center, radius, color);
        }

        /// <summary>
        /// ワイヤーフレームの球を描画する
        /// Debug.DrawLine を使って疑似的に球を表現している
        /// </summary>
        /// <param name="center">球の中心</param>
        /// <param name="radius">球の半径</param>
        /// <param name="color">描画色</param>
        private static void DrawWireSphere(
            Vector3 center,
            float radius,
            Color color
        )
        {
            // 円を何分割するか（多いほど滑らか）
            const int segments = 16;
            float step = 360f / segments;

            for (int i = 0; i < segments; i++)
            {
                // 現在と次の角度（ラジアン）
                float a0 = step * i * Mathf.Deg2Rad;
                float a1 = step * (i + 1) * Mathf.Deg2Rad;

                // ===== XZ平面（地面に平行な円） =====
                Debug.DrawLine(
                    center + new Vector3(Mathf.Cos(a0), 0, Mathf.Sin(a0)) * radius,
                    center + new Vector3(Mathf.Cos(a1), 0, Mathf.Sin(a1)) * radius,
                    color
                );

                // ===== YZ平面（縦の円） =====
                Debug.DrawLine(
                    center + new Vector3(0, Mathf.Cos(a0), Mathf.Sin(a0)) * radius,
                    center + new Vector3(0, Mathf.Cos(a1), Mathf.Sin(a1)) * radius,
                    color
                );

                // ===== XY平面（縦の円） =====
                Debug.DrawLine(
                    center + new Vector3(Mathf.Cos(a0), Mathf.Sin(a0), 0) * radius,
                    center + new Vector3(Mathf.Cos(a1), Mathf.Sin(a1), 0) * radius,
                    color
                );
            }
        }
    }
}