using UnityEngine;
using UnityEngine.InputSystem; // ★ 新Input System

using Game.Characters;

namespace Game.Stages
{
    public class InputController : MonoBehaviour
    {
        // Input System 用の変数
        Vector2 moveInput;   // WASD / 左スティック
        bool jumpPressed;   // ジャンプ入力
        bool attackPressed;   // アタック入力

        RigidbodyCharacterController ch;
        Transform cameraTransform;

        float lookTimer;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            // 開始直後の入力暴れ対策
            CommonData cd = CommonData.getCommonData();
            lookTimer = cd.LookIgnoreTime;
            ch = cd.Player.GetComponent<RigidbodyCharacterController>();
            cameraTransform = cd.Camera.transform;
        }

        // Update is called once per frame
        void Update()
        {
        }

        void FixedUpdate()
        {
            // 少しの間更新をしない
            if (lookTimer > 0f)
            {
                lookTimer -= Time.deltaTime;
                return;
            }

            // カメラ方向変換後（XZ）
            Vector2 moveAxis = Vector2.zero;
            Vector2 cursorAxis = Vector2.zero;
            
            if (moveInput.sqrMagnitude > 0.01f)
            {
                moveAxis = ConvertInputToMoveAxis();
                cursorAxis = moveInput.normalized;
            }

            ch.MoveInput = moveAxis;
            ch.CursorInput = cursorAxis;

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

        Vector2 ConvertInputToMoveAxis()
        {
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
            Vector2 moveAxis = new Vector2(worldDir.x, worldDir.z);

            moveAxis.Normalize();

            return moveAxis;
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