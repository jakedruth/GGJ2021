using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    public float maxSpeed;
    private float _speed;
    public float turnSpeed;
    private float _heading;
    public float acceleration;

    public void HandleMovementInput(Vector2 input)
    {
        bool hasInput = input.sqrMagnitude > 0;

        float targetSpeed = hasInput ? maxSpeed : 0;
        _speed = Mathf.MoveTowards(_speed, targetSpeed, acceleration * Time.deltaTime);

        if (hasInput)
        {
            float targetAngle = Vector2.SignedAngle(Vector2.right, input);
            _heading = Mathf.MoveTowardsAngle(_heading, targetAngle, turnSpeed * _speed / maxSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.AngleAxis(_heading, Vector3.forward);
        }

        Vector3 vel = transform.right * _speed;

        transform.position += vel * Time.deltaTime;
    }
}
