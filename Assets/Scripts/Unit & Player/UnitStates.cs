using GridMaster;
using System;
using System.Collections;
using System.Collections.Generic;
using UnitUI;
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

        public int currentMoveCost;
        public int actions;

        public Node currentNode;

        public UnitController controller;
        public UnitNodeManager nManager;

        public UnitStats stats;
        public UnitTraits traits;

        public GameObject unitSelectIndicatorPrefab;
        GameObject unitSelectIndicator;
        public GameObject unitInfoUIPrefab;
        UnitUIHolder uiHolder;

        public void Start()
        {
            GameObject uiGO = Instantiate(unitInfoUIPrefab, transform.position, Quaternion.identity);
            uiHolder = uiGO.GetComponent<UnitUIHolder>();
            uiHolder.transform.SetParent(PlayerUI.GetInstance().transform);

            GameObject indicator = Instantiate(unitSelectIndicatorPrefab, transform.position, Quaternion.identity);
            unitSelectIndicator = indicator;
            unitSelectIndicator.transform.parent = this.transform;
        }

        public void Update()
        {
            if(!move)
            {
                EnableDisableUnitUI(true);

                Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
                uiHolder.transform.position = screenPoint;

                unitSelectIndicator.SetActive(true);

            }
            else
            {
                unitSelectIndicator.SetActive(false);
                EnableDisableUnitUI(false);
            }
        }

        public void UpdateActionPoints(int newAP)
        {
            actions = newAP;
            uiHolder.apNumber.text = actions.ToString();
        }

        private void EnableDisableUnitUI(bool v)
        {
            uiHolder.gameObject.SetActive(v);
        }

        public void InitForStartOfTurn()
        {
            UpdateActionPoints(2);
        }

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
