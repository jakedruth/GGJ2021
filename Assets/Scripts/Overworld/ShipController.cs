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
    public UnityEventFloat onShipHPChanged = new UnityEventFloat();
    public UnityEventShip onShipDestroyed = new UnityEventShip();

    [Header("Crew Variables")]
    public Crew crew;

    [Header("Movement Variables")]
    public float baseSpeed;
    public float baseSpeedCrewBonus;
    internal float speed;
    public float turnSpeed;
    public float acceleration;
    private Vector3 _driftingVel;

    [Header("Sail Variables")]
    public Sails targetSailAmount;

    internal float sailAmount;
    public float changeSailRate;
    public float changeSailRateCrewBonus;

    [Header("Cannon Ball Variables")] 
    public string cannonBallPrefabName;
    public float reloadBaseRate;
    public float reloadBaseRateCrewBonus;
    public Transform leftCannonFirePoint;
    public Transform rightCannonFirePoint;
    internal float leftCannonCoolDown;
    internal float rightCannonCoolDown;

    [Header("Repair Variables")]
    public float crewRepairRate;

    void Start()
    {
        InitShip();
    }

    void Update()
    {
        if (GameManager.instance.isPaused)
            return;

        // Handle Sails
        float targetSails = (float) targetSailAmount / (float)Sails.FULL_SAILS;
        float deltaSails = crew.sails.working > 0 ? changeSailRate + (crew.sails.working * changeSailRateCrewBonus) : 0;
        sailAmount = Mathf.MoveTowards(sailAmount, targetSails, deltaSails * Time.deltaTime);
        
        // Handle Speed
        float targetSpeed = sailAmount * (baseSpeed + baseSpeedCrewBonus * crew.sails.working);
        speed = Mathf.MoveTowards(speed, targetSpeed, acceleration * Time.deltaTime);
        
        // Handle Movement
        if (_driftingVel.sqrMagnitude >= 0.000001f) 
            _driftingVel *= 0.9f;
        
        Vector3 vel = transform.right * speed + _driftingVel;
        transform.position += vel * Time.deltaTime;

        // Handle Left Cannon
        if (leftCannonCoolDown < 1)
            leftCannonCoolDown += reloadBaseRate * Time.deltaTime;
        else
            leftCannonCoolDown = 1;

        // Handle Right Cannon
        if (rightCannonCoolDown < 1)
            rightCannonCoolDown += reloadBaseRate * Time.deltaTime;
        else
            rightCannonCoolDown = 1;


        // Handle Crew Repairs
        if (hp < maxHP)
        {
            float repairAmount = crewRepairRate * crew.repair.working * Time.deltaTime;
            hp += repairAmount;
            if (hp > maxHP)
                hp = maxHP;

            onShipHPChanged?.Invoke(repairAmount);
        }

        //* Debug Code
        if (Input.GetKeyDown(KeyCode.Space))
            TakeDamage(30);
        /* End Debug Code */
    }

    public void InitShip()
    {
        hp = maxHP;
        leftCannonCoolDown = rightCannonCoolDown = 1;
        targetSailAmount = Sails.NO_SAILS;
    }

    public void TakeDamage(float amount)
    {
        hp -= amount;
        onShipHPChanged?.Invoke(amount);
        if (hp > maxHP)
        {
            hp = maxHP;
        }
        else if (hp <= 0)
        {
            hp = 0;
            onShipDestroyed?.Invoke(this);
        }
    }

    public void HandleMovementInput(Vector2 input)
    {
        float targetAngle = Vector2.SignedAngle(Vector2.right, input);
        float heading = Vector2.SignedAngle(Vector2.right, transform.right);
        float newHeading = Mathf.MoveTowardsAngle(heading, targetAngle,
            turnSpeed * (0.3f + 0.7f * speed / baseSpeed) * Time.deltaTime);
        transform.localRotation = Quaternion.AngleAxis(newHeading, Vector3.forward);
    }

    public void FireLeft()
    {
        if (leftCannonCoolDown < 1)
            return;

        if (FireCannons(leftCannonFirePoint))
            leftCannonCoolDown = 0;
    }

    public void FireRight()
    {
        if (rightCannonCoolDown < 1)
            return;

        if (FireCannons(rightCannonFirePoint))
            rightCannonCoolDown = 0;
    }

    private bool FireCannons(Transform cannons)
    {
        if (GameManager.instance.isPaused)
            return false;

        if (crew.cannons.working <= 0)
            return false;

        float totalTime = 0.1f * Mathf.Sqrt(crew.cannons.working);

        List<int> indices = new List<int>();
        for (int i = 0; i < crew.cannons.working; i++)
        {
            indices.Add(i);
        }

        Shuffle(indices);

        for (int i = 0; i < indices.Count; i++)
        {
            int index = indices[i];
            float t = (index + 1) / (crew.cannons.working + 1f);
            float delay = totalTime * ((float) i / crew.cannons.working);

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
        cb.InitCannonBall(this);
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag != "Cannonball")
        {
            float speedParam = speed / baseSpeed;
            ContactPoint2D[] contacts = new ContactPoint2D[collision.contactCount];
            collision.GetContacts(contacts);
            Vector2 point = Vector2.zero;
            Vector2 normal = Vector2.zero;
            
            foreach (ContactPoint2D contact in contacts)
            {
                point += contact.point;
                normal += contact.normal;
            }

            Vector3 direction = normal.normalized;
            _driftingVel += direction * speedParam * 30f;
            speed = 0;

            TakeDamage(20 * speedParam);
        }
    }
}
