using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    public enum ToolType
    {
        Pickaxe,
        Hammer,
        OneDamage,
        ClearBoard
    }

    GameObject levelGrid;
    private float distFromCam = -5f;
    public Camera cam;

    [Header("Tile Variables")]
    public string minigameTilePrefabName;
    public string minigameTileBGPrefabName;
    public string minigameTileBorderPrefabName;
    public List<string> minigameTreasurePrefabNames;

    public List<MinigameTile> tiles;
    public List<Treasure> treasures;
    private List<Treasure> treasurePrefabs;

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
    public bool isIslandBroken = false;
    public GameObject finalScore;
    private string scoreMessage = "You Found\n {0}gp\n Worth of\n Treasure!";

    [Header("Treasure Variables")]
    public bool allTreasureUncovered = false;
    [Range(1, 100)]
    public int islandValue;


    void Start()
    {
        levelGrid = new GameObject("Level Grid");
        levelGrid.transform.position = new Vector3(0, 0, distFromCam);

        treasurePrefabs = new List<Treasure>();
        //Load in all the treasure prefabs to instantiate
        foreach (string t in minigameTreasurePrefabNames)
        {
            Treasure resourceTreasure = Resources.Load<Treasure>($"Prefabs/Treasures/{t}");
            treasurePrefabs.Add(resourceTreasure);
        }

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
            if (displayIndex == 0)
                displayIndex++;
            displayIndex = (fracturesList.Count - 1) - displayIndex;

            currentFracture.sprite = fracturesList[displayIndex];
        }
        else if (hitsLeft == hitsTotal)
        {
            currentFracture.enabled = false;
        }
        else
        {
            EndGame();
        }
        CheckForUncoveredTreasure();
    }
    void EndGame()
    {
        isIslandBroken = true;
        currentFracture.sprite = fracturesList[fracturesList.Count - 1];
        hitsLeft = Mathf.Clamp(hitsLeft, 0, hitsTotal);

        finalScore.SetActive(true);
        TMPro.TMP_Text scoreText = finalScore.transform.GetChild(0).GetComponent<TMPro.TMP_Text>();
        scoreText.text = string.Format(scoreMessage, GetCurrentScore().ToString());
    }
    int GetCurrentScore()
    {
        int currentCash = 0;
        foreach (Treasure t in treasures)
        {
            if (!t.isCovered)
            {
                currentCash += (t.value * 10);
            }
        }
        return currentCash;
    }
    public void ClearAllTiles()
    {
        //Clear the grid of all tiles
        foreach (MinigameTile tile in tiles)
        {
            if (tile != null)
            {
                Destroy(tile.gameObject);
            }
        }
        tiles.Clear();
    }
    public void SwitchTools(int _tool)
    {
        tool = (ToolType)_tool;
    }
    void SpawnTreasure()
    {
        int runningValue = islandValue;

        while (runningValue > 0)
        {
            //This makes sure the biggest gem you can spawn never exceeds the total value left for the island
            int currentTreasureValueMax = runningValue > 10 ? 10 : runningValue;

            int treasureValue = Random.Range(1, currentTreasureValueMax + 1);
            Vector2Int coords = new Vector2Int(Random.Range(0, gridWidth), Random.Range(0, gridHeight));

            //Spawn a treasure with a random value between 0 and currentTreasureValueMax
            Treasure tempTreasure = Instantiate(treasurePrefabs[treasureValue - 1], new Vector3(coords.x, coords.y, distFromCam), Quaternion.identity);
            tempTreasure.value = treasureValue;

            tempTreasure.SetPostition(new Vector3(coords.x, coords.y, distFromCam), (treasureValue > 5));

            int attempts = 20;
            while (attempts > 0)
            { 
                if (IsTreasureColliding(tempTreasure))
                {
                    coords = new Vector2Int(Random.Range(0, gridWidth), Random.Range(0, gridHeight));

                    tempTreasure.SetPostition(new Vector3(coords.x, coords.y, distFromCam), (treasureValue > 5));
                }
                else
                {
                    break;
                }
                attempts--;
            }
            if (attempts > 0)
            {
                treasures.Add(tempTreasure);
                runningValue -= tempTreasure.value;
            }
            else
            {
                Destroy(tempTreasure.gameObject);
                runningValue = 0;
            }
        }
    }
    void CheckForUncoveredTreasure()
    {
        allTreasureUncovered = true;

        //Loop through all the treasures
        foreach (Treasure t in treasures)
        {
            //If this treasure isnt uncovered yet, check if it is
            if (t.isCovered)
            {
                bool thisTreasureCovered = false;
                //check the tiles the cover each treasures against the tiles still in the game
                //if none match then the treaure is uncovered
                foreach (Vector3 tilePos in t.tilesCoveringThis)
                {
                    if (Physics2D.OverlapPoint(tilePos, LayerMask.GetMask("Minigame")))
                    {
                        allTreasureUncovered = false;
                        thisTreasureCovered = true;
                        break;
                    }
                }
                if (!thisTreasureCovered)
                {
                    t.isCovered = false;
                }
            }
        }
    }
    
    bool IsTreasureColliding(Treasure treasure)
    {
        bool foundOverlap = false;
        foreach (Treasure t in treasures)
        {
            if (treasure.DoBoxesIntersect(t))
            {
                foundOverlap = true;
            }
        }
        return foundOverlap;
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
        //Reset Island Health
        hitsLeft = hitsTotal;
        isIslandBroken = false;
        currentFracture.enabled = false;

        //Perlin noise map randomization
        offsetX = Random.Range(0f, 99999f);
        offsetY = Random.Range(0f, 99999f);

        //Clear the grid/list of all tiles
        for (int i = 0; i < levelGrid.transform.childCount; i++)
        {
            Destroy(levelGrid.transform.GetChild(i).gameObject);
        }
        tiles.Clear();

        foreach (Treasure treasure in treasures)
        {
            if (treasure != null)
            {
                Destroy(treasure.gameObject);
            }
        }
        treasures.Clear();

        //Load all the necessary resources
        MinigameTile resourceTile = Resources.Load<MinigameTile>($"Prefabs/{minigameTilePrefabName}");
        MinigameTileBorder resourceTileBorder = Resources.Load<MinigameTileBorder>($"Prefabs/{minigameTileBorderPrefabName}");
        GameObject resourceTileBG = Resources.Load<GameObject>($"Prefabs/{minigameTileBGPrefabName}");

        //Generate a new grid of a given size, and make the parent 'grid' for each tile
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                //Destructable tile
                MinigameTile tempTile = Instantiate(resourceTile, new Vector3(x, y, distFromCam), Quaternion.identity);
                tiles.Add(tempTile);
                tempTile.transform.SetParent(levelGrid.transform);

                //Assign the depth based on perlin noise
                tempTile.AssignSpriteBasedOnMaxHealth(CalculateTile(x, y));

                //Background tile
                GameObject tempTileBG = Instantiate(resourceTileBG, new Vector3(x, y, distFromCam), Quaternion.identity);
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
                    MinigameTileBorder tempTileBorder = Instantiate(resourceTileBorder, new Vector3(x, y, distFromCam), Quaternion.identity);
                    tempTileBorder.transform.SetParent(levelGrid.transform);

                    tempTileBorder.AssignSpriteBasedOnNeighbors(GetDirection(x, y));
                }
            }
        }

        //Generate Treasure based on Island Value
        SpawnTreasure();

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
