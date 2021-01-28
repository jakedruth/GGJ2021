using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MinigameTile : MonoBehaviour
{
    MinigameManager minigameManager;
    TMP_Text text;

    public Sprite sprite;
    public int tileDepth = 3;

    void Start()
    {
        minigameManager = GameObject.Find("Minigame Manager").GetComponent<MinigameManager>();
        text = transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        text.text = tileDepth.ToString();
    }

    void OnMouseDown()
    {
        Dig(minigameManager.pickDamage);
    }

    void Dig(int amount)
    {
        tileDepth -= amount;
        text.text = tileDepth.ToString();

        if (tileDepth <= 0)
            Destroy(gameObject);
    }
}
