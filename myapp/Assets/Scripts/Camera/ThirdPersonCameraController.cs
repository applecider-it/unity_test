using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// New Input System 対応 TPS カメラ
/// ・マウスで回転
/// ・キャラクターを追従
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

    // Input System から受け取るマウス入力
    Vector2 lookInput;

    float yaw;   // 左右回転
    float pitch; // 上下回転

    void Start()
    {
        Vector3 angles = targetCamera.transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        RotateCamera();
        FollowTarget();
    }

    /// <summary>
    /// マウス入力によるカメラ回転
    /// </summary>
    void RotateCamera()
    {
        yaw   += lookInput.x * sensitivity;
        pitch -= lookInput.y * sensitivity;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    /// <summary>
    /// ターゲット追従
    /// </summary>
    void FollowTarget()
    {
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        Vector3 targetPos = target.position + Vector3.up * height;
        Vector3 offset = rotation * Vector3.back * distance;

        targetCamera.transform.position = targetPos + offset;
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
