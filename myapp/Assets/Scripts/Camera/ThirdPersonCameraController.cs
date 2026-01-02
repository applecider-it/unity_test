using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Camera
{
    /// <summary>
    /// New Input System 対応 TPS カメラ
    /// ・マウスで回転
    /// ・キャラクターを追従
    /// ・障害物があればカメラを手前に寄せる
    /// </summary>
    public class ThirdPersonCameraController : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] Transform target; // 追従するキャラクター
        [SerializeField] Transform targetCamera;

        [Header("Camera Settings")]
        [SerializeField] float distance = 9f;
        [SerializeField] float height = 2f;

        [Header("Rotation")]
        [SerializeField] float sensitivity = 0.1f;
        [SerializeField] float minPitch = -30f;
        [SerializeField] float maxPitch = 70f;

        [Header("Collision")]
        [SerializeField] float cameraRadius = 0.3f;          // カメラの当たり判定サイズ
        [SerializeField] LayerMask obstacleLayer;             // 障害物レイヤー

        [Header("Input")]
        [SerializeField] float lookIgnoreTime = 0.2f; // 開始直後にLookを無視する時間

        // Input System から受け取るマウス入力
        Vector2 lookInput;

        float yaw;   // 左右回転
        float pitch; // 上下回転

        float lookTimer;

        void Start()
        {
            Vector3 angles = targetCamera.transform.eulerAngles;
            yaw = angles.y;
            pitch = angles.x;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // 開始直後の入力暴れ対策
            lookTimer = lookIgnoreTime;
        }

        void LateUpdate()
        {
            // 少しの間カメラ更新をしない
            if (lookTimer > 0f)
            {
                lookTimer -= Time.deltaTime;
                return;
            }

            RotateCamera();
            FollowTarget();
        }

        /// <summary>
        /// マウス入力によるカメラ回転
        /// </summary>
        void RotateCamera()
        {
            yaw += lookInput.x * sensitivity;
            pitch -= lookInput.y * sensitivity;

            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        /// <summary>
        /// ターゲット追従
        /// </summary>
        void FollowTarget()
        {
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

            // プレイヤー注視点
            Vector3 targetPos = target.position + Vector3.up * height;

            // 理想のカメラ位置
            Vector3 desiredOffset = rotation * Vector3.back * distance;
            Vector3 desiredPos = targetPos + desiredOffset;

            // プレイヤー → カメラ方向
            Vector3 dir = (desiredPos - targetPos).normalized;

            float currentDistance = distance;

            // 障害物チェック（SphereCast）
            if (Physics.SphereCast(
                targetPos,
                cameraRadius,
                dir,
                out RaycastHit hit,
                distance,
                obstacleLayer))
            {
                // 障害物の手前にカメラを寄せる
                currentDistance = hit.distance;
            }

            // 最終的なカメラ位置
            targetCamera.transform.position =
                targetPos + dir * currentDistance;

            targetCamera.transform.rotation = rotation;
        }

        // ===== Input System から呼ばれる =====

        /// <summary>
        /// Mouse Delta（Look）
        /// </summary>
        public void OnLook(InputValue value)
        {
            lookInput = value.Get<Vector2>();
        }
    }
}