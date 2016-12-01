using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Windows.Speech;
using HoloToolkit.Unity;
using HoloToolkit.Sharing;

public class HologramPlacement : Singleton<HologramPlacement>
{
    /// <summary>
    /// Tracks if we have been sent a transform for the model.
    /// The model is rendered relative to the actual anchor.
    /// </summary>
    public bool GotTransform { get; private set; }

    /// <summary>
    /// When the experience starts, we disable all of the rendering of the model.
    /// </summary>
    List<MeshRenderer> disabledRenderers = new List<MeshRenderer>();

    /// <summary>
    /// We use a voice command to enable moving the target.
    /// </summary>
    KeywordRecognizer keywordRecognizer;

    private GameObject gObjBall; //for the ball placement

    void Start()
    {

        gObjBall = GameObject.Find("BallMarker");

        // When we first start, we need to disable the model to avoid it obstructing the user picking a hat.
        //we will disable the puzzle to not give them a headstart
        //DisableModel();

        // We care about getting updates for the model transform.
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.StageTransform] = this.OnStageTransfrom;

        // And when a new user join we will send the model transform we have.
        SharingSessionTracker.Instance.SessionJoined += Instance_SessionJoined;

        // And if the users want to reset the stage transform.
        //might use this
        //CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.ResetStage] = this.OnResetStage;

        // Setup a keyword recognizer to enable resetting the game.
        /*
        List<string> keywords = new List<string>();
        keywords.Add("Reset Game");
        keywordRecognizer = new KeywordRecognizer(keywords.ToArray());
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();
        */


       // GestureManager.Instance.OverrideFocusedObject = this.gameObject;
        
        
        //prob not needed
        /*if (GestureManager.Instance == null)
        {
            OnSelect();
        }*/

    }

    /// <summary>
    /// When the keyword recognizer hears a command this will be called.  
    /// In this case we only have one keyword, which will re-enable moving the 
    /// target.
    /// </summary>
    /// <param name="args">information to help route the voice command.</param>
    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
       // ResetStage();
    }

    /// <summary>
    /// Resets the stage transform, so users can place the target again.
    /// </summary>
    /*public void ResetStage()
    {
        GotTransform = false;

        // AppStateManager needs to know about this so that
        // the right objects get input routed to them.
        AppStateManager.Instance.ResetStage();

        // Other devices in the experience need to know about this as well.
        CustomMessages.Instance.SendResetStage();

        // And we need to reset the object to its start animation state.
        GetComponent<EnergyHubBase>().ResetAnimation();
    }*/

    /// <summary>
    /// When a new user joins we want to send them the relative transform for the model if we have it.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Instance_SessionJoined(object sender, SharingSessionTracker.SessionJoinedEventArgs e)
    {
        if (GotTransform)
        {
         //   CustomMessages.Instance.SendStageTransform(transform.localPosition, transform.localRotation);
          CustomMessages.Instance.SendStageTransform(transform.localPosition);
        }
    }

    /// <summary>
    /// Turns off all renderers for the model.
    /// </summary>
    void DisableModel()
    {
        foreach (MeshRenderer renderer in gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            if (renderer.enabled)
            {
                renderer.enabled = false;
                disabledRenderers.Add(renderer);
            }
        }

        foreach (MeshCollider collider in gameObject.GetComponentsInChildren<MeshCollider>())
        {
            collider.enabled = false;
        }
    }

    /// <summary>
    /// Turns on all renderers that were disabled.
    /// </summary>
    void EnableModel()
    {
        foreach (MeshRenderer renderer in disabledRenderers)
        {
            renderer.enabled = true;
        }

        foreach (MeshCollider collider in gameObject.GetComponentsInChildren<MeshCollider>())
        {
            collider.enabled = true;
        }

        disabledRenderers.Clear();
    }


    void Update()
    {
        // Wait till users have anchors
        if (ImportExportAnchorManager.Instance.AnchorEstablished && GameObject.Find("GameController").GetComponent<GameController>().getisDiffSetup())//GotTransform
        {
                // After which we want to start rendering.
                //EnableModel();

                // And if we've already been sent the relative transform, we will use it.
                if (GotTransform)
                {
                // This triggers the animation sequence for the model and 
                // puts the cool materials on the model.
                // GetComponent<EnergyHubBase>().SendMessage("OnSelect");
                //Debug.Log("Position set.");



                //here we need to start teh game loop
                //if (IsTargetVisible())
                //  {
                // HolographicSettings.SetFocusPointForFrame(gameObject.transform.position, -Camera.main.transform.forward);

                //Debug.Log("Hi");
                // AppStateManager.Instance.CurrentAppState = AppStateManager.AppState.StageTransformSet;

                //  }
            }else
            {
                transform.position = Vector3.Lerp(transform.position, ProposeTransformPosition(), 0.2f);
            }
            

        }
        else if (GotTransform == false)
        {
            //AppStateManager.Instance.CurrentAppState = AppStateManager.AppState.WaitingForStageTransform;
            
        }
    }

    private bool IsTargetVisible()
    {
        // This will return true if the target's mesh is within the Main Camera's view frustums.
        Vector3 targetViewportPosition = Camera.main.WorldToViewportPoint(gameObject.transform.position);
        return (targetViewportPosition.x > 0.0 && targetViewportPosition.x < 1 &&
            targetViewportPosition.y > 0.0 && targetViewportPosition.y < 1 &&
            targetViewportPosition.z > 0);
    }


    Vector3 ProposeTransformPosition()
    {
        Vector3 retval;

        //this is if we want to place it between them
        // We need to know how many users are in the experience with good transforms.
        /*
        Vector3 cumulatedPosition = Camera.main.transform.position;
        int playerCount = 1;
        foreach (RemotePlayerManager.RemoteHeadInfo remoteHead in RemotePlayerManager.Instance.remoteHeadInfos)
        {
            if (remoteHead.Anchored && remoteHead.Active)
            {
                playerCount++;
                cumulatedPosition += remoteHead.HeadObject.transform.position;
            }
        }
        */
        retval = Camera.main.transform.position + Camera.main.transform.forward * 2;
        //float offset = .7f; //change to determine how close you want the maze to appear to players head
        //retval = new Vector3(retval.x, retval.y - offset, retval.z); 

        /* 

         // If we have more than one player ...
         if (playerCount > 1)
         {
             // Put the transform in between the players.
             retval = cumulatedPosition / playerCount;
             RaycastHit hitInfo;

             // And try to put the transform on a surface below the midpoint of the players.
             if (Physics.Raycast(retval, Vector3.down, out hitInfo, 5, SpatialMappingManager.Instance.LayerMask))
             {
                 retval = hitInfo.point;
             }
         }
         // If we are the only player, have the model act as the 'cursor' ...
         else
         {
             // We prefer to put the model on a real world surface.
             RaycastHit hitInfo;

             if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 30, SpatialMappingManager.Instance.LayerMask))
             {
                 retval = hitInfo.point;
             }
             else
             {
                 // But if we don't have a ray that intersects the real world, just put the model 2m in
                 // front of the user.
                 retval = Camera.main.transform.position + Camera.main.transform.forward * 2;
             }
         }

         */

        return retval;
    }

    public void OnSelect()
    {
        // Note that we have a transform.
        GotTransform = true;

        // And send it to our friends.
        //CustomMessages.Instance.SendStageTransform(transform.localPosition, transform.localRotation);
        CustomMessages.Instance.SendStageTransform(transform.localPosition);
    }

    /// <summary>
    /// When a remote system has a transform for us, we'll get it here.
    /// </summary>
    /// <param name="msg"></param>
    void OnStageTransfrom(NetworkInMessage msg)
    {
        // We read the user ID but we don't use it here.
        msg.ReadInt64();

        transform.localPosition = CustomMessages.Instance.ReadVector3(msg);
        //transform.localRotation = CustomMessages.Instance.ReadQuaternion(msg);

        // The first time, we'll want to send the message to the model to do its animation and
        // swap its materials.
        //if (disabledRenderers.Count == 0 && GotTransform == false)
        // {
        //    GetComponent<EnergyHubBase>().SendMessage("OnSelect");
        //}
        GameObject.Find("Ball").transform.position = gObjBall.transform.position;
        GotTransform = true;
    }

    /// <summary>
    /// When a remote system has a transform for us, we'll get it here.
    /// </summary>
  /*  void OnResetStage(NetworkInMessage msg)
    {
        GotTransform = false;

        //GetComponent<EnergyHubBase>().ResetAnimation();
        AppStateManager.Instance.ResetStage();
    }*/
}