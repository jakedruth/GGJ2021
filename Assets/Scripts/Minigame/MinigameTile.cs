using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MinigameTile : MonoBehaviour
{
    MinigameManager minigameManager;

    public List<Sprite> tileBaseSpriteList;
    public List<Sprite> tileFloraSpriteList;
    public List<Sprite> tileRockSpriteList;
    public List<Sprite> crackSpriteList;

    public SpriteRenderer baseTileSprite;
    public SpriteRenderer topTileSprite;
    public SpriteRenderer currentCrackSprite;

    public int hitsToBreak = 3;
    public int hitsRemaining;

    void Awake()
    {
        minigameManager = GameObject.Find("Minigame Manager").GetComponent<MinigameManager>();

        hitsRemaining = hitsToBreak;

        //Assign references to the current tile sprite and crack sprite
        baseTileSprite = transform.GetComponent<SpriteRenderer>();
        topTileSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        currentCrackSprite = transform.GetChild(1).GetComponent<SpriteRenderer>();

        //Assign the initial sprite
        AssignSpriteBasedOnMaxHealth(hitsToBreak);
    }
    
    void OnMouseDown()
    {
        switch (minigameManager.tool)
        {
            case MinigameManager.ToolType.Pickaxe:
                UsePickaxe();
                minigameManager.damageIsland(1);
                break;
            case MinigameManager.ToolType.Hammer:
                UseHammer();
                minigameManager.damageIsland(2);
                break;
            case MinigameManager.ToolType.OneDamage:
                Dig(1);
                break;
            case MinigameManager.ToolType.ClearBoard:
                minigameManager.ClearAllTiles();
                break;
            default:
                break;
        }
        //Notify minigame manager to check for treasures/ updates island breaking
    }
    void UsePickaxe()
    {
        Dig(minigameManager.damageMultiplier * 3);
        List<Collider2D> neighbors = new List<Collider2D>();
        //Add the four tiles to the list, in a plus sign to be destroyed
        neighbors.Add(Physics2D.OverlapPoint(transform.position + Vector3.right));
        neighbors.Add(Physics2D.OverlapPoint(transform.position + Vector3.left));
        neighbors.Add(Physics2D.OverlapPoint(transform.position + Vector3.up));
        neighbors.Add(Physics2D.OverlapPoint(transform.position + Vector3.down));


        foreach (Collider2D col in neighbors)
        {
            if (col != null)
            {
                MinigameTile n = col.GetComponent<MinigameTile>();
                if (n != null)
                {
                    n.Dig(minigameManager.damageMultiplier);
                }
            }
        }
    }
    void UseHammer()
    {
        Dig(minigameManager.damageMultiplier * 3);
        List<Collider2D> neighbors = new List<Collider2D>();

        //Add the four adjacent tiles to the list
        neighbors.Add(Physics2D.OverlapPoint(transform.position + Vector3.right));
        neighbors.Add(Physics2D.OverlapPoint(transform.position + Vector3.left));
        neighbors.Add(Physics2D.OverlapPoint(transform.position + Vector3.up));
        neighbors.Add(Physics2D.OverlapPoint(transform.position + Vector3.down));


        foreach (Collider2D col in neighbors)
        {
            if (col != null)
            {
                MinigameTile n = col.GetComponent<MinigameTile>();
                if (n != null)
                {
                    n.Dig(minigameManager.damageMultiplier * 2);
                }
            }
        }
        neighbors.Clear();

        //Add the four corner tiles to the list
        neighbors.Add(Physics2D.OverlapPoint(transform.position + Vector3.right + Vector3.up));
        neighbors.Add(Physics2D.OverlapPoint(transform.position + Vector3.left + Vector3.up));
        neighbors.Add(Physics2D.OverlapPoint(transform.position + Vector3.right + Vector3.down));
        neighbors.Add(Physics2D.OverlapPoint(transform.position + Vector3.left + Vector3.down));


        foreach (Collider2D col in neighbors)
        {
            if (col != null)
            {
                MinigameTile n = col.GetComponent<MinigameTile>();
                if (n != null)
                {
                    n.Dig(minigameManager.damageMultiplier);
                }
            }
        }
    }
    void Dig(int amount)
    {
        hitsRemaining -= amount;
        if (hitsRemaining <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            AssignCrackedSpriteBasedOnDepth();
        }
    }
    void AssignCrackedSpriteBasedOnDepth()
    {
        if (hitsRemaining < hitsToBreak)
        {
            int displayIndex = Mathf.FloorToInt(((float)hitsRemaining / (float)hitsToBreak) * crackSpriteList.Count);
            displayIndex = (crackSpriteList.Count - 1) - displayIndex;

            currentCrackSprite.sprite = crackSpriteList[displayIndex];
        }
    }
    public void AssignSpriteBasedOnMaxHealth(int max)
    {
        hitsRemaining = hitsToBreak = max;

        baseTileSprite.sprite = tileBaseSpriteList[Random.Range(0, tileBaseSpriteList.Count)];
        topTileSprite.sprite = null;

        if (max > 6)
        {
            topTileSprite.sprite = tileRockSpriteList[Random.Range(0, tileRockSpriteList.Count)];
        }
        else if (max > 3)
        {
            topTileSprite.sprite = tileFloraSpriteList[Random.Range(0, tileFloraSpriteList.Count)];
        }
    }
}
