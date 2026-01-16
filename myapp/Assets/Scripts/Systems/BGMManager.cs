using UnityEngine;
using System.Collections;

namespace Game.Systems
{
    /// <summary>
    /// 複数のBGMをフェードして切り替える処理
    /// </summary>
    public class BGMManager : MonoBehaviour
    {
        public static BGMManager instance;

        public AudioSource source;

        private string step = "ready";

        void Awake()
        {
            instance = this;
        }

        // 外部からBGMを再生するための関数
        public void PlayBGM(AudioClipContainer clipContainer)
        {
            // 同じBGMなら終了
            if (source.clip == clipContainer.Clip) return;

            // 変更受付状態以外では終了
            if (step != "ready") return;

            // フェードを開始
            step = "start";
            StartCoroutine(FadeBGM(clipContainer));
        }

        // 実際のフェード処理
        IEnumerator FadeBGM(AudioClipContainer clipContainer)
        {
            float fadeTime = 3f;
            float waitTime = 1f;

            if (source.clip != null)
            {
                // 再生中の場合

                float t = 0;
                float startVolume = source.volume;

                // 指定した時間かけてフェード
                while (t < fadeTime)
                {
                    t += Time.deltaTime;
                    float rate = t / fadeTime;

                    // 今のBGMをだんだん小さく
                    source.volume = Mathf.Lerp(startVolume, 0, rate);

                    yield return null;
                }

                source.Stop();
            }

            yield return new WaitForSeconds(waitTime);

            // 新しいBGMを開始
            source.clip = clipContainer.Clip;
            source.volume = clipContainer.Volume;
            source.Play();

            step = "ready";
        }

        // getter setter

        public static BGMManager Instance { get => instance; }
    }
}