using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool isPaused { get; private set; }
    public UnityEventBool onPausedChanged = new UnityEventBool();

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void TogglePause()
    {
        SetPause(!isPaused);
    }

    public void SetPause(bool value)
    {
        if (isPaused == value)
            return;

        onPausedChanged?.Invoke(value);

        isPaused = value;
    }

    // TODO: move this to a new locations
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            QuitGame();
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

[System.Serializable]
public class UnityEventBool : UnityEvent<bool>
{ }
public class UnityEventFloat : UnityEvent<float>
{ }

[System.Serializable]
public class UnityEventShip : UnityEvent<ShipController>
{ }