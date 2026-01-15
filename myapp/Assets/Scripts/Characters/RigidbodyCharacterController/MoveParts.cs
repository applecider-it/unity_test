using UnityEngine;

namespace Game.Characters.RigidbodyCharacterControllerParts
{
    /// <summary>
    /// 移動処理
    /// </summary>
    public class MoveParts
    {
        private Rigidbody rb;

        /// <summary> 移動方向 </summary>
        private Vector2 moveInput;

        /// <summary> 地面にいるときの移動ベクトル。ジャンプ時の補正に使う。 </summary>
        private Vector3 moveVelocity = Vector3.zero;

        // コンストラクタ
        public MoveParts(Rigidbody argRb)
        {
            rb = argRb;
        }

        /// <summary>
        /// 方向キーが有効か返す
        /// </summary>
        public bool NoMove()
        {
            return moveInput.sqrMagnitude < 0.01f;
        }

        /// <summary>
        /// 移動処理
        /// </summary>
        public void MoveProccess(
            bool noMove, float gravity, bool isGrounded, Vector3 groundNormal,
            float moveSpeed, float moveSpeedAir, float rotationSpeed,
            bool inWater, bool inWaterBuoyancy, float buoyancy, float moveSpeedWater, float waterFriction, float rotationSpeedWater
        )
        {
            Vector3 moveDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

            MoveInMoveProcces(
                moveDir, gravity, isGrounded, groundNormal,
                moveSpeed, moveSpeedAir, noMove,
                inWaterBuoyancy, buoyancy, moveSpeedWater, waterFriction
            );

            if (!noMove)
            {
                RotationInMoveProcces(moveDir, isGrounded, rotationSpeed, inWater, rotationSpeedWater);
            }
        }

        /// <summary>
        /// 移動処理の移動プロセス
        /// </summary>
        void MoveInMoveProcces(
            Vector3 moveDir, float gravity, bool isGrounded, Vector3 groundNormal,
            float moveSpeed, float moveSpeedAir, bool noMove,
            bool inWaterBuoyancy, float buoyancy, float moveSpeedWater, float waterFriction
        )
        {
            if (inWaterBuoyancy)
            {
                // 水中にいるとき

                rb.linearVelocity = (new Vector3(
                    rb.linearVelocity.x,
                    rb.linearVelocity.y + buoyancy,
                    rb.linearVelocity.z
                ) + (moveDir * moveSpeedWater)) * waterFriction;
            }
            else
            {
                // 水中にいないとき

                if (isGrounded)
                {
                    // 地面にいるとき

                    if (noMove)
                    {
                        // 入力がないとき

                        moveVelocity = Vector3.zero;
                    }
                    else
                    {
                        // 入力があるとき

                        Vector3 slopeMoveDir = Vector3.ProjectOnPlane(moveDir, groundNormal).normalized;

                        moveVelocity = slopeMoveDir * moveSpeed;
                    }

                    Vector3 stickVelocity = -groundNormal * 1f;

                    // こうすることで、上り坂で止まった時に跳ねないようになる
                    rb.linearVelocity = moveVelocity + stickVelocity;
                }
                else
                {
                    // 地面にいないとき

                    rb.linearVelocity = new Vector3(
                        rb.linearVelocity.x,
                        rb.linearVelocity.y - gravity,
                        rb.linearVelocity.z
                    ) + (moveDir * moveSpeedAir);
                }
            }
        }

        /// <summary>
        /// 移動処理の回転プロセス
        /// </summary>
        void RotationInMoveProcces(
            Vector3 moveDir, bool isGrounded, float rotationSpeed,
            bool inWater, float rotationSpeedWater
        )
        {
            if (isGrounded || inWater)
            {
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

        // setter getter

        public Vector2 MoveInput { set => moveInput = value; }
        public Vector3 MoveVelocity { get => moveVelocity; }
    }
}