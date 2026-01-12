using UnityEngine;
using UnityEngine.InputSystem; // ★ 新Input System

using Game.Characters;

namespace Game.Systems
{
    /// <summary>
    /// ステージのセットアップ
    /// </summary>
    public class StageSetup : MonoBehaviour
    {
        Transform targetCamera;
        RigidbodyCharacterController ch;

        [SerializeField] CameraClearFlags cameraClearFlag = CameraClearFlags.Skybox;
        [SerializeField] Color backgroundColor  = Color.black;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            CommonData cd = CommonData.getCommonData();

            ch = cd.Player.GetComponent<RigidbodyCharacterController>();
            targetCamera = cd.Camera.transform;

            SetupCamera();
            SetupCharacter();
        }

        /// <summary>
        /// カメラのセットアップ
        /// </summary>
        void SetupCamera()
        {
            Camera cam = targetCamera.GetComponent<Camera>();
            cam.clearFlags = cameraClearFlag;
            cam.backgroundColor  = backgroundColor;
        }

        /// <summary>
        /// キャラクターのセットアップ
        /// 
        /// 初期状態変更
        /// </summary>
        void SetupCharacter()
        {
            var info = StaticData.SceneConnectorInfo;
            Debug.Log("シーンのセットアップ " + info);

            if (info != null)
            {
                ch.transform.position = info.startPosition;

                StaticData.SceneConnectorInfo = null;

            }
        }
    }
}