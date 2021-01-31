using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour
{
    private MinigameManager minigameManager;
    [Range(1, 10)]
    public int value;
    public List<Vector3> tilesCoveringThis;
    public bool isCovered = true;

    public Vector2Int position;
    public Vector2Int size;
    // Start is called before the first frame update
    void Awake()
    {
        minigameManager = GameObject.Find("Minigame Manager").GetComponent<MinigameManager>();
        tilesCoveringThis = new List<Vector3>();
        RandomlyRotate();
    }
    void RandomlyRotate()
    {
        transform.GetChild(0).localRotation = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward);
    }
    public bool DoBoxesIntersect(Treasure otherTreasure)
    {
        return (Mathf.Abs(this.position.x - otherTreasure.position.x) * 2 < (this.size.x + otherTreasure.size.x)) &&
               (Mathf.Abs(this.position.y - otherTreasure.position.y) * 2 < (this.size.y + otherTreasure.size.y));
    }
    public void SetPostition(Vector3 newPos, bool isBig)
    {
        this.position = new Vector2Int((int)newPos.x, (int)newPos.y);
        tilesCoveringThis.Clear();

        if (isBig)
        {
            this.position = AdjustedCoords(this);
            tilesCoveringThis.Add(transform.position);
            tilesCoveringThis.Add(new Vector3(position.x + 1, position.y, transform.position.z));
            tilesCoveringThis.Add(new Vector3(position.x, position.y + 1, transform.position.z));
            tilesCoveringThis.Add(new Vector3(position.x + 1, position.y + 1, transform.position.z));
        }
        else
        {
            tilesCoveringThis.Add(transform.position);
        }
        
        transform.position = new Vector3(position.x, position.y, newPos.z);
    }
    Vector2Int AdjustedCoords(Treasure treasure)
    {
        int x = treasure.position.x;
        int y = treasure.position.y;

        if (treasure.position.x == minigameManager.gridWidth - 1)
            x--;
        if (treasure.position.y == minigameManager.gridHeight - 1)
            y--;

        return new Vector2Int(x, y);
    }
}
