using UnityEngine;

namespace Game.Systems
{
    public class CommonData : MonoBehaviour
    {
        [Tooltip("地面と判断するためのマスク")][SerializeField] private LayerMask groundLayer;
        [Tooltip("PKファイアー")][SerializeField] private GameObject pkFire;

        // getter

        public LayerMask GroundLayer { get => groundLayer; }
        public GameObject PKFire { get => pkFire; }
    }
}