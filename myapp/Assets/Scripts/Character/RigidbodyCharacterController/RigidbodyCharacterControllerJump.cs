using UnityEngine;

namespace Game.Character.RigidbodyCharacterControllerParts
{
    public class RigidbodyCharacterControllerJump
    {
        private Rigidbody rb;

        // コンストラクタ
        public RigidbodyCharacterControllerJump(Rigidbody argRb)
        {
            rb = argRb;
        }

        /// <summary>
        /// ジャンプ処理
        /// </summary>
        public void JumpProccess(
            ref bool jump, ref int jumpCnt, int movingPlatformDeltaPosCnt,
            bool isGrounded, Vector3 moveVelocity, float jumpForce, Vector3 movingPlatformDeltaPos
        )
        {
            if (jump)
            {
                if (isGrounded)
                {
                    // 地面にいるとき

                    ExecJump(
                        movingPlatformDeltaPosCnt, moveVelocity, jumpForce,
                        movingPlatformDeltaPos
                    );

                    jumpCnt = 5;
                }

                jump = false;
            }
        }

        /// <summary>
        /// ジャンプ実行
        /// </summary>
        void ExecJump(
            int movingPlatformDeltaPosCnt, Vector3 moveVelocity, float jumpForce,
            Vector3 movingPlatformDeltaPos
        )
        {
            Vector3 velocity = new Vector3(
                moveVelocity.x,
                jumpForce,
                moveVelocity.z
            );

            // 動く床の影響を足す
            if (movingPlatformDeltaPosCnt > 0)
            {
                velocity += movingPlatformDeltaPos * 50f;
            }

            rb.linearVelocity = velocity;
        }
    }
}