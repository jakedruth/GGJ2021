using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    [Header("Ship Health")]
    public float maxHP;
    public float hp;

    [Header("Crew Variables")]
    [PlusMinus(0, 100)]
    public int sailCrew;
    [PlusMinus(0, 100)]
    public int cannonCrew;
    [PlusMinus(0, 100)]
    public int repairCrew;

    [Header("Sail/ Movement Variables")]
    public float baseSpeed;
    public float crewSpeedBonus;
    private float _speed;
    public float turnSpeed;
    private float _heading;
    public float acceleration;

    [Header("Cannon Ball Variables")] 
    public string cannonBallPrefabName;
    public float reloadBaseRate;
    public float crewReloadBonus;
    public Transform leftCannonFirePoint;
    public Transform rightCannonFirePoint;
    private float _leftCannonCoolDown;
    private float _rightCannonCoolDown;

    void Start()
    {
        SetHPToMax();
    }

    void Update()
    {
        // Handle Left Cannon
        if (_leftCannonCoolDown < 1)
            _leftCannonCoolDown += reloadBaseRate * Time.deltaTime;
        else
            _leftCannonCoolDown = 1;

        // Handle Right Cannon
        if (_rightCannonCoolDown < 1)
            _rightCannonCoolDown += reloadBaseRate * Time.deltaTime;
        else
            _rightCannonCoolDown = 1;
    }

    public void SetHPToMax()
    {
        hp = maxHP;
    }

    public void HandleMovementInput(Vector2 input)
    {
        bool hasInput = input.sqrMagnitude > 0;
        float targetSpeed = hasInput ? baseSpeed + crewSpeedBonus * sailCrew : 0;

        _speed = Mathf.MoveTowards(_speed, targetSpeed, acceleration * Time.deltaTime);

        if (hasInput)
        {
            float targetAngle = Vector2.SignedAngle(Vector2.right, input);
            _heading = Mathf.MoveTowardsAngle(_heading, targetAngle, turnSpeed * _speed / baseSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.AngleAxis(_heading, Vector3.forward);
        }

        Vector3 vel = transform.right * _speed;

        transform.position += vel * Time.deltaTime;
    }

    public void FireLeft()
    {
        if (_leftCannonCoolDown < 1)
            return;

        _leftCannonCoolDown = 0;
        SpawnCannonballAtTransform(leftCannonFirePoint);
    }

    public void FireRight()
    {
        if (_rightCannonCoolDown < 1)
            return;

        _rightCannonCoolDown = 0;
        SpawnCannonballAtTransform(rightCannonFirePoint);
    }

    private void SpawnCannonballAtTransform(Transform t)
    {
        Cannonball resource = Resources.Load<Cannonball>($"Prefabs/{cannonBallPrefabName}");
        Cannonball cb = Instantiate(resource, t.position, t.rotation);
        cb.InitCannonBall(transform, transform.right * _speed);
    }
}
