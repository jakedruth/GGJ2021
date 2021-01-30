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
        moveToTarget.transform.SetParent(null);
        moveToTarget.enabled = false;
    }

    private void OnEnable()
    {
        shipController = GetComponent<ShipController>();
        shipController.onShipHPChanged.AddListener(OnShipChangeHP);
    }

    private void OnDisable()
    {
        shipController.onShipHPChanged.RemoveListener(OnShipChangeHP);
    }

    void Start()
    {
        UpdateCrewSectionHUD();
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

        StartCoroutine(shipController.crew.MoveMemberToStation(station, UpdateCrewSectionHUD));
    }

    public void RemoveCrewMemberFromStation(int stationID)
    {
        RemoveCrewMemberFromStation((StationType)stationID);
    }

    public void RemoveCrewMemberFromStation(StationType station)
    {
        shipController.crew.RemoveMemberFromStation(station);
        UpdateCrewSectionHUD();
    }

    public void UpdateCrewSectionHUD()
    {
        Crew c = shipController.crew;
        OverWorldHUD.instance.UpdateUI(
            c.availableMembers, c.members,
            c.sails.working, c.sails.onRoute, c.sails.max,
            c.cannons.working, c.cannons.onRoute, c.cannons.max,
            c.repair.working, c.repair.onRoute, c.repair.max);
    }

    public void OnShipChangeHP(float amount)
    {
        //OverWorldHUD.instance.SetHP(shipController.hp, shipController.maxHP);
    }

    // Update is called once per frame
    void Update()
    {
        #region Cannon Controls

        if (Input.GetKeyDown(KeyCode.Q))
        {
            shipController.FireLeft();
            if (shipController.crew.cannons.working == 0)
                OverWorldHUD.instance.ShowPopUpForDuration(3f,
                    "You need to have crew members on the Cannons Station in order to fire a cannon", true,
                    onButtonClickedAction: () => { OverWorldHUD.instance.HidePopUp(); });
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            shipController.FireRight();
            if (shipController.crew.cannons.working == 0)
                OverWorldHUD.instance.ShowPopUpForDuration(3f,
                    "You need to have crew members on the Cannons Station in order to fire a cannon", true,
                    onButtonClickedAction: () => { OverWorldHUD.instance.HidePopUp(); });
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (shipController.crew.cannons.working == 0)
            {
                OverWorldHUD.instance.ShowPopUpForDuration(3f,
                    "You need to have crew members on the Cannons Station in order to fire a cannon", true,
                    onButtonClickedAction: () => { OverWorldHUD.instance.HidePopUp(); });
            }
            else
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
        }

        #endregion

        #region Sails Controlls

        if (Input.GetKeyDown(KeyCode.W) || Input.mouseScrollDelta.y > 0)
        {
            if (shipController.targetSailAmount != ShipController.Sails.FULL_SAILS)
            {
                shipController.targetSailAmount++;
                if (shipController.crew.sails.working == 0)
                    OverWorldHUD.instance.ShowPopUpForDuration(2f,
                        "You need to have crew members on the Sails Station in order to change the sails", false);
            }
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.mouseScrollDelta.y < 0)
        {
            if (shipController.targetSailAmount != ShipController.Sails.NO_SAILS)
            {
                shipController.targetSailAmount--;
                if (shipController.crew.sails.working == 0)
                    OverWorldHUD.instance.ShowPopUpForDuration(2f,
                        "You need to have crew members on the Sails Station in order to change the sails", false);
            }
        }

        // Update HUD
        OverWorldHUD.instance.SetSails01(shipController.sailAmount);
        OverWorldHUD.instance.SetTargetSail(shipController.targetSailAmount);
        OverWorldHUD.instance.SetHP(shipController.hp, shipController.maxHP);
        OverWorldHUD.instance.SetSpeed(shipController.speed);

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
