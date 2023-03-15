using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class KeyboardInput : MonoBehaviour
    {
        public KeyCode moveKey = KeyCode.Space;
        public KeyCode endTurn = KeyCode.Return;

        PlayerInteractions playerInteractions = null;
        GameManager gameManager = null;

        // Start is called before the first frame update
        void Start()
        {
            playerInteractions = PlayerInteractions.GetInstance();
            gameManager = GameManager.GetInstance();
        }

        // Update is called once per frame
        void Update()
        {
            playerInteractions.moveUnit = Input.GetKeyDown(moveKey);

            if (Input.GetKeyDown(endTurn))
            {
                if (gameManager.playersList[gameManager.index].playerType == PlayersBase.PlayerTypes.user)
                {
                    gameManager.ChangePlayer();
                }
            }
        }
    }
}
