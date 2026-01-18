using UnityEngine;

namespace Game.Objects.RigidbodyObjectControllerParts
{
    /// <summary>
    /// 移動処理
    /// </summary>
    public class MoveParts
    {
        private Rigidbody rb;
        private Transform transform;

        // コンストラクタ
        public MoveParts(Rigidbody argRb, Transform tr)
        {
            rb = argRb;
            transform = tr;
        }

        /// <summary>
        /// 移動処理
        /// </summary>
        public void MoveProcces(
            bool inWaterBuoyancy, float buoyancy, float waterFriction
        )
        {
            if (inWaterBuoyancy)
            {
                // 水中にいるとき

                rb.useGravity = false;

                WaterProcces(
                    buoyancy, waterFriction
                );
            }
            else
            {
                // 水中にいないとき

                rb.useGravity = true;
            }
        }

        /// <summary>
        /// 水中処理
        /// </summary>
        void WaterProcces(
            float buoyancy, float waterFriction
        )
        {
            rb.linearVelocity = (new Vector3(
                rb.linearVelocity.x,
                rb.linearVelocity.y + buoyancy,
                rb.linearVelocity.z
            )) * waterFriction;

            rb.angularVelocity *= waterFriction;
        }
    }
}