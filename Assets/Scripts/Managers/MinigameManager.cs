using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    public enum ToolType
    {
        Pickaxe,
        Hammer,
        OneDamage
    }

    GameObject levelGrid;
    public Camera cam;

    [Header("Tile Variables")]
    public string minigameTilePrefabName;
    public string minigameTileBGPrefabName;
    public string minigameTileBorderPrefabName;
    public string minigameTreasurePrefabName;

    public List<MinigameTile> tiles;
    public List<Treasure> treasures;

    [Header("Perlin Noise Adjustment")]
    [Range(1f, 20f)]
    public int gridWidth = 10;
    [Range(1f, 20f)]
    public int gridHeight = 10;

    public float scale = 4f;

    public float grassWeighted = 1f;
    public float bushWeighted = 1f;
    public float rockWeighted = 1f;

    private float offsetX = 100f;
    private float offsetY = 100f;

    [Header("Tool Variables")]
    public ToolType tool = ToolType.Pickaxe;
    public int damageMultiplier = 1;

    [Header("Island Variables")]
    public List<Sprite> fracturesList;
    public Image currentFracture;
    public int hitsTotal = 20;
    private int hitsLeft = 20;
    [Range(1, 40)]
    public int islandValue;


    void Start()
    {
        levelGrid = new GameObject("Level Grid");
        GenerateLevel();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateLevel();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            damageIsland(1);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            damageIsland(-1);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SpawnTreasure();
        }
    }
    public void OnSetSail()
    {
        OverWorldHandler owh = FindObjectOfType<OverWorldHandler>();
        if (owh != null)
            owh.LeaveIsland(this);
        else
            Debug.LogError("The over world was never found");
    }
    public void damageIsland(int damage)
    {
        hitsLeft -= damage;
        if ((hitsLeft >= 0) && (hitsLeft < hitsTotal))
        {
            currentFracture.enabled = true;

            int displayIndex = Mathf.FloorToInt(((float)hitsLeft / (float)hitsTotal) * fracturesList.Count);
            displayIndex = (fracturesList.Count - 1) - displayIndex;

            currentFracture.sprite = fracturesList[displayIndex];
        }
        else if (hitsLeft == hitsTotal)
        {
            currentFracture.enabled = false;
        }
        else
        {
            hitsLeft = Mathf.Clamp(hitsLeft, 0, hitsTotal);
        }
    }
    public void SwitchTools(int _tool)
    {
        tool = (ToolType)_tool;
    }
    void SpawnTreasure()
    {
        Treasure resourceTreasure = Resources.Load<Treasure>($"Prefabs/{minigameTreasurePrefabName}");

        int runningValue = islandValue;

        while (runningValue > 0)
        {
            int currentTreasureValueMax = runningValue > 10 ? 10 : runningValue;
            int xCoord = Random.Range(0, gridWidth);
            int yCoord = Random.Range(0, gridHeight);

            //Spawn a treasure with a random value between 0 and currentTreasureValueMax
            Treasure tempTreasure = Instantiate(resourceTreasure, new Vector3(xCoord, yCoord), Quaternion.identity);
            treasures.Add(tempTreasure);

            runningValue -= currentTreasureValueMax;
        }
    }
    int CalculateTile(int x, int y)
    {
        float xCoord = (float)x / gridWidth * scale + offsetX;
        float yCoord = (float)y / gridHeight * scale + offsetY;

        float sample = Mathf.PerlinNoise(xCoord, yCoord);

        //Calculate weights of spawning different strength tiles
        float totalWeight = grassWeighted + bushWeighted + rockWeighted;
        float grassThreshold = grassWeighted / totalWeight;
        float bushThreshold = (bushWeighted / totalWeight) + grassThreshold;

        int depth = 0;
        if (sample < grassThreshold)
        {
            depth = 3;
        }
        else if (sample < bushThreshold)
        {
            depth = 6;
        }
        else
        {
            depth = 10;
        }

        return depth;
    }
    void GenerateLevel()
    {
        //Reset fracture bar
        hitsLeft = hitsTotal;

        //Perlin noise map randomization
        offsetX = Random.Range(0f, 99999f);
        offsetY = Random.Range(0f, 99999f);

        //Clear the grid of all tiles
        for (int i = 0; i < levelGrid.transform.childCount; i++)
        {
            Destroy(levelGrid.transform.GetChild(i).gameObject);
        }
        tiles.Clear();
        
        MinigameTile resourceTile = Resources.Load<MinigameTile>($"Prefabs/{minigameTilePrefabName}");
        MinigameTileBorder resourceTileBorder = Resources.Load<MinigameTileBorder>($"Prefabs/{minigameTileBorderPrefabName}");
        GameObject resourceTileBG = Resources.Load<GameObject>($"Prefabs/{minigameTileBGPrefabName}");

        //Generate a new grid of a given size, and make the parent 'grid' for each tile
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                //Destructable tile
                MinigameTile tempTile = Instantiate(resourceTile, new Vector3(x, y), Quaternion.identity);
                tiles.Add(tempTile);
                tempTile.transform.SetParent(levelGrid.transform);

                //Assign the depth based on perlin noise
                tempTile.AssignSpriteBasedOnMaxHealth(CalculateTile(x, y));

                //Background tile
                GameObject tempTileBG = Instantiate(resourceTileBG, new Vector3(x, y), Quaternion.identity);
                tempTileBG.transform.SetParent(levelGrid.transform);
            }
        }
        //Generate a border for the new grid
        for (int y = -1; y < gridHeight + 1; y++)
        {
            for (int x = -1; x < gridWidth + 1; x++)
            {
                if (isBorderTile(x, y))
                {
                    //Border tile
                    MinigameTileBorder tempTileBorder = Instantiate(resourceTileBorder, new Vector3(x, y), Quaternion.identity);
                    tempTileBorder.transform.SetParent(levelGrid.transform);

                    tempTileBorder.AssignSpriteBasedOnNeighbors(GetDirection(x, y));
                }
            }
        }

        //Center the camera to the middle of whatever size grid was generated
        cam.transform.position = new Vector3((gridWidth * .5f) - .5f, (gridHeight * .5f) - .5f, -10);
        //Change camera size to show the entire grid
        cam.orthographicSize = gridWidth > gridHeight ? gridWidth - 2 : gridHeight - 2;
    }

    bool isBorderTile(int x, int y)
    {
        return ((x == -1) || (y == -1) || (x == gridWidth) || (y == gridHeight));
    }
    int GetDirection(int x, int y)
    {
        int dir = 8;

        if ((x == -1) && (y == gridHeight))//Top Left
        {
            dir = 0;
        }
        else if (((x > -1) && (x < gridWidth)) && (y == gridHeight))//Top
        {
            dir = 1;
        }
        else if ((x == gridWidth) && (y == gridHeight))//Top Right
        {
            dir = 2;
        }
        else if ((x == -1) && ((y > -1) && (y < gridHeight)))//Left
        {
            dir = 3;
        }
        else if ((x == gridWidth) && ((y > -1) && (y < gridHeight)))//Right
        {
            dir = 4;
        }
        else if ((x == -1) && (y == -1))//Bottom Left
        {
            dir = 5;
        }
        else if (((x > -1) && (x < gridWidth)) && (y == -1))//Bottom
        {
            dir = 6;
        }
        else if ((x == gridWidth) && (y == -1))//Bottom Right
        {
            dir = 7;
        }

        return dir;
    }
}
