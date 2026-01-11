using UnityEngine;

namespace Game.Systems
{
    [System.Serializable]
    public class AudioClipContainer
    {
        public AudioClip clip;
        [Range(0, 1)] public float volume = 1f;
    }
}