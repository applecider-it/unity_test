using System.Collections.Generic;
using UnityEngine;

using Game.Utils;
using Game.Stages;
using Game.Commons;

namespace Game.Characters.RigidbodyCharacterControllerParts
{
    /// <summary>
    /// 地面判定処理
    /// </summary>
    public class GroundParts
    {
        private ContactInfos contactInfos = new ContactInfos();

        /// <summary> 地面と判断するためのマスク </summary>
        private LayerMask groundLayer;

        private string name;

        // コンストラクタ
        public GroundParts(string argName)
        {
            name = argName;
        }

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

            contactInfos.insert(collision.collider, result, normal);

            //Debug.Log("OnCollisionEnter: groundContacts.Count: " + contactInfos.Count + " name: " + name);
        }

        /// <summary>
        /// 接触継続時
        /// </summary>
        public void OnCollisionStay(Collision collision, float maxSlopeAngle)
        {
            if (!IsGroundLayer(collision.gameObject)) return;

            CheckGroundContact(collision, maxSlopeAngle, out var result, out var normal);

            contactInfos.update(collision.collider, result, normal);

            //Debug.Log("OnCollisionStay: groundContacts.Count: " + contactInfos.Count + " name: " + name);
        }

        /// <summary>
        /// 接触終了時
        /// </summary>
        public void OnCollisionExit(Collision collision)
        {
            if (!IsGroundLayer(collision.gameObject)) return;

            contactInfos.delete(collision.collider);

            //Debug.Log("OnCollisionExit: groundContacts.Count: " + contactInfos.Count + " name: " + name);
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
            contactInfos.CleanupDestroyedData();
        }

        // getter

        public bool IsGrounded { get => contactInfos.Valid; }
        public Vector3 GroundContactNormal { get => contactInfos.ContactNormal; }
    }
}