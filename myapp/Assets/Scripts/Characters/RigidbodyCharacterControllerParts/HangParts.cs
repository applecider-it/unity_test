using System.Collections.Generic;
using UnityEngine;

using Game.Utils;
using Game.Stages;
using Game.Commons;

namespace Game.Characters.RigidbodyCharacterControllerParts
{
    /// <summary>
    /// つかまり判定結果情報
    /// </summary>
    class HangContactInfo
    {
        public bool isHang;
        public Vector3 normal;
    }

    /// <summary>
    /// つかまり判定処理
    /// </summary>
    public class HangParts
    {
        private Dictionary<Collider, HangContactInfo> hangContacts = new Dictionary<Collider, HangContactInfo>();

        /// <summary>
        /// 接触開始時
        /// </summary>
        public void OnCollisionEnter(Collision collision, float maxSlopeAngle)
        {
            if (!IsHangObject(collision.gameObject)) return;

            CheckHangContact(collision, maxSlopeAngle, out var result, out var normal);

            var info = new HangContactInfo
            {
                isHang = result,
                normal = normal
            };

            hangContacts[collision.collider] = info;

            Debug.Log("OnCollisionEnter: hangContacts.Count: " + hangContacts.Count);
        }

        /// <summary>
        /// 接触継続時
        /// </summary>
        public void OnCollisionStay(Collision collision, float maxSlopeAngle)
        {
            if (!IsHangObject(collision.gameObject)) return;

            CheckHangContact(collision, maxSlopeAngle, out var result, out var normal);

            if (hangContacts.TryGetValue(collision.collider, out var info))
            {
                info.isHang = result;
                info.normal = normal;
            }

            Debug.Log("OnCollisionStay: hangContacts.Count: " + hangContacts.Count);
        }

        /// <summary>
        /// 接触終了時
        /// </summary>
        public void OnCollisionExit(Collision collision)
        {
            if (!IsHangObject(collision.gameObject)) return;

            hangContacts.Remove(collision.collider);

            Debug.Log("OnCollisionExit: hangContacts.Count: " + hangContacts.Count);
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
            if (hangContacts.Count == 0) return;

            var removeList = new List<Collider>();

            foreach (var col in hangContacts.Keys)
            {
                if (col == null)
                    removeList.Add(col);
            }

            foreach (var col in removeList)
                hangContacts.Remove(col);
        }

        // getter

        public bool IsHang
        {
            get
            {
                foreach (var info in hangContacts.Values)
                {
                    if (info.isHang) return true;
                }
                return false;
            }
        }
        public Vector3 HangContactNormal
        {
            get
            {
                foreach (var info in hangContacts.Values)
                {
                    if (info.isHang) return info.normal;
                }
                return Vector3.up;
            }
        }
    }
}