using UnityEngine;
using UnityEngine.InputSystem; // ★ 新Input System

using Game.Characters;

namespace Game.Systems
{
    /// <summary>
    /// シーンのセットアップ
    /// </summary>
    public class SceneSetup : MonoBehaviour
    {
        [SerializeField] RigidbodyCharacterController ch;
        [SerializeField] Transform targetCamera;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            SetupCharacterAndCamera();
        }

        /// <summary>
        /// キャラクターとカメラの初期状態変更
        /// </summary>
        void SetupCharacterAndCamera()
        {
            var info = StaticData.SceneConnectorInfo;
            Debug.Log("シーンのセットアップ " + info);

            if (info != null)
            {
                ch.transform.position = info.startPosition;

                Vector3 euler = targetCamera.transform.rotation.eulerAngles;
                euler.y = info.cameraAngleY;
                targetCamera.transform.rotation = Quaternion.Euler(euler);

                euler = ch.transform.rotation.eulerAngles;
                euler.y = info.cameraAngleY;
                ch.transform.rotation = Quaternion.Euler(euler);

                StaticData.SceneConnectorInfo = null;

            }
        }
    }
}