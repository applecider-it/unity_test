using UnityEngine;

using Game.Characters.RigidbodyCharacterControllerParts;

namespace Game.Characters
{
    /// <summary>
    /// Rigidbodyキャラクター管理
    /// </summary>
    public class RigidbodyCharacterController : MonoBehaviour
    {
        [Header("Character")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float moveSpeedAir = 0.05f;
        [SerializeField] private float jumpForce = 6f;
        [SerializeField] private float rotationSpeed = 10f;

        [Header("Physics")]
        [SerializeField] private float gravity = 0.2f;

        [Header("Ground")]
        [Tooltip("地面と認識する最大角度")][SerializeField] private float maxSlopeAngle = 40f;

        // private

        private MoveParts moveCtrl;
        private JumpParts jumpCtrl;
        private AnimationParts animCtrl;
        private GroundParts groundCtrl;
        private AttackParts attackCtrl;

        /// <summary> 移動床の移動量 </summary>
        private Vector3 movingPlatformDeltaPos = Vector3.zero;
        /// <summary> 移動床の移動量有効カウント </summary>
        private int movingPlatformDeltaPosCnt = 0;

        /// <summary> 地面にいるときはtrue </summary>
        private bool isGrounded => (groundCtrl.IsGrounded && !jumpCtrl.JumpWait);
        /// <summary> 地面の法線ベクトル </summary>
        private Vector3 groundNormal => isGrounded ? groundCtrl.GroundContactNormal : Vector3.up;

        void Awake()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            Animator animator = GetComponent<Animator>();

            moveCtrl = new MoveParts(rb);
            jumpCtrl = new JumpParts(rb);
            animCtrl = new AnimationParts(animator);
            groundCtrl = new GroundParts();
            attackCtrl = new AttackParts(transform);

            groundCtrl.Awake();
            attackCtrl.Awake();
        }

        void FixedUpdate()
        {
            groundCtrl.CleanupDestroyedGround();

            bool noMove = moveCtrl.NoMove();

            if (movingPlatformDeltaPosCnt > 0) movingPlatformDeltaPosCnt--;

            Vector3 movingPlatformDelta = (movingPlatformDeltaPosCnt > 0) ? movingPlatformDeltaPos : Vector3.zero;

            moveCtrl.MoveProccess(
                noMove, gravity, isGrounded, groundNormal,
                moveSpeed, moveSpeedAir, rotationSpeed
            );

            jumpCtrl.JumpProccess(
                movingPlatformDelta,
                isGrounded, moveCtrl.MoveVelocity, jumpForce
            );

            animCtrl.SetAnimator(noMove, isGrounded);

            attackCtrl.AttackProccess(this);

            //Debug.Log("isGrounded " + isGrounded + ", groundNormal " + groundNormal);
        }

        /// <summary>
        /// 接触開始時
        /// </summary>
        void OnCollisionEnter(Collision collision)
        {
            groundCtrl.OnCollisionEnter(collision, maxSlopeAngle);
        }

        /// <summary>
        /// 接触継続時
        /// </summary>
        void OnCollisionStay(Collision collision)
        {
            groundCtrl.OnCollisionStay(collision, maxSlopeAngle);
        }

        /// <summary>
        /// 接触終了時
        /// </summary>
        void OnCollisionExit(Collision collision)
        {
            groundCtrl.OnCollisionExit(collision);
        }

        // setter

        public Vector2 MoveInput { set => moveCtrl.MoveInput = value; }
        public bool Jump { set => jumpCtrl.Jump = value; }
        public bool Attack { set => attackCtrl.Attack = value; }
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