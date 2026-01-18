using UnityEngine;

using Game.Systems;

namespace Game.Objects
{
    public class BGMArea : MonoBehaviour
    {
        [Tooltip("このエリアで流すBGM")][SerializeField] private AudioClip clip;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // プレイヤーの場合

                BGMManager.GetInstance().PlayBGM(clip);
            }
        }
    }
}