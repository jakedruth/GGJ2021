using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour
{
    [Range(1, 10)]
    public int value;

    public Vector2Int position;
    public Vector2Int size;
    // Start is called before the first frame update
    void Awake()
    {
    }
    public bool DoBoxesIntersect(Treasure otherTreasure)
    {
        return (Mathf.Abs(this.position.x - otherTreasure.position.x) * 2 < (this.size.x + otherTreasure.size.x)) &&
               (Mathf.Abs(this.position.y - otherTreasure.position.y) * 2 < (this.size.y + otherTreasure.size.y));
    }
}
