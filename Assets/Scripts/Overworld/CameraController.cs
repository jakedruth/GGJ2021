using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public ShipController target;
    public float distanceAheadOfTargetParam;
    public Vector3 offset;
    public float smoothing;

    public float zoomLevel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
            return;

        Vector2 currentPos = transform.position;
        Vector2 targetPos = target.transform.position +
                            target.transform.right * target.speed * distanceAheadOfTargetParam + 
                            offset;
        Vector3 pos = Damp(currentPos, targetPos, smoothing, Time.deltaTime);

        pos.z = offset.z;
        transform.position = pos;
    }

    public float Damp(float a, float b, float lambda, float dt)
    {
        return Mathf.Lerp(a, b, 1 - Mathf.Exp(-lambda * dt));
    }

    public Vector3 Damp(Vector3 a, Vector3 b, float lambda, float dt)
    {
        return new Vector3(
            Damp(a.x, b.x, lambda, dt),
            Damp(a.y, b.y, lambda, dt),
            Damp(a.z, b.z, lambda, dt)
        );
    }
}
