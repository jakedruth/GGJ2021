using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using JetBrains.Annotations;
using Unity.Profiling;
using UnityEngine;

[RequireComponent(typeof(ShipController))]
public class AIInput : MonoBehaviour
{
    private ShipController _shipController;
    private Vector2[] _inputVectors;

    public Transform target;
    public float minRange;
    public float midRange;
    public float maxRange;
    public float changeDirRate;
    public float maxFiringRange;
    public float minFireGradient;
    private Vector2 _input;
    
    public float sinkRate;
    private float _sink;

    void Awake()
    {
        _shipController = GetComponent<ShipController>();
        _shipController.onShipDestroyed.AddListener(OnShipDestroyed);

        _inputVectors = new Vector2[Map.numDirections];
        for (int i = 0; i < Map.numDirections; i++)
        {
            float angle = i * Mathf.PI * 2 / Map.numDirections;
            _inputVectors[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        if (target == null)
            target = FindObjectOfType<PlayerInput>().transform;
    }

    void OnShipDestroyed(ShipController ship)
    {
        _shipController = null;
        _sink = 1;
        GetComponent<CompositeCollider2D>().enabled = false;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (_shipController == null)
        {
            HandleDeath();
            return;
        }

        // Get Direction to target
        Vector2 pos = transform.position;
        Vector2 targetPos = target.position;
        Vector3 delta = targetPos - pos;
        float distance = delta.magnitude;

        // Create Maps
        // Chase Player map
        Map chase = GetChasePlayerMap();
        chase *= delta.sqrMagnitude / (maxRange * maxRange);

        // Circle around the player map
        Map side = GetSideStepPlayer();
        float sideWeight = Mathf.Min(Mathf.Abs(1 + 2f / (midRange - distance)), 2);
        side *= sideWeight;

        // Move Away from the player
        Map away = GetAwayFromPlayer();
        float awayWeight = minRange / distance;
        away *= awayWeight;

        // Prefer moving in the current direction
        Map facing = GetFavorFacing();
        facing *= 0.5f;

        // Try and figure out how to move away form other ships
        Map moveAwayFromShips = MoveAwayFromAllShips();
        for (int i = 0; i < Map.numDirections; i++)
        {
            if (moveAwayFromShips[i] <= -1f)
                moveAwayFromShips[i] = 0;
            else
                moveAwayFromShips[i] = 1;
        }

        // Add up maps as desired
        Map map = (chase + side + away + facing);
        DebugMap(map);

        // Calculate the target Input from the map
        Vector2 targetInput = map.GetStrongest(_inputVectors);
        _input = Vector2.MoveTowards(_input, targetInput, changeDirRate * Time.deltaTime);
        _shipController.HandleMovementInput(_input);

        int sails = Mathf.FloorToInt(4.5f - 3.2f / (1 + Mathf.Pow(-midRange + distance, 2)));
        _shipController.targetSailAmount = (ShipController.Sails) sails;

        // Handle Firing Cannons
        if (distance <= maxFiringRange)
        {
            float dot = Vector3.Dot(transform.right, delta.normalized);
            float pointing = 1f - Mathf.Abs(dot);

            if (pointing > minFireGradient)
            {
                _shipController.FireLeft();
                _shipController.FireRight();
            }
        }
    }

    private void HandleDeath()
    {
        _sink -= sinkRate * Time.deltaTime;
        transform.localScale = Vector3.one * _sink; 
        if (_sink <= 0)
            Destroy(gameObject);
    }

    public void DebugMap(Map map)
    {
        Vector2 pos = transform.position;
        for (int i = 0; i < Map.numDirections; i++)
        {
            float mag = Mathf.Abs(map[i]);
            Color c = map[i] > 0 ? Color.green : Color.red;
            if (map[i] > -0.5f)
                Debug.DrawRay(pos + _inputVectors[i], _inputVectors[i] * mag, c);
        }
    }

    private Map GetChasePlayerMap()
    {
        Map map = new Map();
        Vector2 pos = transform.position;
        Vector2 targetPos = target.position;
        Vector3 delta = targetPos - pos;
        Vector3 direction = delta.normalized;

        for (int i = 0; i < Map.numDirections; i++)
        {
            float dot = Vector3.Dot(_inputVectors[i], direction);
            map[i] = (dot + 1) * 0.5f;
        }

        return map;
    }

    private Map GetSideStepPlayer()
    {
        Map map = new Map();
        Vector2 pos = transform.position;
        Vector2 targetPos = target.position;
        Vector3 delta = targetPos - pos;
        Vector3 direction = delta.normalized;

        for (int i = 0; i < Map.numDirections; i++)
        {
            float dot = Vector3.Dot(_inputVectors[i], direction);
            map[i] = 1f - Mathf.Abs(dot);
        }

        return map;
    }

    private Map GetFavorFacing()
    {
        Map map = new Map();
        Vector2 pos = transform.position;
        Vector2 targetPos = target.position;
        Vector3 delta = targetPos - pos;
        Vector3 direction = delta.normalized;

        for (int i = 0; i < Map.numDirections; i++)
        {
            float dot2 = Vector3.Dot(_inputVectors[i], transform.right);
            map[i] = (dot2 + 1) * 0.5f;
        }

        return map;
    }

    private Map GetAwayFromPlayer()
    {
        Map map = new Map();
        Vector2 pos = transform.position;
        Vector2 targetPos = target.position;
        Vector3 delta = targetPos - pos;
        Vector3 direction = delta.normalized;

        for (int i = 0; i < Map.numDirections; i++)
        {
            float dot = Vector3.Dot(_inputVectors[i], -direction);
            map[i] = 1f - Mathf.Abs(dot - 0.65f);
        }

        return map;
    }

    private Map MoveAwayFromAllShips()
    {
        Map map = new Map();
        Vector2 pos = transform.position;
        ShipController[] ships = FindObjectsOfType<ShipController>();
        foreach (ShipController ship in ships)
        {
            if (ship == _shipController)
                continue;

            if (ship.tag == "Player")
                continue;

            Map shipMap = new Map();
            Vector2 targetPos = ship.transform.position;
            Vector3 delta = targetPos - pos;
            Vector3 direction = delta.normalized;

            for (int i = 0; i < Map.numDirections; i++)
            {
                float dot = Vector3.Dot(_inputVectors[i], -direction);
                dot *= 20 / (1f + delta.magnitude / 6f);
                map[i] = 1 - Mathf.Abs(dot - 0.65f);
            }

            map += shipMap;
        }

        return map;
    }
}

public class Map
{
    public static int numDirections { get; } = 24;
    public float[] weights = new float[numDirections];

    public Map()
    {
        SetAll(0);
    }

    public Map(float @default)
    {
        SetAll(@default);
    }

    public Vector2 GetStrongest(Vector2[] dir)
    {
        int bestIndex = -1;
        float bestWeight = -1;
        for (int i = 0; i < numDirections; i++)
        {
            if (weights[i] > bestWeight)
            {
                bestWeight = weights[i];
                bestIndex = i;
            }
        }

        return dir[bestIndex] * weights[bestIndex];
    }

    public Vector2 GetAveraged(Vector2[] dir)
    {
        Vector2 result = Vector2.zero;

        for (int i = 0; i < weights.Length; i++)
        {
            result += dir[i] * weights[i] * 1f / numDirections;
        }

        return result;
    }

    public Map SetAll(float value)
    {
        for (int i = 0; i < numDirections; i++)
            weights[i] = value;

        return this;
    }

    public Map Normalize()
    {
        float best = 0f;
        for (int i = 0; i < numDirections; i++)
        {
            float abs = Mathf.Abs(weights[i]);
            if (abs > best)
                best = abs;
        }

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (best != 0f)
            this.Scale(1f / best);

        return this;
    }

    public Map Scale(float value)
    {
        for (int i = 0; i < numDirections; i++)
            weights[i] *= value;

        return this;
    }

    public Map Min(float min)
    {
        for (int i = 0; i < numDirections; i++)
            weights[i] = Mathf.Min(min, weights[i]);

        return this;
    }

    public Map Max(float max)
    {
        for (int i = 0; i < numDirections; i++)
            weights[i] = Mathf.Max(max, weights[i]);

        return this;
    }

    public Map Clamp(float min, float max)
    {
        for (int i = 0; i < numDirections; i++)
            weights[i] = Mathf.Clamp(weights[i], min, max);

        return this;
    }

    public float this[int key]
    {
        get { return weights[key]; }
        set { weights[key] = value; }
    }

    public static Map operator +(Map a, Map b)
    {
        Map newMap = new Map();
        for (int i = 0; i < numDirections; i++) 
            newMap[i] = a[i] + b[i];

        return newMap;
    }

    public static Map operator -(Map a, Map b)
    {
        Map newMap = new Map();
        for (int i = 0; i < numDirections; i++) 
            newMap[i] = a[i] - b[i];

        return newMap;
    }

    public static Map operator *(Map a, Map b)
    {
        Map newMap = new Map();
        for (int i = 0; i < numDirections; i++) 
            newMap[i] = a[i] * b[i];

        return newMap;
    }

    public static Map operator /(Map a, Map b)
    {
        Map newMap = new Map();
        for (int i = 0; i < numDirections; i++) 
            newMap[i] = a[i] / b[i];

        return newMap;
    }

    public static Map operator +(Map map, float t)
    {
        Map newMap = new Map();
        for (int i = 0; i < numDirections; i++) 
            newMap[i] = map[i] + t;

        return newMap;
    }

    public static Map operator -(Map map, float t)
    {
        Map newMap = new Map();
        for (int i = 0; i < numDirections; i++) 
            newMap[i] = map[i] - t;

        return newMap;
    }

    public static Map operator *(Map map, float t)
    {
        Map newMap = new Map();
        for (int i = 0; i < numDirections; i++) 
            newMap[i] = map[i] * t;

        return newMap;
    }

    public static Map operator /(Map map, float t)
    {
        Map newMap = new Map();
        for (int i = 0; i < numDirections; i++) 
            newMap[i] = map[i] / t;

        return newMap;
    }

    public static Map Lerp(Map a, Map b, float t)
    {
        Map result = new Map();
        for (int i = 0; i < numDirections; i++) 
            result[i] = a[i] * (1 - t) + b[i] * t;

        return result;
    }
}
