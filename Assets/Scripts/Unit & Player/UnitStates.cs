using GridMaster;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnitControl
{
    [RequireComponent(typeof(HandleAnimations))]
    public class UnitStates : MonoBehaviour
    {
        public string playerID;
        public int health;
        public bool selected = false;
        public bool hasPath;
        public bool move;

        public float movingSpeed;
        public float maxSpeed = 6;

        public Node currentNode;

        public UnitController controller;
        public UnitNodeManager nManager;

        public UnitStats stats;
        public UnitTraits traits;

        [System.Serializable]
        public class UnitStats
        {
            public int basicMoveDistance;
            public int doubleMoveDistance;

        }

        [System.Serializable]
        public class UnitTraits
        {
            public bool runAndGun;
        }
    }
}
