using UnityEngine;
using UnityEngine.AI;

namespace Game.Character
{
    /// <summary>
    /// Rigidbodyナビゲーション管理
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(RigidbodyCharacterController))]
    public class RigidbodyNavMove : MonoBehaviour
    {
        public Transform target;

        NavMeshAgent agent;
        Rigidbody rb;
        RigidbodyCharacterController ch;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            rb = GetComponent<Rigidbody>();
            ch = GetComponent<RigidbodyCharacterController>();

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

            desired.Normalize();

            Vector2 moveAxis = Vector2.zero;

            if (desired.sqrMagnitude > 0.01f)
            {
                moveAxis = new Vector2(desired.x, desired.z);
            }

            //Debug.Log(moveAxis);

            ch.MoveInput = moveAxis;
        }
    }
}