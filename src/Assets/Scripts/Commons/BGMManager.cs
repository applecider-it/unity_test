using UnityEngine;
using System.Collections;

using Game.Utils;
using Game.Systems;

namespace Game.Commons
{
    /// <summary>
    /// 複数のBGMをフェードして切り替える処理
    /// </summary>
    public class BGMManager : MonoBehaviour
    {
        private static BGMManager instance;

        [SerializeField] private AudioSource source;

        // フェード処理のCoroutine
        Coroutine fadeCoroutine;

        AudioClip targetClip;

        /// <summary>
        /// BGMを再生する
        /// </summary>
        public void PlayBGM(AudioClip clip)
        {
            // 同じBGMなら終了
            if (targetClip == clip) return;

            // すでにフェード中なら止める
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            targetClip = clip;

            // フェードを開始
            fadeCoroutine = StartCoroutine(FadeBGM(clip));
        }

        /// <summary>
        /// 実際のフェード処理
        /// </summary>
        IEnumerator FadeBGM(AudioClip clip)
        {
            float fadeTime = 3f;
            float waitTime = 1f;

            BGMInfo info = CommonData.GetInstance().GetBGMInfo(clip.name);

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
            source.clip = clip;
            source.volume = info.Volume;
            source.Play();

            Debug.Log("Start BGM " + source.clip.name);
        }

        public static BGMManager GetInstance()
        {
            if (instance == null)
            {
                Debug.Log("BGMManager: GetInstance: Create");
                GameObject obj = StageUtil.GetCommonScriptGameObject();
                instance = obj.GetComponent<BGMManager>();
            }
            return instance;
        }
    }
}