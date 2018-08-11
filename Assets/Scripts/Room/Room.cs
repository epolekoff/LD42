using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {

    private const float ScaleRate = 0.01f;
    private const float MaxScale = 1f;
    private const float MinScale = 0.1f;

    private bool m_growing = false;
    private const float GrowTime = 2f;

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
        if(m_growing)
        {
            return;
        }

        // Shrink constantly
        transform.localScale -= Vector3.one * ScaleRate * Time.deltaTime;

        // If we hit the player, stop shrinking? Shrink the player?
    }

    public void GrowToScale(float newScale)
    {

    }

    private IEnumerator GrowToScaleCoroutine(float newScale)
    {
        m_growing = true;
        float startScale = Scale;
        float timer = 0;

        while(timer < GrowTime)
        {
            timer += Time.deltaTime;
            float currentScale = Mathf.Lerp(startScale, newScale, timer / GrowTime);
            transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            yield return new WaitForEndOfFrame();
        }

        m_growing = false;
    }
}
