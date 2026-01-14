using UnityEngine;
using UnityEngine.AI;

using Game.Stages;

namespace Game.Characters
{
    /// <summary>
    /// Rigidbodyナビゲーション管理
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(RigidbodyCharacterController))]
    public class RigidbodyNavMove : MonoBehaviour
    {
        Transform target;

        NavMeshAgent agent;
        Rigidbody rb;
        RigidbodyCharacterController ch;

        bool jump;

        void Awake()
        {
            CommonData cd = CommonData.getCommonData();

            agent = GetComponent<NavMeshAgent>();
            rb = GetComponent<Rigidbody>();
            ch = GetComponent<RigidbodyCharacterController>();

            target = cd.Player.transform;

            agent.updatePosition = false;
            agent.updateRotation = false;
        }

        void OnTriggerEnter(Collider other)
        {
            Debug.Log("入った: " + other.tag);

            if (other.tag == "JumpArea")
            {
                jump = true;
            }
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

            if (jump)
            {
                ch.Jump = true;

                jump = false;
            }
        }
    }
}