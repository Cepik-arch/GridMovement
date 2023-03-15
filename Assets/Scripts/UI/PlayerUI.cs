using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Text currentPlayer;
    public Text turnNumber;

    public void UpdateCurrentPlayerProperty(string player, Color color)
    {
        currentPlayer.text = turnNumber.text;
        currentPlayer.color = color;
    }

    public void UpdateTurnNumber(string number)
    {
        turnNumber.text = number;
    }

    //singleton
    public static PlayerUI instance;
    public static PlayerUI GetInstance()
    {
        return instance;
    }
    void Awake()
    {
        instance = this;
    }
    


}
