using UnityEngine;

namespace Game.Character.RigidbodyCharacterControllerParts
{
    /// <summary>
    /// ジャンプ処理
    /// </summary>
    public class JumpParts
    {
        private Rigidbody rb;

        /// <summary> ジャンプフラグ </summary>
        private bool jump;
        /// <summary> ジャンプ直後カウント </summary>
        private int jumpCnt = 0;

        // コンストラクタ
        public JumpParts(Rigidbody argRb)
        {
            rb = argRb;
        }

        /// <summary>
        /// ジャンプ処理
        /// </summary>
        public void JumpProccess(
            Vector3 movingPlatformDelta,
            bool isGrounded, Vector3 moveVelocity, float jumpForce
        )
        {
            if (jumpCnt > 0) jumpCnt--;

            if (jump)
            {
                if (isGrounded)
                {
                    // 地面にいるとき

                    ExecJump(
                        moveVelocity, jumpForce,
                        movingPlatformDelta
                    );

                    jumpCnt = 5;
                }

                jump = false;
            }
        }

        /// <summary>
        /// ジャンプ実行
        /// </summary>
        void ExecJump(Vector3 moveVelocity, float jumpForce, Vector3 movingPlatformDelta)
        {
            Vector3 velocity = new Vector3(
                moveVelocity.x,
                jumpForce,
                moveVelocity.z
            );

            // 動く床の影響を足す
            velocity += movingPlatformDelta * 50f;

            rb.linearVelocity = velocity;
        }

        // getter setter

        public bool Jump { set => jump = value; }
        public bool JumpWait { get => jumpCnt > 0;}
    }
}