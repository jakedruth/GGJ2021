using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OverWorldHUD : MonoBehaviour
{
    public static OverWorldHUD instance;

    public TMP_Text availableText;
    public TMP_Text sailsText;
    public TMP_Text cannonsText;
    public TMP_Text repairsText;

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
