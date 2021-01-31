using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ShipController))]
public class AIInput : MonoBehaviour
{
    private const int NUM_DIRECTIONS = 12;

    private ShipController _shipController;
    private Vector2[] _inputVectors;

    public Transform target;


    void Awake()
    {
        _shipController = GetComponent<ShipController>();
        _inputVectors = new Vector2[NUM_DIRECTIONS];
        for (int i = 0; i < NUM_DIRECTIONS; i++)
        {
            float angle = i * Mathf.PI * 2 / NUM_DIRECTIONS;
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

        float[] map = GetAvoidMap();

        for (int i = 0; i < NUM_DIRECTIONS; i++)
        {
            //Color c = Color.Lerp(Color.red, Color.green, (_inputStrength[i] + 1) * 0.5f);
            Debug.DrawRay(pos + _inputVectors[i], _inputVectors[i] * map[i] * 2, Color.green);
        }

    }

    private float[] GetChasePlayerMap()
    {
        float[] map = new float[NUM_DIRECTIONS];
        Vector2 pos = transform.position;
        Vector2 targetPos = target.position;
        Vector3 delta = targetPos - pos;
        Vector3 direction = delta.normalized;

        for (int i = 0; i < NUM_DIRECTIONS; i++)
        {
            float dot = Vector3.Dot(_inputVectors[i], direction);
            map[i] = Mathf.Max(0, dot);
            //Color c = Color.Lerp(Color.red, Color.green, (_inputStrength[i] + 1) * 0.5f);
            //Debug.DrawRay(pos + _inputVectors[i], _inputVectors[i] * ([i] + 1) * 0.5f * 2, Color.green);
        }

        return map;
    }

    private float[] GetAvoidMap()
    {
        float[] map = new float[NUM_DIRECTIONS];
        Vector2 pos = transform.position;
        Vector2 targetPos = target.position;
        Vector3 delta = targetPos - pos;
        Vector3 direction = delta.normalized;

        for (int i = 0; i < NUM_DIRECTIONS; i++)
        {
            float dot = Vector3.Dot(_inputVectors[i], direction);
            map[i] = 1f - Mathf.Abs(dot - 0.65f);
        }

        return map;
    }
}
