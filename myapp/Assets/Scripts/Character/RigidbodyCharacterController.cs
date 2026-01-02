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
        private RigidbodyCharacterControllerJump jumpCtrl;
        private RigidbodyCharacterControllerAnimation animCtrl;
        private RigidbodyCharacterControllerGround groundCtrl;

        private Rigidbody rb;

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

        /// <summary> 地面にいるときはtrue </summary>
        private bool isGrounded => (groundCtrl.GroundContactCount > 0 && jumpCnt <= 0);
        /// <summary> 地面の法線ベクトル </summary>
        private Vector3 groundNormal => isGrounded ? groundCtrl.GroundContactCountNormal : Vector3.up;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            Animator animator = GetComponent<Animator>();

            moveCtrl = new RigidbodyCharacterControllerMove(rb);
            jumpCtrl = new RigidbodyCharacterControllerJump(rb);
            animCtrl = new RigidbodyCharacterControllerAnimation(animator);
            groundCtrl = new RigidbodyCharacterControllerGround();
        }

        void FixedUpdate()
        {
            bool noMove = moveInput.sqrMagnitude < 0.01f;

            moveCtrl.MoveProccess(
                noMove, gravity, moveInput, isGrounded, groundNormal,
                ref moveVelocity, moveSpeed, moveSpeedAir, rotationSpeed
            );

            if (jumpCnt > 0) jumpCnt--;
            if (movingPlatformDeltaPosCnt > 0) movingPlatformDeltaPosCnt--;

            jumpCtrl.JumpProccess(
                ref jump, ref jumpCnt, movingPlatformDeltaPosCnt,
                isGrounded, moveVelocity, jumpForce, movingPlatformDeltaPos
            );

            animCtrl.SetAnimator(noMove, isGrounded);

            Debug.Log("isGrounded " + isGrounded + ", groundNormal " + groundNormal);
        }

        /// <summary>
        /// 接触開始時
        /// </summary>
        void OnCollisionEnter(Collision collision)
        {
            groundCtrl.OnCollisionEnter(collision, groundLayer, maxSlopeAngle);
        }

        /// <summary>
        /// 接触継続時
        /// </summary>
        void OnCollisionStay(Collision collision)
        {
            groundCtrl.OnCollisionStay(collision, groundLayer, maxSlopeAngle);
        }

        /// <summary>
        /// 接触終了時
        /// </summary>
        void OnCollisionExit(Collision collision)
        {
            groundCtrl.OnCollisionExit(collision, groundLayer);
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