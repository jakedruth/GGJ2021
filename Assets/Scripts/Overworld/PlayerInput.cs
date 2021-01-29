using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ShipController))]
public class PlayerInput : MonoBehaviour
{
    private ShipController _shipController;
    private EventSystem _eventSystem;

    public SpriteRenderer moveToTarget;
    public Transform fireLeftText;
    public Transform fireRightText;
    private Vector2 _target;
    private bool _isMovingToTarget;

    void Awake()
    {
        _shipController = GetComponent<ShipController>();
        _eventSystem = FindObjectOfType<EventSystem>();

        moveToTarget.transform.SetParent(null);
        moveToTarget.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        #region Cannon Controls

        if (Input.GetKeyDown(KeyCode.Q))
        {
            _shipController.FireLeft();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            _shipController.FireRight();
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
                _shipController.FireLeft();
                _shipController.FireRight();
            }
            else
            {
                float side = Mathf.Sign(angle);
                if (side > 0)
                    _shipController.FireLeft();
                else 
                    _shipController.FireRight();
            }
        }

        #endregion

        #region Sails Controlls

        if (Input.GetKeyDown(KeyCode.W) || Input.mouseScrollDelta.y > 0)
        {
            if (_shipController.targetSailAmount != ShipController.Sails.FULL_SAILS)
                _shipController.targetSailAmount++;
        }

        if (Input.GetKeyDown(KeyCode.S)  || Input.mouseScrollDelta.y < 0)
        {
            if (_shipController.targetSailAmount != ShipController.Sails.NO_SAILS)
                _shipController.targetSailAmount--;
        }

        #endregion

        #region Turning Input

        if (_isMovingToTarget)
        {
            Vector2 pos = transform.position;
            Vector2 delta = _target - pos;
            _shipController.HandleMovementInput(delta.normalized);

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
                _shipController.HandleMovementInput(transform.up);

            if (Input.GetKey(KeyCode.D))
                _shipController.HandleMovementInput(-transform.up);
        }

        #endregion

        fireLeftText.rotation = Quaternion.identity;
        fireRightText.rotation = Quaternion.identity;
    }
}
