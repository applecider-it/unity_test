using UnityEngine;

using Game.Characters.RigidbodyCharacterControllerParts;

using Game.Systems;

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
        [SerializeField] private float moveSpeedWater = 0.08f;
        [SerializeField] private float jumpForce = 6f;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float rotationSpeedWater = 1f;

        [Header("Physics")]
        [Tooltip("重力")][SerializeField] private float gravity = 0.2f;
        [Tooltip("浮力")][SerializeField] private float buoyancy = 0.1f;
        [Tooltip("水の摩擦")][SerializeField] private float waterFriction = 0.98f;

        [Header("Ground")]
        [Tooltip("地面と認識する最大角度")][SerializeField] private float maxSlopeAngle = 40f;

        [Header("Audio")]
        [SerializeField] private AudioClipContainer attackAudio;
        [SerializeField] private AudioClipContainer jumpAudio;

        // private

        private MoveParts moveCtrl;
        private RotationParts rotationCtrl;
        private JumpParts jumpCtrl;
        private AnimationParts animCtrl;
        private GroundParts groundCtrl;
        private AttackParts attackCtrl;
        private WaterParts waterCtrl;
        private HangParts hangCtrl;

        /// <summary> 移動床の移動量 </summary>
        private Vector3 movingPlatformDeltaPos = Vector3.zero;
        /// <summary> 移動床の移動量有効カウント </summary>
        private int movingPlatformDeltaPosCnt = 0;

        Collider myCol;

        /// <summary> 移動方向 </summary>
        private Vector2 moveInput;
        /// <summary> カーソル方向 </summary>
        private Vector2 cursorInput;

        void Awake()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            Animator animator = GetComponent<Animator>();
            AudioSource audioSource = GetComponent<AudioSource>();

            jumpAudio.TargetAudioSource = audioSource;
            attackAudio.TargetAudioSource = audioSource;

            moveCtrl = new MoveParts(rb, transform);
            rotationCtrl = new RotationParts(rb);
            jumpCtrl = new JumpParts(rb, jumpAudio);
            animCtrl = new AnimationParts(animator);
            groundCtrl = new GroundParts();
            attackCtrl = new AttackParts(transform, attackAudio);
            waterCtrl = new WaterParts();
            hangCtrl = new HangParts();

            groundCtrl.Awake();
            attackCtrl.Awake();

            myCol = GetComponent<Collider>();
        }

        void FixedUpdate()
        {
            groundCtrl.CleanupDestroyedGround();

            // 地面にいるときはtrue
            bool isGrounded = (groundCtrl.IsGrounded && !jumpCtrl.JumpWait);
            // 地面の法線ベクトル
            Vector3 groundNormal = isGrounded ? groundCtrl.GroundContactNormal : Vector3.up;

            bool noMove = NoMove();
            bool inWater = waterCtrl.InsideCheck();
            bool inWaterBuoyancy = waterCtrl.PointCheck(myCol.bounds.center);
            bool isHang = hangCtrl.IsHang();
            Vector3 hangNormal = hangCtrl.Normal;
            Vector3 moveDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

            if (movingPlatformDeltaPosCnt > 0) movingPlatformDeltaPosCnt--;

            Vector3 movingPlatformDelta = (movingPlatformDeltaPosCnt > 0) ? movingPlatformDeltaPos : Vector3.zero;

            moveCtrl.MoveProcces(
                moveDir, cursorInput, gravity, isGrounded, groundNormal,
                moveSpeed, moveSpeedAir, noMove,
                inWaterBuoyancy, buoyancy, moveSpeedWater, waterFriction,
                isHang, hangNormal
            );

            rotationCtrl.RotationProcces(
                moveDir, isGrounded, rotationSpeed,
                inWater, rotationSpeedWater,
                isHang, hangNormal, noMove
            );

            jumpCtrl.JumpProccess(
                movingPlatformDelta,
                isGrounded, inWaterBuoyancy, moveCtrl.MoveVelocity, jumpForce
            );

            animCtrl.SetAnimator(noMove, isGrounded, inWater);

            attackCtrl.AttackProccess(this);

            //Debug.Log("isGrounded " + isGrounded + ", groundNormal " + groundNormal);
            //Debug.Log("isHang " + isHang);
        }

        /// <summary>
        /// 接触開始時
        /// </summary>
        void OnCollisionEnter(Collision collision)
        {
            groundCtrl.OnCollisionEnter(collision, maxSlopeAngle);
            hangCtrl.OnCollisionEnter(collision);
        }

        /// <summary>
        /// 接触継続時
        /// </summary>
        void OnCollisionStay(Collision collision)
        {
            groundCtrl.OnCollisionStay(collision, maxSlopeAngle);
            hangCtrl.OnCollisionStay(collision);
        }

        /// <summary>
        /// 接触終了時
        /// </summary>
        void OnCollisionExit(Collision collision)
        {
            groundCtrl.OnCollisionExit(collision);
            hangCtrl.OnCollisionExit(collision);
        }

        /// <summary>
        /// トリガー開始時
        /// </summary>
        void OnTriggerEnter(Collider other)
        {
            waterCtrl.OnTriggerEnter(other);
        }

        /// <summary>
        /// トリガー終了時
        /// </summary>
        void OnTriggerExit(Collider other)
        {
            waterCtrl.OnTriggerExit(other);
        }

        /// <summary>
        /// 方向キーが有効か返す
        /// </summary>
        public bool NoMove()
        {
            return moveInput.sqrMagnitude < 0.01f;
        }

        // setter

        public Vector2 MoveInput { set => moveInput = value; }
        public Vector2 CursorInput { set => cursorInput = value; }
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