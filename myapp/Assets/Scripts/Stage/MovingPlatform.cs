using System.Collections.Generic;
using UnityEngine;

using Game.Character;

namespace Game.Stage
{
    /// <summary>
    /// 移動床管理
    /// </summary>
    public class MovingPlatform : MonoBehaviour
    {
        private Vector3 lastPosition;
        private Quaternion lastRotation;

        /// <summary> 上に乗っている Rigidbody 一覧 </summary>
        private HashSet<Rigidbody> ridingBodies = new HashSet<Rigidbody>();

        void Start()
        {
            lastPosition = transform.position;
            lastRotation = transform.rotation;
        }

        void LateUpdate()
        {
            Vector3 positionDelta = transform.position - lastPosition;
            Quaternion rotationDelta = transform.rotation * Quaternion.Inverse(lastRotation);

            if (positionDelta != Vector3.zero || rotationDelta != Quaternion.identity)
            {
                // 変化があった時

                foreach (var rb in ridingBodies)
                {
                    UpdateRidingBody(rb, positionDelta, rotationDelta);
                }
            }

            lastPosition = transform.position;
            lastRotation = transform.rotation;
        }

        /// <summary>
        /// 上に乗っているRigidbodyを更新
        /// </summary>
        private void UpdateRidingBody(Rigidbody rb, Vector3 positionDelta, Quaternion rotationDelta)
        {
            // 回転による位置変化
            Vector3 relativePos = rb.position - transform.position;
            Vector3 rotatedPos = rotationDelta * relativePos;

            Vector3 deltaPos = (transform.position + rotatedPos + positionDelta) - rb.position;

            SetDeltaPosToCharacter(rb, deltaPos);

            rb.position += deltaPos;

            // Y軸だけ回転を適用
            Vector3 euler = rotationDelta.eulerAngles;
            Quaternion yRotation = Quaternion.Euler(0f, euler.y, 0f);

            rb.rotation = yRotation * rb.rotation;
        }


        /// <summary>
        /// 上に乗っているRigidbodyが、RigidbodyCharacterControllerの場合の処理
        /// </summary>
        private void SetDeltaPosToCharacter(Rigidbody rb, Vector3 value)
        {
            var controller = rb.GetComponent<RigidbodyCharacterController>();
            if (controller != null)
            {
                controller.MovingPlatformDeltaPos = value;
            }
        }

        /// <summary>
        /// 接触開始時
        /// </summary>
        private void OnCollisionEnter(Collision collision)
        {
            //Debug.Log("OnCollisionEnter");

            var rb = collision.rigidbody;
            if (rb == null) return;

            foreach (var contact in collision.contacts)
            {
                if (Vector3.Dot(-contact.normal, Vector3.up) > 0.5f)
                {
                    // 接触面の法線が上向きなら「上に乗っている」と判断

                    ridingBodies.Add(rb);
                    break;
                }
            }
        }

        /// <summary>
        /// 接触終了時
        /// </summary>
        private void OnCollisionExit(Collision collision)
        {
            //Debug.Log("OnCollisionExit");

            var rb = collision.rigidbody;
            if (rb != null)
            {
                ridingBodies.Remove(rb);
            }
        }
    }
}