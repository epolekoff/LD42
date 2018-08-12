using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {

    private const float ScaleRate = 0.01f;
    private const float MaxScale = 3f;
    private const float MinScale = 0.1f;
    private const float MaxForcePlayerScale = 1.2f;
    private const float MinForcePlayerScale = 0.1f;

    private bool m_growing = false;
    private const float GrowTime = 2f;

    public float Scale { get { return transform.localScale.x; } }
    public float ScaleRatio { get { return (Scale - MinScale) / (MaxScale - MinScale); } }
    public float ForcePlayerScaleRatio { get { return (Scale - MinForcePlayerScale) / (MaxForcePlayerScale - MinForcePlayerScale); } }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        //HandleShrinking();
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

        if(Scale > MinScale)
        {
            // Shrink constantly
            transform.localScale -= Vector3.one * ScaleRate * Time.deltaTime;
        }

        // If we hit the player, stop shrinking? Shrink the player?
        if(!m_growing && 
            ScaleRatio != 0 &&
            ForcePlayerScaleRatio < GameManager.Instance.Player.ScaleRatio)
        {
            GameManager.Instance.Player.ForceScaleDown(ForcePlayerScaleRatio);
        }
    }

    /// <summary>
    /// Grow the room when a button is pressed.
    /// </summary>
    /// <param name="newScale"></param>
    public void GrowToScale(float newScale)
    {
        if(Scale < newScale)
        {
            StartCoroutine(GrowToScaleCoroutine(newScale));
        }
    }

    /// <summary>
    /// Lerp the growth of the room.
    /// </summary>
    /// <param name="newScale"></param>
    /// <returns></returns>
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
