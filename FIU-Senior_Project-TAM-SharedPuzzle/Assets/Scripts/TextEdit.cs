using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextEdit : MonoBehaviour
{

    [SerializeField]
    private Text text = null;

    string s1 = "Hello World";

    // Use this for initialization
    void Start()
    {
        text.text = s1;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeText(string msg)
    {
        
            text.text = msg;
    }

}
