using UnityEngine;
using UnityEngine.AI;

namespace Game.Character
{
    /// <summary>
    /// Rigidbodyナビゲーション管理
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class RigidbodyNavMove : MonoBehaviour
    {
        public Transform target;
        public float moveSpeed = 3f;
        public float rotateSpeed = 10f; // 回転の速さ

        NavMeshAgent agent;
        Rigidbody rb;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            rb = GetComponent<Rigidbody>();

            agent.updatePosition = false;
            agent.updateRotation = false;
        }

        void FixedUpdate()
        {
            // Navメッシュエージェントの位置情報のずれを修正
            agent.nextPosition = rb.position;

            agent.SetDestination(target.position);

            Vector3 desired = agent.desiredVelocity;
            desired.y = 0;

            // --- 移動 ---
            Vector3 velocity = desired.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(
                velocity.x,
                rb.linearVelocity.y,
                velocity.z
            );

            // --- 回転 ---
            if (desired.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(desired);
                Quaternion newRot = Quaternion.Slerp(
                    rb.rotation,
                    targetRot,
                    rotateSpeed * Time.fixedDeltaTime
                );

                rb.MoveRotation(newRot);
            }
        }
    }
}