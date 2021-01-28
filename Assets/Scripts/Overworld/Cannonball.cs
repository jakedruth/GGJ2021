using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : MonoBehaviour
{
    public float moveSpeed;
    public float fireDistance;
    public AnimationCurve scaleOverTime;
    private float _totalLifeTime;
    private float _lifeTime;
    private Transform _owner;
    private Vector3 _inheritVel;

    void Start()
    {
        _totalLifeTime = fireDistance / moveSpeed;
        transform.GetChild(0).right = Vector3.right;
    }

    // Update is called once per frame
    void Update()
    {
        _lifeTime += Time.deltaTime;
        if (_lifeTime >= _totalLifeTime)
        {
            Destroy(gameObject);
            return;
        }

        transform.GetChild(0).localScale = Vector3.one * scaleOverTime.Evaluate(_lifeTime / _totalLifeTime);
        transform.position += (_inheritVel + transform.right * moveSpeed) * Time.deltaTime;
    }

    public void InitCannonBall(Transform owner, Vector3 inheritVel)
    {
        _owner = owner;
        _inheritVel = inheritVel;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform == _owner)
            return;

        Debug.Log($"Hit Ship: {collision.gameObject.name}");
    }


}
