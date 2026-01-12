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

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            CommonData cd = CommonData.getCommonData();

            ch = cd.Player.GetComponent<RigidbodyCharacterController>();
            targetCamera = cd.Camera.transform;

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

                StaticData.SceneConnectorInfo = null;

            }
        }
    }
}