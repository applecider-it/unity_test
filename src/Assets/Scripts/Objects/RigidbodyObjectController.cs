using UnityEngine;

using Game.Objects.RigidbodyObjectControllerParts;

using Game.Systems;

namespace Game.Objects
{
    /// <summary>
    /// Rigidbodyオブジェクト管理
    /// </summary>
    public class RigidbodyObjectController : MonoBehaviour
    {
        [Header("Physics")]
        [Tooltip("浮力")][SerializeField] private float buoyancy = 0.1f;
        [Tooltip("水の摩擦")][SerializeField] private float waterFriction = 0.98f;

        // private

        private MoveParts moveCtrl;
        private WaterParts waterCtrl;

        Collider myCol;

        void OnEnable()
        {
            Rigidbody rb = GetComponent<Rigidbody>();

            moveCtrl = new MoveParts(rb, transform);
            waterCtrl = new WaterParts(name);

            myCol = GetComponent<Collider>();
        }

        void FixedUpdate()
        {
            bool inWaterBuoyancy = waterCtrl.PointCheck(myCol.bounds.center);

            moveCtrl.MoveProcces(
                inWaterBuoyancy, buoyancy, waterFriction
            );
        }

        /// <summary>
        /// トリガー開始時
        /// </summary>
        void OnTriggerEnter(Collider other)
        {
            waterCtrl.OnTriggerEnter(other);
        }

        /// <summary>
        /// トリガー終了時
        /// </summary>
        void OnTriggerExit(Collider other)
        {
            waterCtrl.OnTriggerExit(other);
        }
    }
}