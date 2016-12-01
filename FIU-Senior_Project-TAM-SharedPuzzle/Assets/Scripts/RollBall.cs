using UnityEngine;
using System.Collections;

public class RollBall : MonoBehaviour {

    //variables

    /// <summary>
    ///Our rigidbody component
    /// </summary>
    public Rigidbody rb;

    /// <summary>
    ///The force applied to move the ball
    /// </summary>
    public float power;

    /// <summary>
    ///Used to check if ball fell out of the maze
    /// </summary>
    public BoxCollider net;


    //methods

    /// <summary>
    ///Use this for initialization
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Called when physics needs to be updated
    /// </summary>
    void FixedUpdate()
    {
        rb.AddForce(rb.velocity.normalized * power);
        // rb.AddForce(transform.forward * power);
    }

    /// <summary>
    /// Called when physics needs to be updated
    /// </summary>
    /// <param name="other">Collider we are interacting with</param>
    void OnTriggerEnter(Collider other)
    {
        if(other == net) //ball fell out of maze
        {
            ResetBall();
        }
        else //we won
        {
            Debug.Log("You win");
            if (!GameObject.Find("GameController").GetComponent<GameController>().isGoal)
            {
                GameObject.Find("GameController").GetComponent<GameController>().isGoal = true;
                AppStateManager.Instance.CurrentAppState = AppStateManager.AppState.EndGame;
            }
        }

    }

    /// <summary>
    /// Called when we need to reposition the ball to the start of the maze.
    /// </summary>
    public void ResetBall()
    {
        this.transform.position = GameObject.Find("BallMarker").transform.position;
    }
}
