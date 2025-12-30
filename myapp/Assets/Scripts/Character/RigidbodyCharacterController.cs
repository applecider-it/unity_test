using UnityEngine;

public class RigidbodyCharacterController : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float moveSpeedAir = 0.05f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float groundCheckDistance = 0.1f;
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float gravity = 0.2f;

    [Header("Ground")]
    [Tooltip("地面と判断するためのマスク")] [SerializeField] LayerMask groundLayer;

    // private

    Rigidbody rb;

    /// <summary> 移動方向 </summary>
    Vector2 moveInput;
    /// <summary> ジャンプフラグ </summary>
    bool jump;
    /// <summary> ジャンプ直後カウント </summary>
    int jumpCnt = 0;
    /// <summary> 地面にいるときはtrue </summary>
    bool isGrounded;
    /// <summary> 地面の法線ベクトル </summary>
    Vector3 groundNormal = Vector3.up;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        CheckGround();
        ExecMove();
        ExecJump();
    }

    /// <summary>
    /// 移動処理
    /// </summary>
    void ExecMove()
    {
        // 重力の影響を与えたあとの、Velocity.y
        float nextVY = rb.linearVelocity.y - gravity;

        if (moveInput.sqrMagnitude < 0.01f)
        {
            // 入力なしの時

            StopProccesInMove(nextVY);
        }
        else
        {
            // 入力ありの時

            Vector3 moveDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

            MoveProccesInMove(nextVY, moveDir);

            RotationProccesInMove(moveDir);
        }
    }

    /// <summary>
    /// 移動処理の移動プロセス
    /// </summary>
    void MoveProccesInMove(float nextVY, Vector3 moveDir)
    {
        if (isGrounded)
        {
            // 地面にいるとき

            Vector3 slopeMoveDir = Vector3.ProjectOnPlane(moveDir, groundNormal).normalized;

            Vector3 targetVelocity = slopeMoveDir * moveSpeed;

            rb.linearVelocity = new Vector3(
                targetVelocity.x,
                targetVelocity.y,
                targetVelocity.z
            );
        }
        else
        {
            // 地面にいないとき

            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                nextVY,
                rb.linearVelocity.z
            ) + (moveDir * moveSpeedAir);
        }
    }

    /// <summary>
    /// 移動処理の停止プロセス
    /// </summary>
    void StopProccesInMove(float nextVY)
    {
        if (isGrounded)
        {
            // Velocity.yを0にすることで、上り坂で止まった時に跳ねないようになる
            rb.linearVelocity = new Vector3(0f, 0f, 0f);
        }
        else
        {
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                nextVY,
                rb.linearVelocity.z
            );
        }
    }

    /// <summary>
    /// 移動処理の回転プロセス
    /// </summary>
    void RotationProccesInMove(Vector3 moveDir)
    {
        if (isGrounded)
        {
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
    }

    /// <summary>
    /// ジャンプ処理
    /// </summary>
    void ExecJump()
    {
        if (jumpCnt > 0) jumpCnt--;

        if (jump)
        {
            if (isGrounded)
            {
                // 地面にいるとき

                rb.linearVelocity = new Vector3(
                    rb.linearVelocity.x,
                    jumpForce,
                    rb.linearVelocity.z
                );

                jumpCnt = 5;

            }

            jump = false;
        }
    }

    /// <summary>
    /// 地面判定
    /// </summary>
    void CheckGround()
    {
        // ジャンプ直後は、判定をスキップ
        if (jumpCnt > 0)
        {
            isGrounded = false;
            groundNormal = Vector3.up;

            return;
        }

        // 足元にレイを飛ばして検査

        float space = 0.5f;
        float radius = 0.3f; // キャラの足の太さに合わせる

        RaycastHit hit;
        isGrounded = Physics.SphereCast(
            transform.position + Vector3.up * space, // 開始位置
            radius, // 球の半径
            Vector3.down, // 下方向
            out hit,
            groundCheckDistance + space - radius, // 距離
            groundLayer
        );

        groundNormal = isGrounded ? hit.normal : Vector3.up;
    }

    // setter

    public Vector2 MoveInput { set => moveInput = value; }
    public bool Jump { set => jump = value; }
}
