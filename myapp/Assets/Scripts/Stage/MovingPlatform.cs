using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private Vector3 lastPosition;

    // 上に乗っている Rigidbody 一覧
    private HashSet<Rigidbody> ridingBodies = new HashSet<Rigidbody>();

    void Start()
    {
        lastPosition = transform.position;
    }

    void LateUpdate()
    {
        Vector3 delta = transform.position - lastPosition;

        if (delta != Vector3.zero)
        {
            foreach (var rb in ridingBodies)
            {
                // Rigidbody を直接動かす方が安全
                rb.position += delta;
            }
        }

        lastPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        var rb = other.attachedRigidbody;
        if (rb != null)
        {
            ridingBodies.Add(rb);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var rb = other.attachedRigidbody;
        if (rb != null)
        {
            ridingBodies.Remove(rb);
        }
    }
}
