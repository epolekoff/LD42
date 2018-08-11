using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldItem : SelectableItem
{


    /// <summary>
    /// Pick it up.
    /// </summary>
    public void PickUp()
    {
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    /// <summary>
    /// Drop it
    /// </summary>
    public void Drop()
    {
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Collider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
    }
}
