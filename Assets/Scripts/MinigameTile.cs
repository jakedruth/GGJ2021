using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MinigameTile : MonoBehaviour
{
    MinigameManager minigameManager;
    TMP_Text text;

    public List<Sprite> tileSpriteList;
    public List<Sprite> crackSpriteList;
    public Sprite currentTileSprite;
    public Sprite currentCrackSprite;
    public int tileDepthMax = 3;
    public int tileDepthCurrent;

    void Start()
    {
        minigameManager = GameObject.Find("Minigame Manager").GetComponent<MinigameManager>();

        tileDepthCurrent = tileDepthMax;

        //Temporary number display for testing purposes
        text = transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        text.text = tileDepthCurrent.ToString();

        //Assign references to the current tile sprite and crack sprite

        //Assign the initial sprite
    }
    
    void OnMouseDown()
    {
        Dig(minigameManager.pickDamage);
    }

    void Dig(int amount)
    {
        tileDepthCurrent -= amount;
        text.text = tileDepthCurrent.ToString();

        if (tileDepthCurrent <= 0)
            Destroy(gameObject);
    }

    void AssignSpriteBasedOnDepth()
    {

    }
}
