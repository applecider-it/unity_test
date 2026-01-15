using UnityEngine;
using System.Collections.Generic;

namespace Game.Characters.RigidbodyCharacterControllerParts
{
    /// <summary>
    /// 水中判定処理
    /// </summary>
    public class HangParts
    {
        Collider collider;
        Vector3 normal;

        /// <summary>
        /// 接触開始時
        /// </summary>
        public void OnCollisionEnter(Collision collision)
        {
            Collider other = collision.collider;

            if (other.tag == "Hang")
            {
                Debug.Log("つかんだ: " + other.tag);
                collider = other;
                normal = collision.contacts[0].normal;
            }
        }

        /// <summary>
        /// 接触継続時
        /// </summary>
        public void OnCollisionStay(Collision collision)
        {
            Collider other = collision.collider;

            if (other.tag == "Hang")
            {
                Debug.Log("つかんだ: " + other.tag);
                normal = collision.contacts[0].normal;
            }
        }

        /// <summary>
        /// 接触終了時
        /// </summary>
        public void OnCollisionExit(Collision collision)
        {
            Collider other = collision.collider;
            
            if (other.tag == "Hang")
            {
                Debug.Log("はなした: " + other.tag);
                collider = null;
            }
        }

        /// <summary>
        /// 掴めるオブジェクトに接触しているか
        /// </summary>
        public bool IsHang()
        {
            return collider != null;
        }

        // getter setter

        public Vector3 Normal { get => normal; }
    }
}