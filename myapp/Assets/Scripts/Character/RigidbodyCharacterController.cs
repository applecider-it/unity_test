using UnityEngine;

public class RigidbodyCharacterController : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float moveSpeedAir = 0.05f;
    [SerializeField] float jumpForce = 6f;
    [SerializeField] float groundCheckDistance = 0.1f;
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float gravity = 0.2f;

    [Header("Ground")]
    [Tooltip("地面と判断するためのマスク")][SerializeField] LayerMask groundLayer;

    // private

    Rigidbody rb;
    private Animator animator;

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
    /// <summary> 移動床の移動量 </summary>
    Vector3 movingPlatformDeltaPos = Vector3.zero;
    /// <summary> 移動床の移動量有効カウント </summary>
    int movingPlatformDeltaPosCnt = 0;
    /// <summary> 地面にいるときの移動ベクトル </summary>
    Vector3 moveVelocity = Vector3.zero;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        bool noMove = moveInput.sqrMagnitude < 0.01f;

        CheckGround();
        ExecMove(noMove);
        ExecJump();

        SetAnimator(noMove);
    }

    /// <summary>
    /// 移動処理
    /// </summary>
    void ExecMove(bool noMove)
    {
        // 重力の影響を与えたあとの、Velocity.y
        float nextVY = rb.linearVelocity.y - gravity;

        if (noMove)
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

            moveVelocity = slopeMoveDir * moveSpeed;

            Vector3 stickVelocity = -groundNormal * 1f;

            rb.linearVelocity = moveVelocity + stickVelocity;
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
            Vector3 stickVelocity = -groundNormal * 1f;

            moveVelocity = Vector3.zero;

            // こうすることで、上り坂で止まった時に跳ねないようになる
            rb.linearVelocity = moveVelocity + stickVelocity;
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
        if (movingPlatformDeltaPosCnt > 0) movingPlatformDeltaPosCnt--;

        if (jump)
        {
            if (isGrounded)
            {
                // 地面にいるとき

                Vector3 velocity = new Vector3(
                    moveVelocity.x,
                    jumpForce,
                    moveVelocity.z
                );

                // 動く床の影響を足す
                if (movingPlatformDeltaPosCnt > 0)
                {
                    velocity += movingPlatformDeltaPos * 50f;
                }

                rb.linearVelocity = velocity;

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

        float radius = 0.3f; // キャラの足の太さに合わせる
        float space = (radius * 2f) + 0.05f;
        float distance = groundCheckDistance + space - radius;
        Vector3 origin = transform.position + Vector3.up * space;

        RaycastHit hit;
        isGrounded = Physics.SphereCast(
            origin, // 開始位置
            radius, // 球の半径
            Vector3.down, // 下方向
            out hit,
            distance, // 距離
            groundLayer
        );

        // デバッグ描画（接地してたら緑、してなければ赤）
        PhysicsDebugUtil.DrawSphereCast(
            origin,
            radius,
            Vector3.down,
            distance,
            isGrounded ? Color.green : Color.red
        );

        groundNormal = isGrounded ? hit.normal : Vector3.up;
    }

    /// <summary>
    /// アニメーター指定
    /// </summary>
    void SetAnimator(bool noMove)
    {
        if (noMove)
        {
            // 入力なしの時

            animator.SetBool("move", false);
        }
        else
        {
            // 入力ありの時

            animator.SetBool("move", isGrounded);
        }
    }

    // setter

    public Vector2 MoveInput { set => moveInput = value; }
    public bool Jump { set => jump = value; }
    public Vector3 MovingPlatformDeltaPos
    {
        set
        {
            movingPlatformDeltaPos = value;
            movingPlatformDeltaPosCnt = 5;
        }
    }
}
