using UnityEngine;

public class RigidbodyCharacterController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;

    [SerializeField] float rotationSpeed = 10f;

    Rigidbody rb;
    bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        CheckGround();
    }

    public void Move(Vector2 moveInput)
    {
        // 入力なし → 即停止
        if (moveInput.sqrMagnitude < 0.01f)
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            return;
        }

        // 入力あり
        Vector3 moveDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        // ===== 移動処理 =====
        
        Vector3 targetVelocity = moveDir * moveSpeed;

        rb.linearVelocity = new Vector3(
            targetVelocity.x,
            rb.linearVelocity.y,
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

    public void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
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

        //Debug.Log($"isGrounded {isGrounded}");
    }
}
