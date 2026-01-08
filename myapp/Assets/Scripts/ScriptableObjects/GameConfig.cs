using UnityEngine;

namespace Game.ScriptableObjects
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Scriptable Objects/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        public int maxHp;
        public float moveSpeed;
    }
}