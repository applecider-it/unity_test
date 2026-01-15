using UnityEngine;

namespace Game.Characters.RigidbodyCharacterControllerParts
{
    /// <summary>
    /// 移動処理
    /// </summary>
    public class MoveParts
    {
        private Rigidbody rb;
        private Transform transform;

        /// <summary> 移動方向 </summary>
        private Vector2 moveInput;
        /// <summary> カーソル方向 </summary>
        private Vector2 cursorInput;

        /// <summary> 地面にいるときの移動ベクトル。ジャンプ時の補正に使う。 </summary>
        private Vector3 moveVelocity = Vector3.zero;

        // コンストラクタ
        public MoveParts(Rigidbody argRb, Transform tr)
        {
            rb = argRb;
            transform = tr;
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
            bool inWater, bool inWaterBuoyancy, float buoyancy, float moveSpeedWater, float waterFriction, float rotationSpeedWater,
            bool isHang, Vector3 hangNormal
        )
        {
            Vector3 moveDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

            MoveInMoveProcces(
                moveDir, gravity, isGrounded, groundNormal,
                moveSpeed, moveSpeedAir, noMove,
                inWaterBuoyancy, buoyancy, moveSpeedWater, waterFriction,
                isHang, hangNormal
            );

            RotationInMoveProcces(
                moveDir, isGrounded, rotationSpeed,
                inWater, rotationSpeedWater,
                isHang, hangNormal, noMove
            );
        }

        /// <summary>
        /// 移動処理の移動プロセス
        /// </summary>
        void MoveInMoveProcces(
            Vector3 moveDir, float gravity, bool isGrounded, Vector3 groundNormal,
            float moveSpeed, float moveSpeedAir, bool noMove,
            bool inWaterBuoyancy, float buoyancy, float moveSpeedWater, float waterFriction,
            bool isHang, Vector3 hangNormal
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

                    if (isHang)
                    {
                        // つかまっているとき

                        moveVelocity = Vector3.zero;

                        Vector3 stickVelocity = -hangNormal * 1f;

                        if (noMove)
                        {
                            // 入力がないとき

                            rb.linearVelocity = stickVelocity;
                        }
                        else
                        {
                            // 入力があるとき

                            Debug.Log("cursorInput: " + cursorInput);

                            Vector3 move;

                            Vector3 right = transform.right;   // キャラの右
                            Vector3 up = Vector3.up;            // はしごは常にY軸

                            move = right * cursorInput.x + up * cursorInput.y;

                            rb.linearVelocity = move * moveSpeed + stickVelocity;
                        }
                    }
                    else
                    {
                        // つかまっていないとき

                        rb.linearVelocity = new Vector3(
                            rb.linearVelocity.x,
                            rb.linearVelocity.y - gravity,
                            rb.linearVelocity.z
                        ) + (moveDir * moveSpeedAir);
                    }
                }
            }
        }

        /// <summary>
        /// 移動処理の回転プロセス
        /// </summary>
        void RotationInMoveProcces(
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

        // setter getter

        public Vector2 MoveInput { set => moveInput = value; }
        public Vector2 CursorInput { set => cursorInput = value; }
        public Vector3 MoveVelocity { get => moveVelocity; }
    }
}