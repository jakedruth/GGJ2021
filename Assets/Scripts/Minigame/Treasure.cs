using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour
{
    public List<Sprite> sprites;
    public SpriteRenderer currentSprite;

    [Range(1, 10)]
    public int value;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void AssignSpriteBasedOnValue()
    {
        currentSprite.sprite = sprites[value - 1];
        currentSprite.transform.localScale.Set(1f, 1f, 1f);
        if (value > (sprites.Count / 2))
        {
            currentSprite.transform.localScale *= 2; 
        }
    }
}
