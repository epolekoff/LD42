using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscapeTimer : MonoBehaviour
{
    public Text TimerText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        int milliseconds = Mathf.FloorToInt((GameManager.Instance.GameTime - Mathf.Floor(GameManager.Instance.GameTime)) * 1000);
        var span = new TimeSpan(0, 0, 0, Mathf.FloorToInt(GameManager.Instance.GameTime), milliseconds);
        string timeFormat = string.Format("{0:00}:{1:00}:{2:000}", (int)span.TotalMinutes, span.Seconds, span.Milliseconds);
        TimerText.text = timeFormat;
    }
}
