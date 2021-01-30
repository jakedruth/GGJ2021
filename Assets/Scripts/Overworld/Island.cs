using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Island : MonoBehaviour
{
    public float difficultyLevel;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            OverWorldHUD.instance.ShowPopUp("Press space bar to land on this island", true, "Land", true, OnButtonClickedAction);
    }

    private void OnButtonClickedAction()
    {
        OverWorldHUD.instance.HidePopUp();
    }
}
