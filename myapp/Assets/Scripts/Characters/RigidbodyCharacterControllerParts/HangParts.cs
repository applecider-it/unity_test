using System.Collections.Generic;
using UnityEngine;

using Game.Utils;
using Game.Stages;
using Game.Commons;

namespace Game.Characters.RigidbodyCharacterControllerParts
{
    /// <summary>
    /// つかまり判定処理
    /// </summary>
    public class HangParts
    {
        private ContactInfos contactInfos = new ContactInfos();

        private string name;

        // コンストラクタ
        public HangParts(string argName)
        {
            name = argName;
        }

        /// <summary>
        /// 接触開始時
        /// </summary>
        public void OnCollisionEnter(Collision collision, float maxSlopeAngle)
        {
            if (!IsHangObject(collision.gameObject)) return;

            CheckHangContact(collision, maxSlopeAngle, out var result, out var normal);

            contactInfos.insert(collision.collider, result, normal);

            //Debug.Log("OnCollisionEnter: groundContacts.Count: " + contactInfos.Count + " name: " + name);
        }

        /// <summary>
        /// 接触継続時
        /// </summary>
        public void OnCollisionStay(Collision collision, float maxSlopeAngle)
        {
            if (!IsHangObject(collision.gameObject)) return;

            CheckHangContact(collision, maxSlopeAngle, out var result, out var normal);

            contactInfos.update(collision.collider, result, normal);

            //Debug.Log("OnCollisionStay: groundContacts.Count: " + contactInfos.Count + " name: " + name);
        }

        /// <summary>
        /// 接触終了時
        /// </summary>
        public void OnCollisionExit(Collision collision)
        {
            if (!IsHangObject(collision.gameObject)) return;

            contactInfos.delete(collision.collider);

            //Debug.Log("OnCollisionExit: groundContacts.Count: " + contactInfos.Count + " name: " + name);
        }

        /// <summary>
        /// つかまれるオブジェクトかチェック
        /// </summary>
        void CheckHangContact(Collision collision, float maxSlopeAngle, out bool result, out Vector3 normal)
        {
            normal = Vector3.up;
            result = false;

            foreach (ContactPoint contact in collision.contacts)
            {
                if (Vector3.Angle(contact.normal, Vector3.up) > maxSlopeAngle)
                {
                    // つかまる角度の時

                    normal = contact.normal;
                    result = true;

                    return;
                }
            }
        }

        /// <summary>
        /// つかまれるオブジェクトかチェック
        /// </summary>
        bool IsHangObject(GameObject obj)
        {
            return obj.tag == "Hang";
        }

        /// <summary>
        /// 消滅するオブジェクトが残るので消す処理
        /// </summary>
        public void CleanupDestroyedHangObject()
        {
            contactInfos.CleanupDestroyedData();
        }

        // getter

        public bool IsHang { get => contactInfos.Valid; }
        public Vector3 HangContactNormal { get => contactInfos.ContactNormal; }
    }
}