using System.Collections.Generic;
using UnityEngine;

using Game.Utils;
using Game.Stages;
using Game.Commons;

namespace Game.Characters.RigidbodyCharacterControllerParts
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
    public class GroundParts
    {
        private Dictionary<Collider, GroundContactInfo> groundContacts = new Dictionary<Collider, GroundContactInfo>();

        /// <summary> 地面と判断するためのマスク </summary>
        private LayerMask groundLayer;

        public void Awake()
        {
            CommonData cd = CommonData.GetInstance();

            groundLayer = cd.GroundLayer;
        }

        /// <summary>
        /// 接触開始時
        /// </summary>
        public void OnCollisionEnter(Collision collision, float maxSlopeAngle)
        {
            if (!IsGroundLayer(collision.gameObject)) return;

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
        public void OnCollisionStay(Collision collision, float maxSlopeAngle)
        {
            if (!IsGroundLayer(collision.gameObject)) return;

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
        public void OnCollisionExit(Collision collision)
        {
            if (!IsGroundLayer(collision.gameObject)) return;

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
                //RayUtil.GetTrueNormal(
                //    contact, out bool trueResult, out Vector3 trueNormal
                //);

                // これで、境界の接点は辞去される
                //    道路の微妙な判定が無効化されてしまうので、なしにした。
                //if (!trueResult)
                //    continue;

                //Debug.Log("contact.normal " + contact.normal);
                // 上向きの面か？（坂も考慮）
                if (Vector3.Angle(contact.normal, Vector3.up) <= maxSlopeAngle)
                {
                    normal = contact.normal;
                    result = true;
                    /*
                    Debug.Log("contact valid: normal: " + normal + " count: " + collision.contactCount);
                    if (collision.contactCount > 1)
                    {
                        foreach (ContactPoint contact2 in collision.contacts)
                        {
                            Debug.Log(" -> normal: " + contact2.normal);
                        }
                    }
                    */
                    return;
                }
            }
        }

        /// <summary>
        /// 地面レイヤーかチェック
        /// </summary>
        bool IsGroundLayer(GameObject obj)
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