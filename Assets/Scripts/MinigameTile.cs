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
    public SpriteRenderer currentTileSprite;
    public SpriteRenderer currentCrackSprite;
    public int hitsMax = 3;
    public int hitsRemaining;

    void Start()
    {
        minigameManager = GameObject.Find("Minigame Manager").GetComponent<MinigameManager>();

        hitsRemaining = hitsMax;

        //Assign references to the current tile sprite and crack sprite
        currentTileSprite = transform.GetComponent<SpriteRenderer>();
        currentCrackSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();

        //Assign the initial sprite
    }
    
    void OnMouseDown()
    {
        switch (minigameManager.tool)
        {
            case MinigameManager.ToolType.Pickaxe:
                UsePickaxe();
                break;
            case MinigameManager.ToolType.Hammer:
                UseHammer();
                break;
            default:
                break;
        }
        //Notify minigame manager to check for treasures/ updates island breaking
    }
    void UsePickaxe()
    {
        Dig(minigameManager.pickDamage * 3);
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
                    n.Dig(minigameManager.pickDamage);
                }
            }
        }
    }

    void UseHammer()
    {
        Dig(minigameManager.pickDamage * 2);
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
                    n.Dig(minigameManager.pickDamage * 2);
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
                    n.Dig(minigameManager.pickDamage);
                }
            }
        }
    }

    void Dig(int amount)
    {
        hitsRemaining -= amount;
        if (hitsRemaining < 0)
            Destroy(gameObject);

        AssignSpriteBasedOnDepth();
    }

    void AssignSpriteBasedOnDepth()
    {
        if (hitsRemaining < hitsMax)
        {
            int displayIndex = Mathf.RoundToInt(((float)hitsRemaining / (float)hitsMax) * crackSpriteList.Count);
            displayIndex = (crackSpriteList.Count - 1) - displayIndex;
            if (displayIndex >= crackSpriteList.Count)
                displayIndex = crackSpriteList.Count - 1;

            currentCrackSprite.sprite = crackSpriteList[displayIndex];
        }
    }
}
