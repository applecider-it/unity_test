using System.Diagnostics;
using UnityEngine;

public class RigidbodyCharacterController : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float groundCheckDistance = 0.1f;
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float gravity = 0.2f;

    [Header("Ground")]
    [SerializeField] LayerMask groundLayer;

    // private

    Vector2 moveInput;  // 移動方向

    bool jump;  // ジャンプフラグ

    Rigidbody rb;
    bool isGrounded;
    Vector3 groundNormal = Vector3.up;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        CheckGround();
        Move();
        Jump();
    }

    void Move()
    {
        // 入力なし → 即停止
        if (moveInput.sqrMagnitude < 0.01f)
        {
            rb.linearVelocity = new Vector3(0f, (isGrounded ? 0f : rb.linearVelocity.y - gravity), 0f);
            return;
        }

        // 入力あり
        Vector3 moveDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        // ===== 移動処理 =====

        Vector3 slopeMoveDir = Vector3.ProjectOnPlane(moveDir, groundNormal).normalized;

        Vector3 targetVelocity = slopeMoveDir * moveSpeed;

        rb.linearVelocity = new Vector3(
            targetVelocity.x,
            (isGrounded ? targetVelocity.y : rb.linearVelocity.y - gravity),
            targetVelocity.z
        );

        // ===== 回転処理 =====

        // 移動方向を向く回転
        Quaternion targetRotation = Quaternion.LookRotation(moveDir);

        // 現在の向きからスムーズに補間
        rb.MoveRotation(
            Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            )
        );
    }

    void Jump()
    {
        if (jump)
        {
            if (isGrounded)
            {
                rb.linearVelocity = new Vector3(
                    rb.linearVelocity.x,
                    jumpForce,
                    rb.linearVelocity.z
                );
            }
            jump = false;
        }
    }

    void CheckGround()
    {
        float space = 0.1f;
        RaycastHit hit;
        isGrounded = Physics.Raycast(
            transform.position + (Vector3.up * space),
            Vector3.down,
            out hit,
            groundCheckDistance + space + 0.01f,
            groundLayer
        );

        groundNormal = isGrounded ? hit.normal : Vector3.up;

        //Debug.Log($"isGrounded {isGrounded}");
    }

    // setter
    public void SetMoveInput(Vector2 value)
    {
        moveInput = value;
    }
    public void SetJump(bool value)
    {
        jump = value;
    }
}
