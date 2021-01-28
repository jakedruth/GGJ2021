using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameTile : MonoBehaviour
{
    public Sprite sprite;
    int tileDepth = 1;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void Dig(int amount)
    {
        tileDepth -= amount;

        if (tileDepth <= 0)
            Destroy(this);
    }
}
