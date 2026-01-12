using UnityEngine;

namespace Game.Systems
{
    [System.Serializable]
    public class AudioClipContainer
    {
        [SerializeField] private AudioClip clip;
        [Range(0, 1)][SerializeField] private float volume = 1f;

        private AudioSource audioSource;

        /// <summary>
        /// １回再生。データがないときは無視される。
        /// </summary>
        public void PlayOneShot()
        {
            if (audioSource == null || clip == null) return;

            audioSource.PlayOneShot(clip, volume);
        }

        // setter

        public AudioSource TargetAudioSource
        {
            set => audioSource = value;
        }
    }
}