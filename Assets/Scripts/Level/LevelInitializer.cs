using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace LevelControl
{
    public class LevelInitializer : MonoBehaviour
    {
        public GameObject gridBasePrefab;

        public int unitCount = 1;
        public GameObject unitPrefab;

        private WaitForEndOfFrame waitEF;

        public GridStats gridStats;

        [System.Serializable ]
        public class GridStats
        {
            //Setting up the grid
            public int maxX = 10;
            public int maxY = 3;
            public int maxZ = 10;

            //Offset relates to the world positions only
            public float offsetX = 1;
            public float offsetY = 1;
            public float offsetZ = 1;
        }

        void Start()
        {
            waitEF = new WaitForEndOfFrame();
            StartCoroutine("InitLevel");
        }

        IEnumerator InitLevel()
        {
            yield return StartCoroutine(CreateGrid());
            yield return StartCoroutine(CreateUnits());
            yield return StartCoroutine(EnableWorldUI());
            yield return StartCoroutine(EnablePlayerInteractions());
        }


        IEnumerator CreateGrid()
        {
            GameObject go = Instantiate(gridBasePrefab,  Vector3.zero, Quaternion.identity) as GameObject;
            go.GetComponent<GridMaster.GridBase>().InitGrid(gridStats);
            yield return waitEF;
        }

        IEnumerator CreateUnits()
        {
            for (int i = 0; i < unitCount; i++)
            {
                GameObject go = Instantiate(unitPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                UnitControl.UnitController uc = go.GetComponent<UnitControl.UnitController>();
                uc.startingPosition.x += i * 2;
            }
            yield return waitEF;
        }

        IEnumerator EnablePlayerInteractions()
        {
            GetComponent<Player.PlayerInteractions>().enabled = true;
            yield return waitEF;
        }
        
        IEnumerator EnableWorldUI()
        {
            GetComponent<WorldUI.WorldUIManager>().enabled = true;
            yield return waitEF;
        }
    }
}
