using UnityEngine;

namespace Game.Characters.RigidbodyCharacterControllerParts
{
    /// <summary>
    /// 回転処理
    /// </summary>
    public class RotationParts
    {
        private Rigidbody rb;

        // コンストラクタ
        public RotationParts(Rigidbody argRb)
        {
            rb = argRb;
        }

        /// <summary>
        /// 回転プロセス
        /// </summary>
        public void RotationProcces(
            Vector3 moveDir, bool isGrounded, float rotationSpeed,
            bool inWater, float rotationSpeedWater,
            bool isHang, Vector3 hangNormal, bool noMove
        )
        {
            if (isHang)
            {
                // つかまっているとき

                Vector3 dir = new Vector3(
                    hangNormal.x,
                    0,
                    hangNormal.z
                );

                dir.Normalize();

                // 移動方向を向く回転
                Quaternion targetRotation = Quaternion.LookRotation(-dir);

                rb.MoveRotation(targetRotation);
            }
            else
            {
                // つかまっていないとき

                if (!noMove)
                {
                    // 入力があるとき

                    if (isGrounded || inWater)
                    {
                        // 地面にいるか水中のとき

                        // 移動方向を向く回転
                        Quaternion targetRotation = Quaternion.LookRotation(moveDir);

                        // 現在の向きからスムーズに補間
                        rb.MoveRotation(
                            Quaternion.Slerp(
                                rb.rotation,
                                targetRotation,
                                (inWater ? rotationSpeedWater : rotationSpeed) * Time.fixedDeltaTime
                            )
                        );
                    }
                }
            }
        }
    }
}