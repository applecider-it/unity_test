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
            Vector3 moveDir, float rotationSpeed, float rotationSpeedWater,
            Vector3 hangNormal, bool noMove,
            CharacterActionType actionType
        )
        {
            bool needBack = true;

            switch (actionType)
            {
                case CharacterActionType.Water:
                    if (!noMove)
                    {
                        // 入力があるとき

                        TurnProcces(moveDir, rotationSpeedWater);

                        needBack = false;
                    }
                    break;

                case CharacterActionType.Ground:
                    if (!noMove)
                    {
                        // 入力があるとき

                        TurnProcces(moveDir, rotationSpeed);

                        needBack = false;
                    }
                    break;

                case CharacterActionType.Hang:
                    HangProcces(hangNormal);

                    needBack = false;
                    break;
            }

            if (needBack) BackProcces(rotationSpeed);
        }

        /// <summary>
        /// 掴むプロセス
        /// </summary>
        void HangProcces(Vector3 hangNormal)
        {
            Vector3 dir = hangNormal.normalized;

            // 移動方向を向く回転
            Quaternion targetRotation = Quaternion.LookRotation(-dir);

            rb.MoveRotation(targetRotation);
        }

        /// <summary>
        /// 回転プロセス
        /// </summary>
        void TurnProcces(Vector3 moveDir, float speed)
        {
            // 移動方向を向く回転
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);

            // 現在の向きからスムーズに補間
            rb.MoveRotation(
                Quaternion.Slerp(
                    rb.rotation,
                    targetRotation,
                    speed * Time.fixedDeltaTime
                )
            );
        }

        /// <summary>
        /// 戻すプロセス
        /// </summary>
        void BackProcces(float rotationSpeed)
        {
            // 傾きだけを戻す
            Quaternion uprightRotation =
                Quaternion.Euler(0f, rb.rotation.eulerAngles.y, 0f);

            rb.MoveRotation(
                Quaternion.Slerp(
                    rb.rotation,
                    uprightRotation,
                    rotationSpeed * Time.fixedDeltaTime
                )
            );
        }
    }
}