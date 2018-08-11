using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {

    private const float ScaleRate = 0.01f;
    private const float MaxScale = 1f;
    private const float MinScale = 0.1f;

    public float Scale { get { return transform.localScale.x; } }
    public float ScaleRatio { get { return (Scale - MinScale) / (MaxScale - MinScale); } }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        HandleShrinking();
    }

    /// <summary>
    /// Shrink constantly.
    /// </summary>
    private void HandleShrinking()
    {
        // Shrink constantly
        transform.localScale -= Vector3.one * ScaleRate * Time.deltaTime;

        // If we hit the player, stop shrinking? Shrink the player?
    }
}
