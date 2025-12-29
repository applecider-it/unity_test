using UnityEngine;
using UnityEngine.InputSystem; // ★ 新Input System

public class RigidbodyCharacterController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;

    Rigidbody rb;
    bool isGrounded;

    // Input System 用の変数
    Vector2 moveInput;   // WASD / 左スティック
    bool jumpPressed;   // ジャンプ入力

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        CheckGround();

        // ジャンプ入力があって、地面にいるならジャンプ
        if (jumpPressed && isGrounded)
        {
            Jump();
            jumpPressed = false; // 1回分消費
        }
    }

    void FixedUpdate()
    {
        Move();
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
            
            Debug.Log("OnJump isPressed");

        }

        Debug.Log($"OnJump {isGrounded}, {jumpPressed}");
    }

    void Move()
    {
        // 2D入力を3D空間に変換
        Vector3 moveDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        Vector3 targetVelocity = moveDir * moveSpeed;
        Vector3 currentVelocity = rb.linearVelocity;

        Vector3 velocityChange =
            targetVelocity - new Vector3(currentVelocity.x, 0f, currentVelocity.z);

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void CheckGround()
    {
        float space = 0.1f;
        isGrounded = Physics.Raycast(
            transform.position + (Vector3.up * space),
            Vector3.down,
            groundCheckDistance + space + 0.01f,
            groundLayer
        );

        Debug.Log($"isGrounded {isGrounded}");
    }
}
