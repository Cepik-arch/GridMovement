using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public PlayerInteractions playerInteractions;

    private bool isAITurn = false;

    private void Start()
    {
        // Get a reference to the PlayerInteractions script
        playerInteractions = GetComponent<PlayerInteractions>();
    }

    private void Update()
    {
        // Check if it's the AI's turn
        if (isAITurn)
        {
            // Move the active unit
            MoveActiveUnit();
        }
    }

    private void MoveActiveUnit()
    {
        // Your code to move the active unit here
        // This can be similar to the MoveUnit() method in PlayerInteractions
    }
}
