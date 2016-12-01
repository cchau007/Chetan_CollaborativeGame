using UnityEngine;
using System.Collections;

public class Puzzle : MonoBehaviour {

    //variables

    /// <summary>
    ///Constants for maze angles
    /// </summary>
    public int MAX_ANGLE = 45, MIN_ANGLE = -45;

    /// <summary>
    ///Initial angle when game starts.
    /// </summary>
    private int startAngle;

    /// <summary>
    ///Tracks the mazes current angle
    /// </summary>
    private Vector3 currentAngle;

    /// <summary>
    ///Angles we are changing to
    /// </summary>
    private int targetAngleX = 0, targetAngleZ = 0;

    /// <summary>
    ///Our puzzle and rectangle objects for manipulation
    /// </summary>
    private GameObject gObjRectangle, gObjPuzzle;

    /// <summary>
    ///Position of our rectangle
    /// </summary>
    public Vector3 pos;

    //methods

    /// <summary>
    ///Use this for initialization
    /// </summary>
    void Start () {


        //find our puzzle rectangle so we can use the coor
        gObjRectangle = GameObject.Find("PuzzleManager");
        if (gObjRectangle)
        {
            pos = gObjRectangle.transform.position;
            startAngle = (int)gObjRectangle.transform.eulerAngles.x;
        }

 
        gObjPuzzle = GameObject.Find("Puzzle");
        if (gObjPuzzle)
        {
            currentAngle = gObjPuzzle.transform.eulerAngles;
        }



        MAX_ANGLE += startAngle;
        MIN_ANGLE += startAngle;
        targetAngleX += startAngle;
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update () {
        //var headPosition = Camera.main.transform.position;
        
        //testing
        //var headRotation = Camera.main.transform.rotation;
        //var headx = headRotation.eulerAngles.x;
        //var heady = headRotation.eulerAngles.y;
        //var headz = headRotation.eulerAngles.z;



        /*

        //check where the User is in the rectangle and rotate the puzzle

       // var targetAngleX = 0;
       // var targetAngleZ = 0;


        //need to change this so that it can factor in rotations if the player places the rectangle in an odd angle
        //cos for z maybe sin or tan for x
        //convert radians to degree: radians*(180/3.141593) or  Mathf.Rad2Deg
        if ( ((headPosition.z > (pos1.z)) && (headPosition.z < (pos2.z + 1))) && ((headPosition.x > pos4.x) && (headPosition.x < pos3.x)))
        {

            if ((headPosition.x > pos4.x) && (headPosition.x < (pos3.x - 1)))
            {
                targetAngleZ = MAX_ANGLE;
            }
            else if ((headPosition.x > (pos4.x + 1)) && (headPosition.x < pos3.x))
            {
                targetAngleZ = MIN_ANGLE;

            }
            else
            {
                targetAngleZ = 0;
            }


            if ((headPosition.z > pos1.z) && (headPosition.z < (pos2.z)))
            {
                targetAngleX = MIN_ANGLE;
            }
            else if ((headPosition.z > (pos1.z+1)) && (headPosition.z < (pos2.z+1)))
            {
                targetAngleX = MAX_ANGLE;

            }
            else
            {
                targetAngleX = 0;
            }


        }
        else
        {
            targetAngleX = 0;
            targetAngleZ = 0;
        }

        */









        //optional: change so that lerp rate is based on how far the player is into the rectangle
        currentAngle = new Vector3(
           Mathf.LerpAngle(currentAngle.x, targetAngleX, Time.deltaTime),
          Mathf.LerpAngle(currentAngle.y, startAngle, Time.deltaTime),
          Mathf.LerpAngle(currentAngle.z, targetAngleZ, Time.deltaTime));
         


        gObjPuzzle.transform.eulerAngles = currentAngle;


    }

    /// <summary>
    /// Used to rotate/tilt the maze
    /// </summary>
    /// <param name="direction">The direction we want to rotate</param>
    public void Rotate(System.Int32 direction)
    {
        switch (direction)
        {
            default:
                break;//do nothing
            case 0: //NW
                targetAngleZ = MAX_ANGLE;
                targetAngleX = MAX_ANGLE;
                break;
            case 1: //SW
                targetAngleZ = MAX_ANGLE;
                targetAngleX = MIN_ANGLE;
                break;
            case 2: //NE
                targetAngleZ = MIN_ANGLE;
                targetAngleX = MAX_ANGLE;
                break;
            case 3: //SE
                targetAngleZ = MIN_ANGLE;
                targetAngleX = MIN_ANGLE;
                break;
            case 4: //MidN               
                targetAngleZ = startAngle;
                targetAngleX = MAX_ANGLE;
                break;
            case 5: //MidS
                targetAngleZ = startAngle;
                targetAngleX = MIN_ANGLE;
                break;
            case 6: //MidW
                targetAngleZ = MAX_ANGLE;
                targetAngleX = startAngle;
                break;
            case 7: //MidE
                targetAngleZ = MIN_ANGLE;
                targetAngleX = startAngle;
                break;
            case 8: //Mid
                targetAngleZ = startAngle;
                targetAngleX = startAngle;
                break;

        }

    }
}
