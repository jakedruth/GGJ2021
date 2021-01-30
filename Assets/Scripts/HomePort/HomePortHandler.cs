using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomePortHandler : MonoBehaviour
{
    void Start()
    {
        
    }

    public void OnSetSail()
    {
        OverWorldHandler owh = FindObjectOfType<OverWorldHandler>();
        if (owh != null)
            owh.LeaveIsland(this);
        else 
            Debug.LogError("The over world was never found");
    }
}
