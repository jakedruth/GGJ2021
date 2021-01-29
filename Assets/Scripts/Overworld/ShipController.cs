using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShipController : MonoBehaviour
{
    public enum Sails
    {
        NO_SAILS,
        QUARTER_SAILS,
        HALF_SAILS,
        THREE_QUARTERS_SAILS,
        FULL_SAILS
    }

    [Header("Ship Health")]
    public float maxHP;
    public float hp;
    public UnityEventShip onShipDestroyed;

    [Header("Crew Variables")]
    [PlusMinus(0, 100)]
    public int crewSailing;
    [PlusMinus(0, 100)]
    public int crewCannons;
    [PlusMinus(0, 100)]
    public int crewRepairs;

    [Header("Sail/ Movement Variables")]
    public Sails sailAmount;
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

    [Header("Repair Variables")]
    public float crewRepairRate;

    void Start()
    {
        InitShip();
    }

    void Update()
    {
        // Handle Movement
        float targetSpeed = baseSpeed + crewSpeedBonus * crewSailing;
        targetSpeed *= (float)sailAmount / (float)Sails.FULL_SAILS;

        _speed = Mathf.MoveTowards(_speed, targetSpeed, acceleration * Time.deltaTime);
        
        Vector3 vel = transform.right * _speed;
        transform.position += vel * Time.deltaTime;

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

        if (hp < maxHP)
            hp += crewRepairRate * crewRepairs * Time.deltaTime;
        else
            hp = maxHP;

        //* Debug Code
        if (Input.GetKeyDown(KeyCode.Space))
            TakeDamage(30);
        /* End Debug Code */
    }

    public void InitShip()
    {
        hp = maxHP;
        _leftCannonCoolDown = _rightCannonCoolDown = 1;
        sailAmount = Sails.NO_SAILS;
    }

    public void TakeDamage(float amount)
    {
        hp -= amount;
        if (hp <= 0)
        {
            hp = 0;
            onShipDestroyed?.Invoke(this);
        }
    }

    public void HandleMovementInput(Vector2 input)
    {
        float targetAngle = Vector2.SignedAngle(Vector2.right, input);
        _heading = Mathf.MoveTowardsAngle(_heading, targetAngle,
            turnSpeed * (0.3f + 0.7f * _speed / baseSpeed) * Time.deltaTime);
        transform.localRotation = Quaternion.AngleAxis(_heading, Vector3.forward);
    }

    public void FireLeft()
    {
        if (_leftCannonCoolDown < 1)
            return;

        if (FireCannons(leftCannonFirePoint))
            _leftCannonCoolDown = 0;
    }

    public void FireRight()
    {
        if (_rightCannonCoolDown < 1)
            return;

        if (FireCannons(rightCannonFirePoint))
            _rightCannonCoolDown = 0;
    }

    private bool FireCannons(Transform cannons)
    {
        if (crewCannons <= 0)
            return false;

        float totalTime = 0.1f * Mathf.Sqrt(crewCannons);

        List<int> indices = new List<int>();
        for (int i = 0; i < crewCannons; i++)
        {
            indices.Add(i);
        }

        Shuffle(indices);

        for (int i = 0; i < indices.Count; i++)
        {
            int index = indices[i];
            float t = (index + 1) / (crewCannons + 1f);
            float delay = totalTime * ((float) i / crewCannons);

            Vector3 position = Vector3.Lerp(cannons.GetChild(0).position, cannons.GetChild(1).position, t);
            Vector3 relativePosition = cannons.InverseTransformPoint(position);

            Vector3 direction = Vector3.Lerp(cannons.GetChild(0).right, cannons.GetChild(1).right, t);
            Vector3 relativeDirection = cannons.InverseTransformDirection(direction);
            
            StartCoroutine(Fire(cannons, relativePosition, relativeDirection, delay));
        }

        return true;
    }

    private IEnumerator Fire(Transform cannons, Vector3 relativePosition, Vector3 relativeDirection, float delay)
    {
        yield return new WaitForSeconds(delay);
        Quaternion rotation = Quaternion.FromToRotation(Vector3.right, cannons.TransformDirection(relativeDirection));
        SpawnCannonball(cannons.TransformPoint(relativePosition), rotation);
    }

    private void SpawnCannonball(Vector3 position, Quaternion rotation)
    {
        Cannonball resource = Resources.Load<Cannonball>($"Prefabs/{cannonBallPrefabName}");
        Cannonball cb = Instantiate(resource, position, rotation);
        cb.InitCannonBall(transform, transform.right * _speed);
    }

    private static readonly System.Random RNG = new System.Random();
    public static void Shuffle<T>(IList<T> list)  
    {  
        int n = list.Count;  
        while (n > 1) {  
            n--;  
            int k = RNG.Next(n + 1);  
            T value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }  
    }
}
