using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ShipController))]
public class PlayerInput : MonoBehaviour
{
    private ShipController _shipController;
    public Transform fireLeftText;
    public Transform fireRightText;

    void Awake()
    {
        _shipController = GetComponent<ShipController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 inputDirection = Vector2.zero;
        if (Input.GetKey(KeyCode.A))
            inputDirection.x -= 1;
        if (Input.GetKey(KeyCode.D))
            inputDirection.x += 1;

        if (Input.GetKey(KeyCode.W))
            inputDirection.y += 1;
        if (Input.GetKey(KeyCode.S))
            inputDirection.y -= 1;

        if (inputDirection.sqrMagnitude > 0)
            inputDirection.Normalize();

        _shipController.HandleMovementInput(inputDirection);

        if (Input.GetMouseButton(0))
        {
            _shipController.FireLeft();
        }

        if (Input.GetMouseButton(1))
        {
            _shipController.FireRight();
        }

        if (Input.mouseScrollDelta.y > 0)
        {
            if (_shipController.sailAmount != ShipController.Sails.FULL_SAILS)
                _shipController.sailAmount++;
        }

        if (Input.mouseScrollDelta.y < 0)
        {
            if (_shipController.sailAmount != ShipController.Sails.NO_SAILS)
                _shipController.sailAmount--;
        }

        fireLeftText.rotation = Quaternion.identity;
        fireRightText.rotation = Quaternion.identity;
    }
}
