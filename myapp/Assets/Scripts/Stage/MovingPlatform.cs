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

    private void OnCollisionEnter(Collision collision)
    {
        var rb = collision.rigidbody;
        if (rb == null) return;

        // 接触面の法線が上向きなら「上に乗っている」と判断
        foreach (var contact in collision.contacts)
        {
            if (Vector3.Dot(-contact.normal, Vector3.up) > 0.5f)
            {
                ridingBodies.Add(rb);
                break;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        var rb = collision.rigidbody;
        if (rb != null)
        {
            ridingBodies.Remove(rb);
        }
    }
}
