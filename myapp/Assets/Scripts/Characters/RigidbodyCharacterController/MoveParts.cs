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

        /// <summary> 地面にいるときの移動ベクトル。ジャンプ時の補正に使う。 </summary>
        private Vector3 moveVelocity = Vector3.zero;

        // コンストラクタ
        public MoveParts(Rigidbody argRb, Transform tr)
        {
            rb = argRb;
            transform = tr;
        }

        /// <summary>
        /// 移動処理
        /// </summary>
        public void MoveProcces(
            Vector3 moveDir, Vector2 cursorInput, float gravity, bool isGrounded, Vector3 groundNormal,
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

        // setter getter

        public Vector3 MoveVelocity { get => moveVelocity; }
    }
}