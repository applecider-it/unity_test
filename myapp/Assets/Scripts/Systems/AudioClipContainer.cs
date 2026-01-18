using UnityEngine;

namespace Game.Systems
{
    [System.Serializable]
    public class AudioClipContainer
    {
        [SerializeField] private AudioClip clip;
        [Range(0, 1)][SerializeField] private float volume = 1f;

        private AudioSource targetAudioSource;

        /// <summary>
        /// １回再生。データがないときは無視される。
        /// </summary>
        public void PlayOneShot()
        {
            if (targetAudioSource == null || clip == null) return;

            targetAudioSource.PlayOneShot(clip, volume);
        }

        // setter getter

        public AudioSource TargetAudioSource { set => targetAudioSource = value; }
    }
}