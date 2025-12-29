using UnityEngine;
using UnityEngine.InputSystem; // ★ 新Input System

public class InputController : MonoBehaviour
{
    // Input System 用の変数
    Vector2 moveInput;   // WASD / 左スティック
    bool jumpPressed;   // ジャンプ入力

    public RigidbodyCharacterController ch;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (jumpPressed)
        {
            ch.Jump();

            jumpPressed = false;
        }
    }

    void FixedUpdate()
    {
        ch.Move(moveInput);
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
}
