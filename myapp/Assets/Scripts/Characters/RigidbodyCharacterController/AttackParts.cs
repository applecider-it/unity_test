using UnityEngine;
using System.Collections;

using Game.Utils;
using Game.Systems;

namespace Game.Characters.RigidbodyCharacterControllerParts
{
    /// <summary>
    /// アタック処理
    /// </summary>
    public class AttackParts
    {
        /// <summary> アタックフラグ </summary>
        private bool attack;

        /// <summary> キャラクターのTransform </summary>
        private Transform transform;

        /// <summary> PKファイアー </summary>
        private GameObject pkFire;

        private AudioSource audioSource;
        private AudioClipContainer attackAudio;

        // コンストラクタ
        public AttackParts(
            Transform argTransform,
            AudioSource argAudioSource,
            AudioClipContainer argAttackAudio
            )
        {
            transform = argTransform;
            audioSource = argAudioSource;
            attackAudio = argAttackAudio;
        }

        public void Awake()
        {
            CommonData cd = CommonData.getCommonData();

            pkFire = cd.PKFire;
        }

        /// <summary>
        /// アタック処理
        /// </summary>
        /// <param name="owner">クロージャーでMonoBehaviourを利用</param>
        public void AttackProccess(MonoBehaviour owner)
        {
            if (attack)
            {
                attack = false;

                //Debug.Log("Attack!!");

                ShootPKFire(owner);

                audioSource.PlayOneShot(attackAudio.clip, attackAudio.volume);
            }

        }

        /// <summary>
        /// PKファイアー発射
        /// </summary>
        void ShootPKFire(MonoBehaviour owner)
        {
            Vector3 position = transform.position + transform.forward * 0.5f + Vector3.up * 1.5f;

            // ファイアー生成
            GameObject fire = UnityEngine.Object.Instantiate(pkFire, position, Quaternion.identity);

            // ファイアーに指向性を持たせる
            Rigidbody rb = fire.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // 前方向で、少し上に発射
                rb.linearVelocity = transform.forward * 10f + Vector3.up * 1f;
            }

            DestroyDelayPKFire(owner, fire);
        }

        /// <summary>
        /// PKファイアー消去予定を設定
        /// </summary>
        void DestroyDelayPKFire(MonoBehaviour owner, GameObject fire)
        {
            // 一定時間経過したときに、消去処理を設定
            owner.StartCoroutine(CallbackUtil.CallAfterSeconds(3f, () =>
            {
                // パーティクルを切り離して、自然に消す
                ParticleSystem ps = fire.GetComponentInChildren<ParticleSystem>();
                if (ps != null)
                {
                    // 親を外す
                    ps.transform.parent = null;

                    // 生成は止める
                    ps.Stop(false, ParticleSystemStopBehavior.StopEmitting);

                    // 既存パーティクルが自然に消えたら消す
                    UnityEngine.Object.Destroy(ps.gameObject, ps.main.startLifetime.constantMax);
                }

                // ファイアーはすぐ消す
                UnityEngine.Object.Destroy(fire);
            }));
        }

        // getter setter

        public bool Attack { set => attack = value; }
    }
}
