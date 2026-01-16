using UnityEngine;
using System.Collections;

namespace Game.Systems
{
    /// <summary>
    /// 複数のBGMをフェードして切り替える処理
    /// </summary>
    public class BGMManager : MonoBehaviour
    {
        // どこからでもアクセスできるようにするシングルトン
        public static BGMManager Instance;

        // クロスフェード用の2つのAudioSource
        public AudioSource sourceA;
        public AudioSource sourceB;

        // 今流れているBGM
        AudioSource current;
        // 次に流すBGM
        AudioSource next;

        // フェード処理のCoroutine
        Coroutine fadeCoroutine;

        void Awake()
        {
            // インスタンス登録
            Instance = this;

            // 最初はAを再生用、Bを次用に設定
            current = sourceA;
            next = sourceB;
        }

        // 外部からBGMを再生するための関数
        public void PlayBGM(AudioClip clip)
        {
            // 同じBGMなら何もしない
            if (current.clip == clip) return;

            // すでにフェード中なら止める
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            // 新しいフェードを開始
            fadeCoroutine = StartCoroutine(FadeBGM(clip));
        }

        // 実際のフェード処理
        IEnumerator FadeBGM(AudioClip clip)
        {
            float fadeTime = 3f;

            if (current.clip != null)
            {
                // 再生中の場合

                float t = 0;
                float startVolume = current.volume;

                // 指定した時間かけてフェード
                while (t < fadeTime)
                {
                    t += Time.deltaTime;
                    float rate = t / fadeTime;

                    // 今のBGMをだんだん小さく
                    current.volume = Mathf.Lerp(startVolume, 0, rate);

                    yield return null;
                }

                current.Stop();
            }

            // 次のBGMを開始
            next.clip = clip;
            next.volume = 1;
            next.Play();

            // current と next を入れ替える
            var temp = current;
            current = next;
            next = temp;
        }
    }
}