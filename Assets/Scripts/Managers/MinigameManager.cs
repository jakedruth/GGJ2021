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
    public GameObject tileBGTemplate;
    public GameObject tileBorderTemplate;

    public GameObject grid;
    public List<GameObject> tiles;
    public List<GameObject> tilesBG;
    public List<GameObject> tilesBorder;
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
    public void SwitchTools(int _tool)
    {
        tool = (ToolType)_tool;
    }
    void GenerateLevel()
    {
        //Clear the grid of all tiles
        foreach (GameObject tile in tiles)
        {
            Destroy(tile);
        }
        tiles.Clear();

        //Clear the grid of all tiles
        foreach (GameObject tile in tilesBG)
        {
            Destroy(tile);
        }
        tilesBG.Clear();

        //Generate a new grid of a given size, and make the parent 'grid' for each tile
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (!isBorderTile(x, y))
                {
                    //Destructable tile
                    GameObject tempTile = Instantiate<GameObject>(tileTemplate);
                    tempTile.transform.position = new Vector3(x, y);
                    tiles.Add(tempTile);
                    tempTile.transform.SetParent(grid.transform);

                    //Background tile
                    GameObject tempTileBG = Instantiate<GameObject>(tileBGTemplate);
                    tempTileBG.transform.position = new Vector3(x, y);
                    tilesBG.Add(tempTileBG);
                    tempTileBG.transform.SetParent(grid.transform);
                }
                else
                {
                    //Border tile
                    GameObject tempTileBorder = Instantiate<GameObject>(tileBorderTemplate);
                    tempTileBorder.transform.position = new Vector3(x, y);
                    tilesBG.Add(tempTileBorder);
                    tempTileBorder.transform.SetParent(grid.transform);

                    MinigameTileBorder border = tempTileBorder.GetComponent<MinigameTileBorder>();
                    border.AssignSpriteBasedOnNeighbors(GetDirection(x, y));
                }
            }
        }

        //Center the camera to the middle of whatever size grid was generated
        Camera.main.transform.position = new Vector3((gridWidth * .5f) - .5f, (gridHeight * .5f) - .5f, -10);
        //Change camera size to show the entire grid
        Camera.main.orthographicSize = gridWidth > gridHeight ? gridWidth - 2 : gridHeight - 2;
    }

    bool isBorderTile(int x, int y)
    {
        return ((x == 0) || (y == 0) || (x == gridWidth - 1) || (y == gridHeight - 1));
    }
    int GetDirection(int x, int y)
    {
        int dir = 8;

        if ((x == 0) && (y == gridHeight - 1))//Top Left
        {
            dir = 0;
        }
        else if (((x > 0) && (x < gridWidth - 1)) && (y == gridHeight - 1))//Top
        {
            dir = 1;
        }
        else if ((x == gridWidth - 1) && (y == gridHeight - 1))//Top Right
        {
            dir = 2;
        }
        else if ((x == 0) && ((y > 0) && (y < gridHeight - 1)))//Left
        {
            dir = 3;
        }
        else if ((x == gridWidth - 1) && ((y > 0) && (y < gridHeight - 1)))//Right
        {
            dir = 4;
        }
        else if ((x == 0) && (y == 0))//Bottom Left
        {
            dir = 5;
        }
        else if (((x > 0) && (x < gridWidth - 1)) && (y == 0))//Bottom
        {
            dir = 6;
        }
        else if ((x == gridWidth - 1) && (y == 0))//Bottom Right
        {
            dir = 7;
        }

        return dir;
    }
}
