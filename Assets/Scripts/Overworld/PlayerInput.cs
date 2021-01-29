using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ShipController))]
public class PlayerInput : MonoBehaviour
{
    private ShipController _shipController;
    public SpriteRenderer moveToTarget;
    public Transform fireLeftText;
    public Transform fireRightText;
    private Vector2 _target;
    private bool _isMovingToTarget;

    void Awake()
    {
        _shipController = GetComponent<ShipController>();
        moveToTarget.transform.SetParent(null);
        moveToTarget.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _shipController.FireLeft();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            _shipController.FireRight();
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.mouseScrollDelta.y > 0)
        {
            if (_shipController.sailAmount != ShipController.Sails.FULL_SAILS)
                _shipController.sailAmount++;
        }

        if (Input.GetKeyDown(KeyCode.S)  || Input.mouseScrollDelta.y < 0)
        {
            if (_shipController.sailAmount != ShipController.Sails.NO_SAILS)
                _shipController.sailAmount--;
        }

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
            _target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            moveToTarget.transform.position = _target;
            moveToTarget.enabled = _isMovingToTarget = true;
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

        fireLeftText.rotation = Quaternion.identity;
        fireRightText.rotation = Quaternion.identity;
    }
}
