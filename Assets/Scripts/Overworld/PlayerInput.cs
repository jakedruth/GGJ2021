using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ShipController))]
public class PlayerInput : MonoBehaviour
{
    public ShipController shipController;
    
    [Header("Required Transforms")]
    public SpriteRenderer moveToTarget;
    public Transform fireLeftText;
    public Transform fireRightText;
    private Vector2 _target;
    private bool _isMovingToTarget;

    void Awake()
    {
        shipController = GetComponent<ShipController>();

        moveToTarget.transform.SetParent(null);
        moveToTarget.enabled = false;
    }

    void Start()
    {
        UpdateHUD();
    }

    public void HireCrewMember()
    {
        shipController.crew.AddCrewMember();
    }

    public void FireCrewMember()
    {
        shipController.crew.RemoveCrewMember();
    }

    public void AddCrewMemberToStation(int stationID)
    {
        AddCrewMemberToStation((StationType)stationID);
    }

    public void AddCrewMemberToStation(StationType station)
    {
        if (shipController.crew.availableMembers <= 0)
            return;

        StartCoroutine(shipController.crew.MoveMemberToStation(station, UpdateHUD));
    }

    public void RemoveCrewMemberFromStation(int stationID)
    {
        RemoveCrewMemberFromStation((StationType)stationID);
    }

    public void RemoveCrewMemberFromStation(StationType station)
    {
        shipController.crew.RemoveMemberFromStation(station);
        UpdateHUD();
    }

    public void UpdateHUD()
    {
        Crew c = shipController.crew;
        OverWorldHUD.instance.UpdateUI(
            c.availableMembers, c.members,
            c.sails.working, c.sails.onRoute, c.sails.max,
            c.cannons.working, c.cannons.onRoute, c.cannons.max,
            c.repair.working, c.repair.onRoute, c.repair.max);
    }

    // Update is called once per frame
    void Update()
    {
        #region Cannon Controls

        if (Input.GetKeyDown(KeyCode.Q))
        {
            shipController.FireLeft();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            shipController.FireRight();
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 pos = transform.position;

            Vector3 delta = mousePos - pos;
            Vector3 dir = delta.normalized;

            float angle = Vector2.SignedAngle(transform.right, dir);
           
            // TODO: Make 30 a public variable, and maybe a setting a player can fiddle with
            if (Mathf.Abs(angle) <= 30)
            {
                shipController.FireLeft();
                shipController.FireRight();
            }
            else
            {
                float side = Mathf.Sign(angle);
                if (side > 0)
                    shipController.FireLeft();
                else 
                    shipController.FireRight();
            }
        }

        #endregion

        #region Sails Controlls

        if (Input.GetKeyDown(KeyCode.W) || Input.mouseScrollDelta.y > 0)
        {
            if (shipController.targetSailAmount != ShipController.Sails.FULL_SAILS)
                shipController.targetSailAmount++;
        }

        if (Input.GetKeyDown(KeyCode.S)  || Input.mouseScrollDelta.y < 0)
        {
            if (shipController.targetSailAmount != ShipController.Sails.NO_SAILS)
                shipController.targetSailAmount--;
        }

        #endregion

        #region Turning Input

        if (_isMovingToTarget)
        {
            Vector2 pos = transform.position;
            Vector2 delta = _target - pos;
            shipController.HandleMovementInput(delta.normalized);

            const float rangeToStop = 1f;
            if (delta.sqrMagnitude <= rangeToStop * rangeToStop)
            {
                moveToTarget.enabled = _isMovingToTarget = false;
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                _target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                moveToTarget.transform.position = _target;
                moveToTarget.enabled = _isMovingToTarget = true;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
                moveToTarget.enabled = _isMovingToTarget = false;

            if (Input.GetKey(KeyCode.A))
                shipController.HandleMovementInput(transform.up);

            if (Input.GetKey(KeyCode.D))
                shipController.HandleMovementInput(-transform.up);
        }

        #endregion

        fireLeftText.rotation = Quaternion.identity;
        fireRightText.rotation = Quaternion.identity;
    }
}
