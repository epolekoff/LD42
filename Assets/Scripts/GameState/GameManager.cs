using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

    public Player Player;
    public Room Room;
    public GameCanvas GameCanvas;   

    public Color SelectionColorPositive;
    public Color SelectionColorNegative;

    public float GameTime { get; set; }
    public bool GameActive { get { return m_gameActive; } }
    private bool m_gameActive = true;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Increment game time.
        if(m_gameActive)
        {
            GameTime += Time.deltaTime;
        }

        // Quit the game
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
	}

    /// <summary>
    /// Winning the game.
    /// </summary>
    public void WinGame()
    {
        m_gameActive = false;
        GameCanvas.PlayWinAnimation();
    }
}
