using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class switchTurn : MonoBehaviour
{
    public Button toggleTurnButton; // reference to your button
    public GameManager gameManager; // reference to your GameManager

    void Start()
    {
        toggleTurnButton.onClick.AddListener(gameManager.TogglePlayerTurn);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
