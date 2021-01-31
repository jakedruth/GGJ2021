using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CompositeCollider2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Island : MonoBehaviour
{
    public bool isHomePort;
    public float difficultyLevel;
    private PlayerInput _player;

    void Update()
    {
        if (_player == null)
            return;

        bool canLand = _player.shipController.speed <= 0;
        string text =  !canLand
            ? "In order to land on this island you need to stop." 
            : "Press the space bar or click \"land\" to hunt for buried treasure.";

        OverWorldHUD.instance.ShowPopUp(text, true, "Land", canLand, null);
        if (canLand)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                OnButtonClickedAction();
            }
        }
    }

    private void OnButtonClickedAction()
    {
        if (_player != null)
            _player.moveToTarget.enabled = _player.isMovingToTarget = false;

        _player = null;

        FindObjectOfType<OverWorldHandler>().LandOnIsland(this);

        OverWorldHUD.instance.HidePopUp();

        if (isHomePort)
            return;

        Destroy(gameObject);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            _player = collision.GetComponent<PlayerInput>();
            OverWorldHUD.instance.ShowPopUp("In order to land on this island you need to stop.", true, "Land", false, OnButtonClickedAction);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            _player = null;
            OverWorldHUD.instance.HidePopUp();
        }
    }
}
