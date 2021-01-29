using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OverWorldHUD : MonoBehaviour
{
    public static OverWorldHUD instance;

    [Header("Ship Bars")]
    public RectTransform hpValue;
    public RectTransform sailsValue;
    public RectTransform sailsTarget;
    private float _sailsTargetMaxX;

    [Header("Ship TMP_Texts")] 
    public TMP_Text hpText;
    public TMP_Text sailAmountText;
    public TMP_Text speedText;

    [Header("Crew TMP_Texts")]
    public TMP_Text availableText;
    public TMP_Text sailsText;
    public TMP_Text cannonsText;
    public TMP_Text repairsText;

    [Header("Crew Buttons")]
    public Button sailsRemoveButton;
    public Button sailsAddButton;
    public Button cannonsRemoveButton;
    public Button cannonsAddButton;
    public Button repairsRemoveButton;
    public Button repairsAddButton;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        _sailsTargetMaxX = sailsTarget.localPosition.x;
    }

    void Start()
    {
        PlayerInput player = FindObjectOfType<PlayerInput>();
        
        // Sails
        sailsRemoveButton.onClick.AddListener(() => { player.RemoveCrewMemberFromStation(StationType.SAILS); });
        sailsAddButton.onClick.AddListener(() => { player.AddCrewMemberToStation(StationType.SAILS); });

        // Cannons
        cannonsRemoveButton.onClick.AddListener(() => { player.RemoveCrewMemberFromStation(StationType.CANNONS); });
        cannonsAddButton.onClick.AddListener(() => { player.AddCrewMemberToStation(StationType.CANNONS); });

        // Repairs
        repairsRemoveButton.onClick.AddListener(() => { player.RemoveCrewMemberFromStation(StationType.REPAIRS); });
        repairsAddButton.onClick.AddListener(() => { player.AddCrewMemberToStation(StationType.REPAIRS); });
    }

    public void SetHP(float value, float max)
    {
        hpValue.localScale = new Vector3(value / max, 1f, 1f);
        hpText.text = $"{value:f1} / {max:f1}";
    }

    public void SetSails01(float value)
    {
        sailsValue.localScale = new Vector3(value, 1f, 1f);
    }

    public void SetTargetSail(ShipController.Sails sails)
    {
        float value = (float) sails / (float) ShipController.Sails.FULL_SAILS;
        Vector3 pos = sailsTarget.localPosition;
        pos.x = _sailsTargetMaxX * (value * 2f - 1f);
        sailsTarget.localPosition = pos;

        string sailPosition = "Sail Amount: ";
        switch (sails)
        {
            case ShipController.Sails.NO_SAILS:
                sailPosition += "No Sails";
                break;
            case ShipController.Sails.QUARTER_SAILS:
                sailPosition += "¼ Sails";
                break;
            case ShipController.Sails.HALF_SAILS:
                sailPosition += "½ Sails";
                break;
            case ShipController.Sails.THREE_QUARTERS_SAILS:
                sailPosition += "¾ Sails";
                break;
            case ShipController.Sails.FULL_SAILS:
                sailPosition += "Full Sails";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(sails), sails, null);
        }

        sailAmountText.text = sailPosition;
    }

    public void SetSpeed(float value)
    {
        const float unitsPerSecondToKnots = 0.6f; // ~~Rough~~ estimate of units to meters to knots
        float knots = value * unitsPerSecondToKnots;

        speedText.text = $"Speed: {knots:f2} knots";
    }
    
    public void UpdateUI(int available, int availableMax,
        int sails, int sailsMoving, int sailsMax,
        int cannons, int cannonsMoving, int cannonsMax,
        int repairs, int repairsMoving, int repairsMax)
    {
        availableText.text = $"Available:\t{available} / {availableMax}";
        sailsText.text = $"Sails:\t\t{sails} / {sailsMoving} / {sailsMax}";
        cannonsText.text = $"Cannons:\t{cannons} / {cannonsMoving} / {cannonsMax}";
        repairsText.text = $"Repairs:\t{repairs} / {repairsMoving} / {repairsMax}";

        sailsRemoveButton.interactable = sails + sailsMoving > 0;
        cannonsRemoveButton.interactable = cannons + cannonsMoving > 0;
        repairsRemoveButton.interactable = repairs + repairsMoving > 0;

        sailsAddButton.interactable = available > 0 && sails + sailsMoving != sailsMax;
        cannonsAddButton.interactable = available > 0 && cannons + cannonsMoving != cannonsMax;
        repairsAddButton.interactable = available > 0 && repairs + repairsMoving != repairsMax;
    }
}
