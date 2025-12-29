using UnityEngine;

public class RigidbodyCharacterController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;

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
        // 2D入力を3D空間に変換
        Vector3 moveDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        Vector3 targetVelocity = moveDir * moveSpeed;
        Vector3 currentVelocity = rb.linearVelocity;

        Vector3 velocityChange =
            targetVelocity - new Vector3(currentVelocity.x, 0f, currentVelocity.z);

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
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
