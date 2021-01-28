using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ShipController))]
public class PlayerInput : MonoBehaviour
{
    private ShipController _shipController;

    void Awake()
    {
        _shipController = GetComponent<ShipController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 inputDirection = Vector2.zero;
        if (Input.GetMouseButton(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 pos = transform.position;

            Vector3 delta = mousePos - pos;
            inputDirection = delta.normalized;
        }

        _shipController.HandleMovementInput(inputDirection);

        if (Input.GetKeyDown(KeyCode.A))
        {
            _shipController.FireLeft();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            _shipController.FireRight();
        }
    }
}
