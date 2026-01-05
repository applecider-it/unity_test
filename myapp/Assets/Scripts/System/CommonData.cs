using UnityEngine;

namespace Game.System
{
    public class CommonData : MonoBehaviour
    {
        [Tooltip("地面と判断するためのマスク")][SerializeField] private LayerMask groundLayer;

        // getter

        public LayerMask GroundLayer { get => groundLayer; }
    }
}