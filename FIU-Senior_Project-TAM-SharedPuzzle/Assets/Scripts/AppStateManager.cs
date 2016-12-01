using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

/// <summary>
/// Keeps track of the current state of the experience.
/// </summary>
public class AppStateManager : Singleton<AppStateManager>
{

    /// <summary>
    /// Enum to track progress through the experience.
    /// </summary>
    public enum AppState
    {
        Starting = 0,
        WaitingForAnchor,
        WaitingForStageTransform,
        StageTransformSet,
        Ready,
        InGame,
        EndGame,
        GameOver
    }

    /// <summary>
    /// Tracks the current state in the experience.
    /// </summary>
    public AppState CurrentAppState { get; set; }


    /// <summary>
    ///Use this for initialization
    /// </summary>
    void Start () {

        // We start in the 'waiting for anchor' mode.
        CurrentAppState = AppState.WaitingForAnchor;

        //used during testing
      // CurrentAppState = AppState.WaitingForStageTransform;
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update() {

        switch (CurrentAppState)
        {
            case AppState.WaitingForAnchor:
                // Once the anchor is established we need to run spatial mapping for a 
                // little while to build up some meshes.
                if (ImportExportAnchorManager.Instance.AnchorEstablished)
                {
                    GameObject.Find("UIManager").GetComponent<TextEdit>().ChangeText("Choose Difficulty");
                    CurrentAppState = AppState.WaitingForStageTransform;
                    //GestureManager.Instance.OverrideFocusedObject = HologramPlacement.Instance.gameObject;

                    SpatialMappingManager.Instance.gameObject.SetActive(true);
                    SpatialMappingManager.Instance.DrawVisualMeshes = true;
                 //   SpatialMappingDeformation.Instance.ResetGlobalRendering();
                    SpatialMappingManager.Instance.StartObserver();
                }
                break;
            case AppState.WaitingForStageTransform:
                if (GameObject.Find("GameController").GetComponent<GameController>().getisDiffSetup())
                {
                    GameObject.Find("UIManager").GetComponent<TextEdit>().ChangeText("Air Tap to Place");
                    GestureManager.Instance.OverrideFocusedObject = HologramPlacement.Instance.gameObject;
                }


                // Now if we have the stage transform we are ready to go.
                if (HologramPlacement.Instance.GotTransform)
                {
                    CurrentAppState = AppState.Ready;
                   
                }
                break;
            case AppState.Ready:
                StartCoroutine(GameObject.Find("GameController").GetComponent<GameController>().StartRound());
                CurrentAppState = AppState.InGame;
                break;
            case AppState.EndGame:
                break;
        }
    }
}
