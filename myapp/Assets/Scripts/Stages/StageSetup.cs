using UnityEngine;
using UnityEngine.InputSystem; // ★ 新Input System

using Game.Characters;
using Game.Commons;
using Game.Systems;

namespace Game.Stages
{
    /// <summary>
    /// ステージのセットアップ
    /// </summary>
    public class StageSetup : MonoBehaviour
    {
        Transform targetCamera;
        RigidbodyCharacterController ch;

        [Header("Background")]
        [SerializeField] CameraClearFlags cameraClearFlag = CameraClearFlags.Skybox;
        [SerializeField] Color backgroundColor  = Color.black;

        [Header("Audio")]
        [Tooltip("このステージで流すBGM")][SerializeField] private AudioClip bgmClip;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            CommonData cd = CommonData.GetInstance();

            ch = cd.Player.GetComponent<RigidbodyCharacterController>();
            targetCamera = cd.Camera.transform;

            SetupBGM();
            SetupCamera();
            SetupCharacter();
        }

        /// <summary>
        /// BGMのセットアップ
        /// </summary>
        void SetupBGM()
        {
            if (bgmClip == null) return;

            BGMManager.GetInstance().PlayBGM(bgmClip);
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
            CommonStatus cs = CommonStatus.getCommonStatus();

            var info = cs.NextSceneConnectorInfo;
            Debug.Log("シーンのセットアップ " + info);

            if (info != null)
            {
                ch.transform.position = info.startPosition;

                cs.NextSceneConnectorInfo = null;
            }
        }
    }
}