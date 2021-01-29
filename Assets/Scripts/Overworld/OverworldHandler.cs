using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverWorldHandler : MonoBehaviour
{
    private OverWorldHUD _hud;
    public ShipController player;
    public Crew crew;

    void Start()
    {
        _hud = FindObjectOfType<OverWorldHUD>();
        player = FindObjectOfType<PlayerInput>().GetComponent<ShipController>();
        crew.sails.max = crew.cannons.max = crew.repair.max = 2;
        UpdateHUD();
    }

    public void HireCrewMember()
    {
        crew.AddCrewMember();
        UpdateHUD();
    }

    public void FireCrewMember()
    {
        crew.RemoveCrewMember();
        UpdateHUD();
    }

    public void UpdateHUD()
    {
        //_hud.UpdateUI();
    }
}

[Serializable]
public class Crew
{
    [SerializeField]
    public int members { get; private set; }

    public Station sails;
    public Station cannons;
    public Station repair;

    public int availableMembers
    {
        get { return members - sails.membersInStation - cannons.membersInStation - repair.membersInStation; }
    }

    public void AddCrewMember()
    {
        members += 1;
    }

    public void RemoveCrewMember()
    {
        if (members == 0)
            return;

        if (availableMembers > 0)
            members -= 1;
        else if (sails.membersInStation > 0)
            sails.RemoveCrewMember();
        else if (cannons.membersInStation > 0)
            cannons.RemoveCrewMember();
        else if (repair.membersInStation > 0)
            repair.RemoveCrewMember();
        else
            Debug.LogError("THIS AIN'T GOOD! YOU TRIED TO REMOVE A CREW BUT HAVE NO CREW");
    }

    public IEnumerator AddMemberToStation(Station station, float delay)
    {
        station.moving += 1;

        yield return new WaitForSeconds(delay);
        
        // Double check there is still a member trying to get into 
        if (station.moving > 1)
        {
            station.moving -= 1;
            station.working += 1;
        }
    }
}

[Serializable]
public class Station
{
    public int working;
    public int moving;
    public int max;
    public int membersInStation
    {
        get { return working + moving; }
    }

    public bool isFull
    {
        get { return membersInStation == max; }
    }

    public bool RemoveCrewMember()
    {
        if (membersInStation == 0)
            return false;

        if (moving > 1)
        {
            moving -= 1;
        }
        else
        {
            working -= 1;
        }

        return true;
    }
}
