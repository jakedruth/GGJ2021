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

    void Awake()
    {
        _shipController = GetComponent<ShipController>();
        _inputVectors = new Vector2[Map.numDirections];
        for (int i = 0; i < Map.numDirections; i++)
        {
            float angle = i * Mathf.PI * 2 / Map.numDirections;
            _inputVectors[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

    }
    
    // Update is called once per frame
    void Update()
    {
        Vector2 pos = transform.position;
        Vector2 targetPos = target.position;
        Vector3 delta = targetPos - pos;
        Vector3 direction = delta.normalized;

        Map chase = GetChasePlayerMap();

        Map side = GetSideStepPlayer();
        float sideWeight = Mathf.Clamp(midRange / delta.magnitude, 0, 4);
        side *= sideWeight;

        Map away = GetAwayFromPlayer();
        float awayWeight = minRange / delta.magnitude;
        away *= awayWeight;

        Map facing = GetFavorFacing();

        Map map = (chase + side + away + facing);
        DebugMap(map);

        Vector2 input = map.GetStrongest(_inputVectors);
        _shipController.HandleMovementInput(input);


        //Vector2 perpendicular = Vector3.Cross(direction, Vector3.forward) * minRange;
        //Debug.DrawLine(pos, targetPos + perpendicular);
        //Vector2 newDir = (targetPos + perpendicular) - pos;
        //_shipController.HandleMovementInput(newDir.normalized);

        //Map inputMap = GetSideStepPlayer();
        //if (delta.sqrMagnitude < maxRange * maxRange && delta.sqrMagnitude > midRange * midRange)
        //{
        //    float t = 0.5f;
        //    inputMap = Map.Lerp(GetChasePlayerMap(), GetSideStepPlayer(), t);
        //}
        //else if (delta.sqrMagnitude < midRange * midRange&& delta.sqrMagnitude > minRange * minRange)
        //{
        //    float t = 0.5f;
        //    inputMap = Map.Lerp(GetSideStepPlayer(), GetAwayFromPlayer(), t);
        //} 
        //else if (delta.sqrMagnitude < minRange * minRange)
        //{
        //    inputMap = GetAwayFromPlayer();
        //}

        //Vector2 input = inputMap.GetStrongest(_inputVectors);

        //_shipController.HandleMovementInput(input);
        //inputMap.Debug(transform.position, _inputVectors);

        //float[] map = GetChasePlayerMap();

        //for (int i = 0; i < Map.numDirections; i++)
        //{
        //    //Color c = Color.Lerp(Color.red, Color.green, (_inputStrength[i] + 1) * 0.5f);
        //    Debug.DrawRay(pos + _inputVectors[i], _inputVectors[i] * map[i] * 2, Color.green);
        //}

        //Vector2 moveInput = AverageMap(map);

        //_shipController.HandleMovementInput(moveInput);



        //if (delta.sqrMagnitude > maxRange * maxRange)
        //    _shipController.targetSailAmount = ShipController.Sails.FULL_SAILS;
        //else
        //    _shipController.targetSailAmount = ShipController.Sails.QUARTER_SAILS;

    }

    
    public void DebugMap(Map map)
    {
        Vector2 pos = transform.position;
        for (int i = 0; i < Map.numDirections; i++)
        {
            float mag = Mathf.Abs(map[i]);
            Color c = map[i] > 0 ? Color.green : Color.red;
            Debug.DrawRay(pos + _inputVectors[i], _inputVectors[i] * mag * 3f, c);
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


    //private float[] GetChasePlayerMap()
    //{
    //    float[] map = new float[Map.numDirections];
    //    Vector2 pos = transform.position;
    //    Vector2 targetPos = target.position;
    //    Vector3 delta = targetPos - pos;
    //    Vector3 direction = delta.normalized;

    //    for (int i = 0; i < Map.numDirections; i++)
    //    {
    //        float dot = Vector3.Dot(_inputVectors[i], direction);
    //        map[i] = Mathf.Max(0, dot);
    //        //Color c = Color.Lerp(Color.red, Color.green, (_inputStrength[i] + 1) * 0.5f);
    //        //Debug.DrawRay(pos + _inputVectors[i], _inputVectors[i] * ([i] + 1) * 0.5f * 2, Color.green);
    //    }

    //    return map;
    //}

    //private float[] GetAvoidMap()
    //{
    //    float[] map = new float[Map.numDirections];
    //    Vector2 pos = transform.position;
    //    Vector2 targetPos = target.position;
    //    Vector3 delta = targetPos - pos;
    //    Vector3 direction = delta.normalized;

    //    for (int i = 0; i < Map.numDirections; i++)
    //    {
    //        float dot = Vector3.Dot(_inputVectors[i], direction);
    //        map[i] = 1f - Mathf.Abs(dot - 0.65f);
    //    }

    //    return map;
    //}
    
    //private Vector2 GetBestDirection(float[] map)
    //{
    //    int bestIndex = -1;
    //    float bestValue = -1;

    //    for (int i = 0; i < Map.numDirections; i++)
    //    {
    //        if (map[i] > bestValue)
    //        {
    //            bestValue = map[i];
    //            bestIndex = i;
    //        }
    //    }

    //    return _inputVectors[bestIndex] * bestValue;
    //}

    //private Vector2 AverageMap(float[] map)
    //{
    //    Vector2 result = Vector2.zero;
    //    for (int i = 0; i < map.Length; i++)
    //    {
    //        result += _inputVectors[i] * map[i];
    //    }

    //    result *= 1f / Map.numDirections;
    //    return result;
    //}
}

public class Map
{
    public static int numDirections { get; } = 24;
    public float[] weights = new float[numDirections];

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
