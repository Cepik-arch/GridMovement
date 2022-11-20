using GridMaster;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class GameManager : MonoBehaviour
    {
        PlayersBase activePlayer;
        public List<PlayersBase> playersList = new List<PlayersBase>();

        void Start()
        {
            activePlayer = playersList[0];
        }

        void Update()
        {

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
    public class PlayersBase
    {
        public string playerId;
        public PlayerTypes playerType;

        public enum PlayerTypes
        {
            user,
            ai,
            simulated
        }
    }
}
