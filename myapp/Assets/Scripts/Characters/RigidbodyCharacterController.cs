using UnityEngine;

using Game.Characters.RigidbodyCharacterControllerParts;

using Game.Systems;

namespace Game.Characters
{
    public enum CharacterActionType
    {
        Undefined = 0,
        Water = 1,
        Ground = 2,
        Hang = 3,
        Air = 4
    }

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
        private MovingPlatformParts movingPlatformCtrl;

        Collider myCol;

        /// <summary> 移動方向 </summary>
        private Vector2 moveInput;
        /// <summary> カーソル方向 </summary>
        private Vector2 cursorInput;

        // 外部に情報提供する用の変数

        private CharacterActionType outerActionType = CharacterActionType.Undefined;

        void OnEnable()
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
            waterCtrl = new WaterParts(name);
            hangCtrl = new HangParts();
            movingPlatformCtrl = new MovingPlatformParts();

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
            bool isHang = hangCtrl.IsHang(maxSlopeAngle) && !jumpCtrl.JumpWait;
            Vector3 hangNormal = hangCtrl.Normal;
            Vector3 moveDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

            CharacterActionType actionType = GetActionType(isGrounded, inWaterBuoyancy, isHang);

            outerActionType = actionType;

            movingPlatformCtrl.MovingPlatformProccess();

            moveCtrl.MoveProcces(
                moveDir, cursorInput, gravity, groundNormal,
                moveSpeed, moveSpeedAir, noMove,
                buoyancy, moveSpeedWater, waterFriction,
                hangNormal,
                actionType
            );

            rotationCtrl.RotationProcces(
                moveDir, rotationSpeed, rotationSpeedWater,
                hangNormal, noMove,
                actionType
            );

            jumpCtrl.JumpProccess(
                movingPlatformCtrl.MovingPlatformDelta,
                moveCtrl.MoveVelocity, jumpForce,
                actionType
            );

            animCtrl.SetAnimator(noMove, actionType);

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

        /// <summary>
        /// アクションタイプ取得
        /// </summary>
        public CharacterActionType GetActionType(bool isGrounded, bool inWaterBuoyancy, bool isHang)
        {
            CharacterActionType type = CharacterActionType.Undefined;

            if (inWaterBuoyancy)
            {
                // 水中にいるとき

                type = CharacterActionType.Water;
            }
            else
            {
                // 水中にいないとき

                if (isGrounded)
                {
                    // 地面にいるとき

                    type = CharacterActionType.Ground;
                }
                else
                {
                    // 地面にいないとき

                    if (isHang)
                    {
                        // つかまっているとき

                        type = CharacterActionType.Hang;
                    }
                    else
                    {
                        // つかまっていないとき

                        type = CharacterActionType.Air;
                    }
                }
            }

            return type;
        }

        // setter getter

        public Vector2 MoveInput { set => moveInput = value; }
        public Vector2 CursorInput { set => cursorInput = value; }
        public bool Jump { set => jumpCtrl.Jump = value; }
        public bool Attack { set => attackCtrl.Attack = value; }
        public Vector3 MovingPlatformDeltaPos { set => movingPlatformCtrl.MovingPlatformDeltaPos = value; }

        public CharacterActionType ActionType { get => outerActionType; }
    }
}