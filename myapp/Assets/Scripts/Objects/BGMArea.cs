using UnityEngine;

using Game.Systems;

namespace Game.Objects
{
    public class BGMArea : MonoBehaviour
    {
        // このエリアで流すBGM
        [SerializeField] private AudioClip clip;

        private void OnTriggerEnter(Collider other)
        {
            // プレイヤーが入ったかチェック
            if (other.CompareTag("Player"))
            {
                // BGMマネージャーに切り替えを依頼
                BGMManager.Instance.PlayBGM(clip);
            }
        }
    }
}