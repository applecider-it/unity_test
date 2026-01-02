using UnityEngine;

namespace Game.Character.RigidbodyCharacterControllerParts
{
    /// <summary>
    /// 地面判定処理
    /// </summary>
    public class RigidbodyCharacterControllerGround
    {
        /// <summary> 地面接触コライダー </summary>
        private Collider groundContactCollider = null;

        /// <summary> 地面接触カウント </summary>
        private int groundContactCount = 0;
        /// <summary> 地面接触法線ベクトル </summary>
        private Vector3 groundContactNormal = Vector3.zero;

        /// <summary>
        /// 接触開始時
        /// </summary>
        public void OnCollisionEnter(Collision collision, LayerMask groundLayer, float maxSlopeAngle)
        {
            if (!IsGroundLayer(collision.gameObject, groundLayer)) return;

            if (IsGroundContact(collision, maxSlopeAngle))
            {
                groundContactCollider = collision.collider;
                groundContactCount++;
            }
        }

        /// <summary>
        /// 接触継続時
        /// </summary>
        public void OnCollisionStay(Collision collision, LayerMask groundLayer, float maxSlopeAngle)
        {
            if (!IsGroundLayer(collision.gameObject, groundLayer)) return;

            // 接触した時の地面以外は対象外
            if (groundContactCollider != collision.collider) return;

            if (IsGroundContact(collision, maxSlopeAngle))
            {
                groundContactCount = Mathf.Max(groundContactCount, 1);
            }
            else
            {
                groundContactCount = Mathf.Max(groundContactCount - 1, 0);
            }
        }

        /// <summary>
        /// 接触終了時
        /// </summary>
        public void OnCollisionExit(Collision collision, LayerMask groundLayer)
        {
            if (!IsGroundLayer(collision.gameObject, groundLayer)) return;

            groundContactCount = Mathf.Max(groundContactCount - 1, 0);
        }

        /// <summary>
        /// 接触点が「足元付近」かチェック
        /// </summary>
        bool IsGroundContact(Collision collision, float maxSlopeAngle)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                //Debug.Log("contact.normal " + contact.normal);
                // 上向きの面か？（坂も考慮）
                if (Vector3.Angle(contact.normal, Vector3.up) <= maxSlopeAngle)
                {
                    //Debug.Log("contact valid");
                    groundContactNormal = contact.normal;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 地面レイヤーかチェック
        /// </summary>
        bool IsGroundLayer(GameObject obj, LayerMask groundLayer)
        {
            return (groundLayer.value & (1 << obj.layer)) != 0;
        }

        // getter

        public int GroundContactCount { get => groundContactCount; }
        public Vector3 GroundContactNormal { get => groundContactNormal; }
    }
}