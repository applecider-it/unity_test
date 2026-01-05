using UnityEngine;

using Game.Character.RigidbodyCharacterControllerParts;
using Game.Util;
using Game.System;

namespace Game.Character
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

        private RigidbodyCharacterControllerMove moveCtrl;
        private RigidbodyCharacterControllerJump jumpCtrl;
        private RigidbodyCharacterControllerAnimation animCtrl;
        private RigidbodyCharacterControllerGround groundCtrl;

        /// <summary> 移動床の移動量 </summary>
        private Vector3 movingPlatformDeltaPos = Vector3.zero;
        /// <summary> 移動床の移動量有効カウント </summary>
        private int movingPlatformDeltaPosCnt = 0;

        /// <summary> 地面にいるときはtrue </summary>
        private bool isGrounded => (groundCtrl.IsGrounded && !jumpCtrl.JumpWait);
        /// <summary> 地面の法線ベクトル </summary>
        private Vector3 groundNormal => isGrounded ? groundCtrl.GroundContactNormal : Vector3.up;
        /// <summary> 地面と判断するためのマスク </summary>
        private LayerMask groundLayer;

        void Awake()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            Animator animator = GetComponent<Animator>();

            moveCtrl = new RigidbodyCharacterControllerMove(rb);
            jumpCtrl = new RigidbodyCharacterControllerJump(rb);
            animCtrl = new RigidbodyCharacterControllerAnimation(animator);
            groundCtrl = new RigidbodyCharacterControllerGround();

            CommonData cd = DataUtil.getCommonData();
            groundLayer = cd.GroundLayer;
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

            //Debug.Log("isGrounded " + isGrounded + ", groundNormal " + groundNormal);
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

        public Vector2 MoveInput { set => moveCtrl.MoveInput = value; }
        public bool Jump { set => jumpCtrl.Jump = value; }
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