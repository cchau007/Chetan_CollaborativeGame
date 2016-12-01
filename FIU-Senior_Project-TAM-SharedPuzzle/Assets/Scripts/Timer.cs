using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour {
    //variables

    /// <summary>
    /// Used to count down timer
    /// </summary>
    public float timeLeft;

    /// <summary>
    /// Used to change UI text
    /// </summary>
    private TextEdit tScript;

    /// <summary>
    /// Colliders that block the ball from moving at start.
    /// </summary>
    public BoxCollider bMaze1, bMaze2;

    /// <summary>
    /// Used to enable the timer
    /// </summary>
    public bool isTimerStart = false;

    //methods


    /// <summary>
    ///Use this for initialization
    /// </summary>
    void Start () {
        tScript = GameObject.Find("UIManager").GetComponent<TextEdit>();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update () {
        if (isTimerStart)
        {
            //when the round starts allow the ball to move based on the maze we are playing
            switch (GameObject.Find("GameController").GetComponent<GameController>().difficulty)
            {
                default:
                    bMaze1.enabled = false;
                    break;
                case 2:
                    bMaze2.enabled = false;
                    break;
                case 3:
                    bMaze2.enabled = false;
                    break;
            }

            timeLeft -= Time.deltaTime;

            if (timeLeft > 0)
            {
                tScript.ChangeText("" + (int)timeLeft);
            }
            else
            {
                tScript.ChangeText("Round Over");
                isTimerStart = false;
                if (GameObject.Find("GameController").GetComponent<GameController>().NextRound())
                {
                    StartCoroutine(GameObject.Find("GameController").GetComponent<GameController>().StartRound());
                } 
                else
                    AppStateManager.Instance.CurrentAppState = AppStateManager.AppState.EndGame;
            }
        }
	}
}
