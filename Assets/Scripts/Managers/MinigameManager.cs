using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    public enum ToolType
    {
        Pickaxe,
        Hammer
    }
    public GameObject tileTemplate;

    public GameObject grid;
    public List<GameObject> tiles;
    //public List<Treasure> treasures;

    public int gridWidth = 10;
    public int gridHeight = 10;

    public float scale = 10f;
    public float offsetX = 100f;
    public float offsetY = 100f;

    public ToolType tool = ToolType.Pickaxe;
    public int pickDamage = 1;
    public int hitsLeft = 20;


    void Start()
    {
        grid = new GameObject("Grid");
        GenerateLevel();
    }

    void Update()
    {
        //Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateLevel();
        }
    }
    public void SwitchTools(string _tool)
    {
        Debug.Log("Switching to" + _tool);
        switch (_tool)
        {
            case "Pickaxe":
                tool = ToolType.Pickaxe;
                break;
            case "Hammer":
                tool = ToolType.Hammer;
                break;
            default:
                break;
        }
    }
    void GenerateLevel()
    {
        //Clear the grid of all tiles
        foreach (GameObject tile in tiles)
        {
            Destroy(tile);
        }
        tiles.Clear();

        //Generate a new grid of a given size, and make the parent 'grid' for each tile
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                GameObject tempTile = Instantiate<GameObject>(tileTemplate);
                tempTile.transform.position = new Vector3(x, y);
                tiles.Add(tempTile);
                tempTile.transform.SetParent(grid.transform);
            }
        }

        //Center the camera to the middle of whatever size grid was generated
        Camera.main.transform.position = new Vector3((gridWidth * .5f) - .5f, (gridHeight * .5f) - .5f, -10);
        //Change camera size to show the entire grid
        Camera.main.orthographicSize = gridWidth > gridHeight ? gridWidth : gridHeight;
    }
}
