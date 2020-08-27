using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOnUI : MonoBehaviour
{
    public GameObject solarSystemUI;
    private bool haveClicked;
    // Start is called before the first frame update
    void Start()
    {
        haveClicked = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {        
        if(!haveClicked)
        {
            solarSystemUI.SetActive(true);
            haveClicked = true;
        }
        else
        {
            solarSystemUI.SetActive(false);
            haveClicked = false;
        }
    }
}
