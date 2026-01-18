using UnityEngine;

namespace Game.Systems
{
    /// <summary>
    /// BGMデータ
    /// </summary>
    [System.Serializable]
    public class BGMInfo
    {
        [SerializeField] private string name;
        [Range(0, 1)][SerializeField] private float volume = 1;

        // setter getter

        public string Name { get => name; }
        public float Volume { get => volume; }
    }
}