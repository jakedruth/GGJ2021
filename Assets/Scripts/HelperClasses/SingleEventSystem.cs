using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleEventSystem : UnityEngine.EventSystems.EventSystem
{
    protected override void Awake()
    {
        if (FindObjectsOfType<SingleEventSystem>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        base.Awake();
    }
}
