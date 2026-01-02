using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private Vector3 lastPosition;
    private Quaternion lastRotation;

    // 上に乗っている Rigidbody 一覧
    private HashSet<Rigidbody> ridingBodies = new HashSet<Rigidbody>();

    void Start()
    {
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    void LateUpdate()
    {
        Vector3 positionDelta = transform.position - lastPosition;
        Quaternion rotationDelta = transform.rotation * Quaternion.Inverse(lastRotation);

        if (positionDelta != Vector3.zero || rotationDelta != Quaternion.identity)
        {
            foreach (var rb in ridingBodies)
            {
                // 回転による位置変化
                Vector3 relativePos = rb.position - lastPosition;
                Vector3 rotatedPos = rotationDelta * relativePos;

                rb.position = lastPosition + rotatedPos + positionDelta;

                // 足場の回転に合わせて Rigidbody も回転させたい場合
                rb.rotation = rotationDelta * rb.rotation;
            }
        }

        lastPosition = transform.position;
        lastRotation = transform.rotation;
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
