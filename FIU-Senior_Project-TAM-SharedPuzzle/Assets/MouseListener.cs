using UnityEngine;
using System.Collections;

//this class is used to allow mouse use in editor
public class MouseListener : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {


        if (Input.GetButtonDown("Fire1"))
        {
            HologramPlacement.Instance.OnSelect();
            Debug.Log("button Clicked");

        }
	}


}
