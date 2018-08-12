using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCanvas : MonoBehaviour {

    public EscapeTimer EscapeTimer;
    public GameObject Reticle;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Play the win animation.
    /// </summary>
    public void PlayWinAnimation()
    {
        EscapeTimer.GetComponent<Animator>().enabled = true;
    }

    public void PlayGrabFailAnimation(bool tooBig)
    {
        Reticle.GetComponent<Animator>().SetTrigger(tooBig ? "big" : "small");
    }
}
