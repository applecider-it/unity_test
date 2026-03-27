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
            Vector3 moveDir, Vector2 cursorInput, float gravity, Vector3 groundNormal,
            float moveSpeed, float moveSpeedAir, bool noMove,
            float buoyancy, float moveSpeedWater, float waterFriction,
            Vector3 hangNormal,
            CharacterActionType actionType
        )
        {
            switch (actionType)
            {
                case CharacterActionType.Water:
                    WaterProcces(
                        moveDir, buoyancy, moveSpeedWater, waterFriction
                    );
                    break;

                case CharacterActionType.Ground:
                    GroundProcces(
                        moveDir, groundNormal, moveSpeed, noMove
                    );
                    break;

                case CharacterActionType.Hang:
                    HangProcces(
                        cursorInput, moveSpeed, noMove, hangNormal
                    );
                    break;

                case CharacterActionType.Air:
                    AirProcces(
                        moveDir, gravity, moveSpeedAir
                    );
                    break;
            }
        }

        /// <summary>
        /// 水中処理
        /// </summary>
        void WaterProcces(
            Vector3 moveDir, float buoyancy, float moveSpeedWater, float waterFriction
        )
        {
            rb.linearVelocity = (new Vector3(
                rb.linearVelocity.x,
                rb.linearVelocity.y + buoyancy,
                rb.linearVelocity.z
            ) + (moveDir * moveSpeedWater)) * waterFriction;
        }

        /// <summary>
        /// 地面処理
        /// </summary>
        public void GroundProcces(
            Vector3 moveDir, Vector3 groundNormal, float moveSpeed, bool noMove
        )
        {
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

        /// <summary>
        /// つかまり処理
        /// </summary>
        public void HangProcces(
            Vector2 cursorInput, float moveSpeed, bool noMove, Vector3 hangNormal
        )
        {
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

                //Debug.Log("cursorInput: " + cursorInput);

                Vector3 move;

                // 梯子の「上方向」は常にY
                Vector3 up = Vector3.up;

                Vector3 ladderNormal = -hangNormal;

                // 梯子の「横方向」を法線から計算
                Vector3 right = Vector3.Cross(up, ladderNormal).normalized;

                // 念のため、面上に投影（斜め梯子対策）
                right = Vector3.ProjectOnPlane(right, ladderNormal).normalized;
                up = Vector3.ProjectOnPlane(up, ladderNormal).normalized;

                // 入力から移動ベクトル生成
                move = right * cursorInput.x + up * cursorInput.y;

                // Rigidbody に反映
                rb.linearVelocity = move * moveSpeed + stickVelocity;

            }
        }

        /// <summary>
        /// 空中処理
        /// </summary>
        public void AirProcces(
            Vector3 moveDir, float gravity, float moveSpeedAir
        )
        {
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                rb.linearVelocity.y - gravity,
                rb.linearVelocity.z
            ) + (moveDir * moveSpeedAir);
        }

        // setter getter

        public Vector3 MoveVelocity { get => moveVelocity; }
    }
}