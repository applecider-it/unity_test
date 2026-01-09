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
        public RigidbodyCharacterController ch;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            Debug.Log("シーンのセットアップ " + StaticData.startPosition + " " + StaticData.validStartPosition);

            if (StaticData.validStartPosition)
            {
                StaticData.validStartPosition = false;
                ch.transform.position = StaticData.startPosition;
            }
        }
    }
}