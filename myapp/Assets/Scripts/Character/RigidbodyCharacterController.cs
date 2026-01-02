using UnityEngine;

using Game.Character.RigidbodyCharacterControllerParts;

namespace Game.Character
{
    public class RigidbodyCharacterController : MonoBehaviour
    {
        [Header("Main")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float moveSpeedAir = 0.05f;
        [SerializeField] private float jumpForce = 6f;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float gravity = 0.2f;
        [SerializeField] private float maxSlopeAngle = 40f;

        [Header("Ground")]
        [Tooltip("地面と判断するためのマスク")][SerializeField] private LayerMask groundLayer;

        // private

        private RigidbodyCharacterControllerMove moveCtrl;

        private Rigidbody rb;
        private Animator animator;

        /// <summary> 移動方向 </summary>
        private Vector2 moveInput;
        /// <summary> ジャンプフラグ </summary>
        private bool jump;
        /// <summary> ジャンプ直後カウント </summary>
        private int jumpCnt = 0;

        /// <summary> 移動床の移動量 </summary>
        private Vector3 movingPlatformDeltaPos = Vector3.zero;
        /// <summary> 移動床の移動量有効カウント </summary>
        private int movingPlatformDeltaPosCnt = 0;

        /// <summary> 地面にいるときの移動ベクトル。ジャンプ時の補正に使う。 </summary>
        private Vector3 moveVelocity = Vector3.zero;

        /// <summary> 地面接触カウント </summary>
        private int groundContactCount = 0;
        /// <summary> 地面接触法線ベクトル </summary>
        private Vector3 groundContactCountNormal = Vector3.zero;
        /// <summary> 地面接触コライダー </summary>
        private Collider groundContactCollider = null;

        /// <summary> 地面にいるときはtrue </summary>
        private bool isGrounded => (groundContactCount > 0 && jumpCnt <= 0);
        /// <summary> 地面の法線ベクトル </summary>
        private Vector3 groundNormal => isGrounded ? groundContactCountNormal : Vector3.up;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();

            moveCtrl = new RigidbodyCharacterControllerMove(rb);
        }

        void FixedUpdate()
        {
            bool noMove = moveInput.sqrMagnitude < 0.01f;

            moveCtrl.MoveProccess(
                noMove, gravity, moveInput, isGrounded, groundNormal,
                ref moveVelocity, moveSpeed, moveSpeedAir, rotationSpeed
            );
            JumpProccess();

            SetAnimator(noMove);

            Debug.Log("isGrounded " + isGrounded + ", groundNormal " + groundNormal);
        }

        /// <summary>
        /// ジャンプ処理
        /// </summary>
        void JumpProccess()
        {
            if (jumpCnt > 0) jumpCnt--;
            if (movingPlatformDeltaPosCnt > 0) movingPlatformDeltaPosCnt--;

            if (jump)
            {
                if (isGrounded)
                {
                    // 地面にいるとき

                    ExecJump();

                    jumpCnt = 5;
                }

                jump = false;
            }
        }

        /// <summary>
        /// ジャンプ実行
        /// </summary>
        void ExecJump()
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

        /// <summary>
        /// 接触開始時
        /// </summary>
        void OnCollisionEnter(Collision collision)
        {
            if (!IsGroundLayer(collision.gameObject)) return;

            if (IsGroundContact(collision))
            {
                groundContactCollider = collision.collider;
                groundContactCount++;
            }
        }

        /// <summary>
        /// 接触継続時
        /// </summary>
        void OnCollisionStay(Collision collision)
        {
            if (!IsGroundLayer(collision.gameObject)) return;

            // 接触した時の地面以外は対象外
            if (groundContactCollider != collision.collider) return;

            if (IsGroundContact(collision))
            {
                groundContactCount = Mathf.Max(groundContactCount, 1);
            }
            else
            {
                groundContactCount = Mathf.Max(groundContactCount - 1, 0);
            }
        }

        /// <summary>
        /// 接触終了時
        /// </summary>
        void OnCollisionExit(Collision collision)
        {
            if (!IsGroundLayer(collision.gameObject)) return;

            groundContactCount = Mathf.Max(groundContactCount - 1, 0);
        }

        /// <summary>
        /// 接触点が「足元付近」かチェック
        /// </summary>
        bool IsGroundContact(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                Debug.Log("contact.normal " + contact.normal);
                // 上向きの面か？（坂も考慮）
                if (Vector3.Angle(contact.normal, Vector3.up) <= maxSlopeAngle)
                {
                    Debug.Log("contact valid");
                    groundContactCountNormal = contact.normal;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 地面レイヤーかチェック
        /// </summary>
        bool IsGroundLayer(GameObject obj)
        {
            return (groundLayer.value & (1 << obj.layer)) != 0;
        }

        /// <summary>
        /// アニメーター指定
        /// </summary>
        void SetAnimator(bool noMove)
        {
            if (noMove)
            {
                // 入力なしの時

                animator.SetBool("move", false);
            }
            else
            {
                // 入力ありの時

                animator.SetBool("move", isGrounded);
            }
        }

        // setter

        public Vector2 MoveInput { set => moveInput = value; }
        public bool Jump { set => jump = value; }
        public Vector3 MovingPlatformDeltaPos
        {
            set
            {
                movingPlatformDeltaPos = value;
                movingPlatformDeltaPosCnt = 5;
            }
        }
    }
}