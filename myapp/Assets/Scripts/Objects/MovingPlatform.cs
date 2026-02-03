using System.Collections.Generic;
using UnityEngine;

using Game.Characters;

namespace Game.Objects
{
    /// <summary>
    /// 移動床判定結果情報
    /// </summary>
    public class ContactInfo
    {
        public bool result;
    }

    /// <summary>
    /// 移動床判定結果情報をまとめるクラス
    /// </summary>
    public class ContactInfos
    {
        private Dictionary<Rigidbody, ContactInfo> contacts = new Dictionary<Rigidbody, ContactInfo>();

        /// <summary>
        /// 作成
        /// </summary>
        public void insert(Rigidbody rb, bool result)
        {
            var info = new ContactInfo
            {
                result = result
            };

            contacts[rb] = info;
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void update(Rigidbody rb, bool result)
        {
            if (contacts.TryGetValue(rb, out var info))
            {
                info.result = result;
            }
        }

        /// <summary>
        /// 削除
        /// </summary>
        public void delete(Rigidbody rb)
        {
            contacts.Remove(rb);
        }

        // setter getter

        public Dictionary<Rigidbody, ContactInfo> Contacts { get => contacts; }
    }

    /// <summary>
    /// 移動床管理
    /// </summary>
    public class MovingPlatform : MonoBehaviour
    {
        private Vector3 lastPosition;
        private Quaternion lastRotation;

        private ContactInfos contactInfos = new ContactInfos();

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

                foreach (var (rb, info) in contactInfos.Contacts)
                {
                    if (info.result)
                    {
                        UpdateRidingBody(rb, positionDelta, rotationDelta);
                    }
                }
            }

            lastPosition = transform.position;
            lastRotation = transform.rotation;
        }

        /// <summary>
        /// 接触開始時
        /// </summary>
        private void OnCollisionEnter(Collision collision)
        {
            var rb = collision.rigidbody;
            if (rb == null) return;

            CheckContact(collision, out var result);

            contactInfos.insert(rb, result);
        }

        /// <summary>
        /// 接触継続時
        /// </summary>
        private void OnCollisionStay(Collision collision)
        {
            var rb = collision.rigidbody;
            if (rb == null) return;

            CheckContact(collision, out var result);

            contactInfos.update(rb, result);
        }

        /// <summary>
        /// 接触終了時
        /// </summary>
        private void OnCollisionExit(Collision collision)
        {
            var rb = collision.rigidbody;
            if (rb == null) return;

            contactInfos.delete(rb);
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
        /// 接触確認
        /// </summary>
        private void CheckContact(Collision collision, out bool result)
        {
            result = false;

            // キャラクターが掴んでいるときは有効
            var controller = collision.transform.GetComponent<RigidbodyCharacterController>();
            if (controller != null && controller.ActionType == CharacterActionType.Hang)
            {
                result = true;
            }

            foreach (var contact in collision.contacts)
            {
                if (Vector3.Dot(-contact.normal, Vector3.up) > 0.5f)
                {
                    // 接触面の法線が上向きなら「上に乗っている」と判断

                    result = true;
                    return;
                }
            }
        }
    }
}