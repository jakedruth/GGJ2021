using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameTileBorder : MonoBehaviour
{
    MinigameManager minigameManager;

    public List<Sprite> tileSpriteList;
    public SpriteRenderer currentTileSprite;

    void Awake()
    {
        minigameManager = GameObject.Find("Minigame Manager").GetComponent<MinigameManager>();

        //Assign references to the current tile sprite
        currentTileSprite = transform.GetComponent<SpriteRenderer>();
    }

    public void AssignSpriteBasedOnNeighbors(int dir)
    {
        currentTileSprite.sprite = tileSpriteList[dir];
    }
}
