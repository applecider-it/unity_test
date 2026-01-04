using System.Collections.Generic;
using UnityEngine;
using Game.Util;

namespace Game.Character.RigidbodyCharacterControllerParts
{
    /// <summary>
    /// 地面判定結果情報
    /// </summary>
    class GroundContactInfo
    {
        public bool isGrounded;
        public Vector3 normal;
    }

    /// <summary>
    /// 地面判定処理
    /// </summary>
    public class RigidbodyCharacterControllerGround
    {
        private Dictionary<Collider, GroundContactInfo> groundContacts = new Dictionary<Collider, GroundContactInfo>();

        /// <summary>
        /// 接触開始時
        /// </summary>
        public void OnCollisionEnter(Collision collision, LayerMask groundLayer, float maxSlopeAngle)
        {
            if (!IsGroundLayer(collision.gameObject, groundLayer)) return;

            CheckGroundContact(collision, maxSlopeAngle, out var result, out var normal);

            var info = new GroundContactInfo
            {
                isGrounded = result,
                normal = normal
            };

            groundContacts[collision.collider] = info;

            //Debug.Log("OnCollisionEnter: groundContacts.Count: " + groundContacts.Count);
        }

        /// <summary>
        /// 接触継続時
        /// </summary>
        public void OnCollisionStay(Collision collision, LayerMask groundLayer, float maxSlopeAngle)
        {
            if (!IsGroundLayer(collision.gameObject, groundLayer)) return;

            CheckGroundContact(collision, maxSlopeAngle, out var result, out var normal);

            if (groundContacts.TryGetValue(collision.collider, out var info))
            {
                info.isGrounded = result;
                info.normal = normal;
            }

            //Debug.Log("OnCollisionStay: groundContacts.Count: " + groundContacts.Count);
        }

        /// <summary>
        /// 接触終了時
        /// </summary>
        public void OnCollisionExit(Collision collision, LayerMask groundLayer)
        {
            if (!IsGroundLayer(collision.gameObject, groundLayer)) return;

            groundContacts.Remove(collision.collider);

            //Debug.Log("OnCollisionExit: groundContacts.Count: " + groundContacts.Count);
        }

        /// <summary>
        /// 接触点が「足元付近」かチェック
        /// </summary>
        void CheckGroundContact(Collision collision, float maxSlopeAngle, out bool result, out Vector3 normal)
        {
            normal = Vector3.up;
            result = false;

            foreach (ContactPoint contact in collision.contacts)
            {
                //Debug.Log("contact.normal " + contact.normal);
                // 上向きの面か？（坂も考慮）
                if (Vector3.Angle(contact.normal, Vector3.up) <= maxSlopeAngle)
                {
                    //Debug.Log("contact valid");
                    normal = contact.normal;
                    result = true;
                    return;
                }
            }
        }

        /// <summary>
        /// 地面レイヤーかチェック
        /// </summary>
        bool IsGroundLayer(GameObject obj, LayerMask groundLayer)
        {
            return LayerMaskUtil.checkLayerMask(obj, groundLayer);
        }

        /// <summary>
        /// 消滅する地面が残るので消す処理
        /// </summary>
        public void CleanupDestroyedGround()
        {
            if (groundContacts.Count == 0) return;

            var removeList = new List<Collider>();

            foreach (var col in groundContacts.Keys)
            {
                if (col == null)
                    removeList.Add(col);
            }

            foreach (var col in removeList)
                groundContacts.Remove(col);
        }

        // getter

        public bool IsGrounded
        {
            get
            {
                foreach (var info in groundContacts.Values)
                {
                    if (info.isGrounded) return true;
                }
                return false;
            }
        }
        public Vector3 GroundContactNormal
        {
            get
            {
                foreach (var info in groundContacts.Values)
                {
                    if (info.isGrounded) return info.normal;
                }
                return Vector3.up;
            }
        }
    }
}