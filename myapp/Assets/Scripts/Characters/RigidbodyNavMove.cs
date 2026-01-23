using UnityEngine;
using UnityEngine.AI;

using Game.Stages;
using Game.Commons;

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

        float resetIgnoreTime = 2f;
        float resetTimer;

        float repathTimer;

        void Awake()
        {
            CommonData cd = CommonData.GetInstance();

            agent = GetComponent<NavMeshAgent>();
            rb = GetComponent<Rigidbody>();
            ch = GetComponent<RigidbodyCharacterController>();

            target = cd.Player.transform;

            agent.updatePosition = false;
            agent.updateRotation = false;

            resetTimer = resetIgnoreTime;
        }

        void FixedUpdate()
        {
            SyncAgentToBody();

            repathTimer -= Time.fixedDeltaTime;
            if (repathTimer <= 0f)
            {
                agent.SetDestination(target.position);
                repathTimer = 1f;

                //Debug.Log("SetDestination");
            }

            Vector3 desired = agent.desiredVelocity;
            //desired.y = 0;

            Vector2 moveAxis = Vector2.zero;

            if (desired.sqrMagnitude > 0.01f)
            {
                moveAxis = new Vector2(desired.x, desired.z);

                moveAxis.Normalize();
            }

            //Debug.Log(moveAxis);

            ch.MoveInput = moveAxis;
            //ch.CursorInput = Vector2.down;

            if (jump)
            {
                ch.Jump = true;

                jump = false;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            Debug.Log("入った: " + other.tag);

            if (other.tag == "JumpArea")
            {
                jump = true;
            }
        }

        /// <summary>
        /// Navメッシュエージェントの位置情報のずれを修正
        /// </summary>
        void SyncAgentToBody()
        {
            float dist = Vector3.Distance(agent.nextPosition, rb.position);

            if (dist > 1.5f)
            {
                // agentと大きくずれたとき（崖などを滑り落ちた時などに起きる）

                // リセット直後は、少しの間リセットをしない
                if (resetTimer > 0f)
                {
                    resetTimer -= Time.deltaTime;
                    return;
                }

                resetTimer = resetIgnoreTime;

                // リセット
                agent.Warp(rb.position);

                Debug.Log("Reset NavAgent: dist: " + dist);
            }
            else
            {
                // 通常時も少しづつずれるので、それを埋める
                agent.nextPosition = rb.position;
            }
        }
    }
}