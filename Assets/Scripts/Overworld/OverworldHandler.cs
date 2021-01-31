using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OverWorldHandler : MonoBehaviour
{
    private OverWorldHUD _hud;
    public PlayerInput player;

    void Start()
    {
        _hud = FindObjectOfType<OverWorldHUD>();
        player = FindObjectOfType<PlayerInput>();
        
        UpdateHUD();
    }
    
    public void UpdateHUD()
    {
        //_hud.UpdateUI();

        // TODO: Probs the day night cycle shit
    }

    [ContextMenu("Land at port")]
    public void LandAtPort()
    {
        SceneManager.LoadScene("HomePortMenu", LoadSceneMode.Additive);
        Time.timeScale = 0;
    }

    public void LandOnIsland(Island island)
    {
        SceneManager.LoadScene("Minigame Testing", LoadSceneMode.Additive);
        Time.timeScale = 0;
    }

    public void LeaveIsland(object other)
    {
        Time.timeScale = 1;
        if (other is HomePortHandler port)
        {
            SceneManager.UnloadSceneAsync("HomePortMenu");
        }
        else if (other is Island island)
        {

        }
    }
}

[Serializable]
public class Crew
{
    [SerializeField] 
    public int members;
    public float moveSpeed;

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

    public IEnumerator MoveMemberToStation(StationType type, Action updateHudCallback)
    {
        if (availableMembers <= 0)
            yield break;

        Station s;
        switch (type)
        {
            case StationType.SAILS:
                s = sails;
                break;
            case StationType.CANNONS:
                s = cannons;
                break;
            case StationType.REPAIRS:
                s = repair;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        s.onRoute += 1;
        updateHudCallback.Invoke();

        yield return new WaitForSeconds(moveSpeed);
        
        // Double check there is still a member is available in onRoute to get into 
        if (s.onRoute <= 0)
        {
            updateHudCallback.Invoke();
            yield break;
        }

        s.onRoute -= 1;
        s.working += 1;

        updateHudCallback.Invoke();
    }

    public void RemoveMemberFromStation(StationType type)
    {
        Station s;
        switch (type)
        {
            case StationType.SAILS:
                s = sails;
                break;
            case StationType.CANNONS:
                s = cannons;
                break;
            case StationType.REPAIRS:
                s = repair;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        if (s.membersInStation <= 0)
            return;

        s.RemoveCrewMember();
    }
}

[System.Serializable]
[SerializeField]
public enum StationType
{
    SAILS,
    CANNONS,
    REPAIRS
}

[Serializable]
public class Station
{
    public int working;
    public int onRoute;
    public int max;

    public int membersInStation
    {
        get { return working + onRoute; }
    }

    public bool isFull
    {
        get { return membersInStation == max; }
    }

    public bool RemoveCrewMember()
    {
        if (membersInStation == 0)
            return false;

        if (onRoute > 0)
        {
            onRoute -= 1;
        }
        else
        {
            working -= 1;
        }

        return true;
    }

    public int SetMax(int value)
    {
        if (value >= max)
            max = value;
        else if (onRoute > 0)
        {
            onRoute -= 1;
            max = value;
        }
        else
        {
            working -= 1;
            max = value;
        }

        return max;
    }
}
