using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldItem : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Get the largest size of the bounding box.
    /// </summary>
    /// <returns></returns>
    public float GetMaxBounds()
    {
        Vector3 bounds = GetComponent<Renderer>().bounds.size * transform.localScale.x;
        return Mathf.Max(Mathf.Max(bounds.x, bounds.y), bounds.z);
    }

    /// <summary>
    /// When you look at an item and can't pick it up, highlight it in red.
    /// </summary>
    public void SetSelectionVisual(bool canPickUp)
    {

    }

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
