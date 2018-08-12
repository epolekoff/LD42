using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldItem : SelectableItem
{

    private float m_scaleRatioFromPlayer = 1f;

    private const float MinGravity = 135f;
    private const float MaxGravity = 4050f;

    void Start()
    {
        GetComponent<Rigidbody>().useGravity = false;
    }

    public override void Update()
    {
        base.Update();

        float gravity = Mathf.Lerp(MinGravity, MaxGravity, Mathf.Lerp(MinScale, MaxScale, transform.lossyScale.z));
        GetComponent<Rigidbody>().AddForce(-Vector3.up * gravity);
    }

    /// <summary>
    /// Pick it up.
    /// </summary>
    public void PickUp()
    {
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    /// <summary>
    /// Drop it
    /// </summary>
    public void Drop()
    {
        GetComponent<Collider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
    }
}
