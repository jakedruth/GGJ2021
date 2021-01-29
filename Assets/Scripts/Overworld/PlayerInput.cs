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
        if (Input.GetMouseButton(0))
        {
            _target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            moveToTarget.transform.position = _target;
            moveToTarget.enabled = _isMovingToTarget = true;
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


        //if (Input.GetMouseButton(0))
        //{
        //    Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    Vector2 pos = transform.position;

        //    Vector3 delta = mousePos - pos;
        //    _shipController.HandleMovementInput(delta.normalized);
        //}

        if (Input.GetKeyDown(KeyCode.A))
        {
            _shipController.FireLeft();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            _shipController.FireRight();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            if (_shipController.sailAmount != ShipController.Sails.FULL_SAILS)
                _shipController.sailAmount++;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (_shipController.sailAmount != ShipController.Sails.NO_SAILS)
                _shipController.sailAmount--;
        }

        fireLeftText.rotation = Quaternion.identity;
        fireRightText.rotation = Quaternion.identity;
    }
}
