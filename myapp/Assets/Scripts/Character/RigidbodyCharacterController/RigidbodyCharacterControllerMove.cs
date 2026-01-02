using UnityEngine;

namespace Game.Character.RigidbodyCharacterControllerParts
{
    public class RigidbodyCharacterControllerMove
    {
        private Rigidbody rb;

        // コンストラクタ
        public RigidbodyCharacterControllerMove(Rigidbody argRb)
        {
            rb = argRb;
        }
        /// <summary>
        /// 移動処理
        /// </summary>
        public void MoveProccess(
            bool noMove, float gravity, Vector2 moveInput, bool isGrounded, Vector3 groundNormal,
            ref Vector3 moveVelocity, float moveSpeed, float moveSpeedAir, float rotationSpeed
        )
        {
            // 重力の影響を与えたあとの、Velocity.y
            float nextVY = rb.linearVelocity.y - gravity;

            if (noMove)
            {
                // 入力なしの時

                StopInMoveProcces(nextVY, isGrounded, groundNormal, ref moveVelocity);
            }
            else
            {
                // 入力ありの時

                Vector3 moveDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

                MoveInMoveProcces(
                    nextVY, moveDir, isGrounded, groundNormal,
                    ref moveVelocity, moveSpeed, moveSpeedAir
                );

                RotationInMoveProcces(moveDir, isGrounded, rotationSpeed);
            }
        }

        /// <summary>
        /// 移動処理の移動プロセス
        /// </summary>
        void MoveInMoveProcces(
            float nextVY, Vector3 moveDir, bool isGrounded, Vector3 groundNormal,
            ref Vector3 moveVelocity, float moveSpeed, float moveSpeedAir
        )
        {
            if (isGrounded)
            {
                // 地面にいるとき

                Vector3 slopeMoveDir = Vector3.ProjectOnPlane(moveDir, groundNormal).normalized;

                moveVelocity = slopeMoveDir * moveSpeed;

                Vector3 stickVelocity = -groundNormal * 1f;

                rb.linearVelocity = moveVelocity + stickVelocity;
            }
            else
            {
                // 地面にいないとき

                rb.linearVelocity = new Vector3(
                    rb.linearVelocity.x,
                    nextVY,
                    rb.linearVelocity.z
                ) + (moveDir * moveSpeedAir);
            }
        }

        /// <summary>
        /// 移動処理の停止プロセス
        /// </summary>
        void StopInMoveProcces(
            float nextVY, bool isGrounded, Vector3 groundNormal,
            ref Vector3 moveVelocity
        )
        {
            if (isGrounded)
            {
                Vector3 stickVelocity = -groundNormal * 1f;

                moveVelocity = Vector3.zero;

                // こうすることで、上り坂で止まった時に跳ねないようになる
                rb.linearVelocity = moveVelocity + stickVelocity;
            }
            else
            {
                rb.linearVelocity = new Vector3(
                    rb.linearVelocity.x,
                    nextVY,
                    rb.linearVelocity.z
                );
            }
        }

        /// <summary>
        /// 移動処理の回転プロセス
        /// </summary>
        void RotationInMoveProcces(Vector3 moveDir, bool isGrounded, float rotationSpeed)
        {
            if (isGrounded)
            {
                // 移動方向を向く回転
                Quaternion targetRotation = Quaternion.LookRotation(moveDir);

                // 現在の向きからスムーズに補間
                rb.MoveRotation(
                    Quaternion.Slerp(
                        rb.rotation,
                        targetRotation,
                        rotationSpeed * Time.fixedDeltaTime
                    )
                );
            }
        }

    }
}