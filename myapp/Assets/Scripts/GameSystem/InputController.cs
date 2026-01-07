using UnityEngine;
using UnityEngine.InputSystem; // ★ 新Input System

using Game.Character;

namespace Game.GameSystem
{
    public class InputController : MonoBehaviour
    {
        // Input System 用の変数
        Vector2 moveInput;   // WASD / 左スティック
        bool jumpPressed;   // ジャンプ入力
        bool attackPressed;   // アタック入力

        Vector2 moveAxis; // ← カメラ方向変換後（XZ）

        [SerializeField] Transform cameraTransform;
        public RigidbodyCharacterController ch;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        void FixedUpdate()
        {
            ConvertInputToMoveAxis();
            ch.MoveInput = moveAxis;

            if (jumpPressed)
            {
                ch.Jump = true;

                jumpPressed = false;
            }

            if (attackPressed)
            {
                ch.Attack = true;

                attackPressed = false;
            }
        }

        void ConvertInputToMoveAxis()
        {
            if (moveInput.sqrMagnitude < 0.01f)
            {
                moveAxis = Vector2.zero;
                return;
            }

            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;

            camForward.y = 0f;
            camRight.y = 0f;

            camForward.Normalize();
            camRight.Normalize();

            Vector3 worldDir =
                camForward * moveInput.y +
                camRight * moveInput.x;

            worldDir.Normalize();

            // XZ平面に落とす
            moveAxis = new Vector2(worldDir.x, worldDir.z);

            moveAxis.Normalize();
        }

        // ===== Input System から呼ばれる関数 =====

        // 移動入力（PlayerInput から自動で呼ばれる）
        public void OnMove(InputValue value)
        {
            moveInput = value.Get<Vector2>();
        }

        // ジャンプ入力
        public void OnJump(InputValue value)
        {
            if (value.isPressed)
            {
                jumpPressed = true;

                //Debug.Log("OnJump isPressed");

            }

            //Debug.Log($"OnJump {isGrounded}, {jumpPressed}");
        }

        // あった句入力
        public void OnAttack(InputValue value)
        {
            if (value.isPressed)
            {
                attackPressed = true;

                //Debug.Log("OnAttack isPressed");
            }
        }
    }
}