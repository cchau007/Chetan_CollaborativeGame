using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Sharing; //for custom messages

public class Player : MonoBehaviour {

    //variables

    /// <summary>
    ///Store the rectangle container for the colliders we interact with
    /// </summary>
    private GameObject gObjRectangleManager;

    /// <summary>
    ///Store the script used to rotate the puzzle
    /// </summary>
    private Puzzle pScript;

    /// <summary>
    ///List of BoxColliders we will interact with
    /// </summary>
    public List<BoxCollider> Box = new List<BoxCollider>();

    /// <summary>
    ///Toggle to control whether player is allowed to move the maze
    /// </summary>
    public bool isMoveAllowed = false;

    //methods

    /// <summary>
    ///Use this for initialization
    /// </summary>
    void Start()
    {
        // We care about getting updates for the model transform.
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.Direction] = this.OnMazeTransform;

        gObjRectangleManager = GameObject.Find("RectangleManager");
        pScript = GameObject.Find("Puzzle").GetComponent<Puzzle>();
        foreach (BoxCollider collider in gObjRectangleManager.GetComponents<BoxCollider>())
        {
            Box.Add(collider);
        }

    }

    /// <summary>
    ///Update is called once per frame
    /// </summary>
    void Update()
    {

    }

    /// <summary>
    ///Called when we enter a collider
    /// </summary>
    /// <param name="other">The Collider we are interacting with</param>
    void OnTriggerEnter(Collider other)
    {

        if (isMoveAllowed)
        {
           
            System.Int32 temp = Box.IndexOf((BoxCollider)other);
            //GameObject.Find("GameController").GetComponent<GameController>().tScript.ChangeText("Direction Sent:" + temp);
            CustomMessages.Instance.SendMazeTransform(temp); //send maze changes to other hololens
         // Debug.Log("Direction Sent:" + Box.IndexOf((BoxCollider)other));
           // pScript.Rotate(Box.IndexOf((BoxCollider)other)); //local rotation
        }
    }

    /// <summary>
    ///Called when we stay a collider
    /// </summary>
    /// <param name="other">The Collider we are interacting with</param>
    void OnTriggerStay(Collider other)
    {

        if (isMoveAllowed)
        {
            System.Int32 temp = Box.IndexOf((BoxCollider)other);
            CustomMessages.Instance.SendMazeTransform(temp); //send maze changes to other hololens
             // Debug.Log("Direction Sent:" + Box.IndexOf((BoxCollider)other));
            //pScript.Rotate(Box.IndexOf((BoxCollider)other)); //local rotation
        }

    }

    /// <summary>
    /// We call this when a remote player tries to manipulate our local maze
    /// </summary>
    /// <param name="msg">The msg that tells us which way to rotate the maze</param>
    void OnMazeTransform(NetworkInMessage msg)
    {
        // We read the user ID but we don't use it here.
        long temp = msg.ReadInt64();

        //get our local user id and store it
        //we do this here since we are assured the IDs have been set
        long localUserId = CustomMessages.Instance.localUserID;

        if(temp != localUserId)
            pScript.Rotate(msg.ReadInt32());


        //rDirection = CustomMessages.Instance.ReadInt(msg);
        //Rotate(rDirection);

    }
}
