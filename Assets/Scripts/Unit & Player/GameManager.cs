using GridMaster;
using System.Collections;
using System.Collections.Generic;
using UnitControl;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class GameManager : MonoBehaviour
    {
        public Button toggleTurnButton;
        public bool isPlayerTurn = false;

        PlayersBase activePlayer;
        public int index;
        public List<PlayersBase> playersList = new List<PlayersBase> { new PlayersBase { playerId = "Player1", playerType = PlayersBase.PlayerTypes.user } };

        public bool noUnitsLeft;

        public GameStats gameStats;
        public PlayerUI playerUI;

        void Start()
        {
            playerUI = PlayerUI.GetInstance();
            toggleTurnButton.onClick.AddListener(TogglePlayerTurn);
            activePlayer = playersList[0];

            gameStats.turnNumber = 0;
            playerUI.UpdateCurrentPlayerProperty(activePlayer.playerId, activePlayer.playerColor);

        }

        void Update()
        {

        }

        public void TogglePlayerTurn()
        {
            isPlayerTurn = !isPlayerTurn;
            Debug.Log("Player Turn: " + isPlayerTurn);
        }

        public bool CompareIDwithActivePlayer(string id)
        {
            bool retval = false;
            if (activePlayer != null)
            {
                if (string.Equals(activePlayer.playerId, id))
                {
                    retval = true;
                }
            }
            return retval;
        }

        public void ChangePlayer()
        {
            if(index < playersList.Count)
            {
                index++;
            }
            else
            {
                gameStats.turnNumber++;
                playerUI.UpdateTurnNumber(gameStats.turnNumber.ToString());
                index = 0;
            }
            PlayerInteractions.GetInstance().ClearActiveUnit();
            activePlayer = playersList[index];
            playerUI.UpdateCurrentPlayerProperty(activePlayer.playerId, activePlayer.playerColor);
            EnableUnitForActivePlayer();

        }

        public void EnableUnitForActivePlayer()
        {
            for(int i = 0; i < activePlayer.allUnits.Count; i++)
            {
                activePlayer.allUnits[i].InitForStartOfTurn();
            }
        }

        //Singleton
        public static GameManager instance;
        void Awake()
        {
            instance = this;
            Debug.Log(instance.ToString());
            DontDestroyOnLoad(gameObject);
        }

        public static GameManager GetInstance()
        {

            return instance;
        }

    }

    [System.Serializable]
    public class GameStats
    {
        public int turnNumber;

        public int playerCount;
        public int aiCount;
        public int simulatedCount;
        public int totalUnits;
        public int totalUnitsPerPlayer;
        public int totalUnitsPerAI;
        public int totalUnitsPerSimulated;
    }

    [System.Serializable]
    public class PlayersBase
    {
        public string playerId;
        public PlayerTypes playerType;
        public Color playerColor;
        public int indexUnit;
        public int playerUnits;
        public Vector3[] startingPos;
        public GameObject unitPrefab;

        public enum PlayerTypes
        {
            user,
            ai,
            simulated
        }

        public List<UnitStates> allUnits = new List<UnitStates>();
    }
}
