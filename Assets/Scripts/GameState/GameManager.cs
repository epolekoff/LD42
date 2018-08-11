using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

    public Player Player;
    public Room Room;

    public Color SelectionColorPositive;
    public Color SelectionColorNegative;

    public float GameTime { get; set; }

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        GameTime += Time.deltaTime;
	}
}
