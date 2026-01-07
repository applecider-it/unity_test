using UnityEngine;
using System.Collections;

using Game.Util;
using Game.GameSystem;

namespace Game.Character.RigidbodyCharacterControllerParts
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

        // コンストラクタ
        public AttackParts(Transform argTransform)
        {
            transform = argTransform;
        }

        public void Awake()
        {
            CommonData cd = DataUtil.getCommonData();

            pkFire = cd.PKFire;
        }

        /// <summary>
        /// アタック処理
        /// </summary>
        public void AttackProccess(MonoBehaviour owner)
        {
            if (attack)
            {
                attack = false;

                Debug.Log("Attack!!");

                ShootPKFire(owner);
            }

        }

        void ShootPKFire(MonoBehaviour owner)
        {
            Vector3 position = transform.position + transform.forward * 1.5f + (new Vector3(0, 1f, 0));
            GameObject fire = UnityEngine.Object.Instantiate(pkFire, position, Quaternion.identity);
            Rigidbody rb = fire.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = transform.forward * 10f; // 前方向に発射
            }

            owner.StartCoroutine(CallAfterSeconds(3f, () =>
            {
                Debug.Log("3秒経過した！");
                // パーティクルを子から切り離す
                ParticleSystem ps = fire.GetComponentInChildren<ParticleSystem>();
                if (ps != null)
                {
                    ps.transform.parent = null;       // 親を外す
                    ps.Stop(false, ParticleSystemStopBehavior.StopEmitting); // 生成は止める
                    UnityEngine.Object.Destroy(ps.gameObject, ps.main.startLifetime.constantMax); // 既存パーティクルが自然に消えるまで破棄

                    Debug.Log("パーティクル停止");
                }
                UnityEngine.Object.Destroy(fire);
            }));
        }

        IEnumerator CallAfterSeconds(float seconds, System.Action callback)
        {
            yield return new WaitForSeconds(seconds);
            callback?.Invoke();
        }

        // getter setter

        public bool Attack { set => attack = value; }
    }
}
