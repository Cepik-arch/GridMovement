using GridMaster;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using WorldUI;
using System;

namespace UnitControl
{
    public class UnitNodeManager : MonoBehaviour
    {
        UnitStates states;
        GridBase grid;

        public bool holdPath;
        public bool hasMovingPath;

        int distanceFromMain;

        List<WorldUIManager.MovementNodes> movementNodes = new List<WorldUIManager.MovementNodes>();
        public List<Node> basicMoveNodes = new List<Node>();
        public List<Node> doubleMoveNodes = new List<Node>();

        WorldUIManager wUI;

        void Start()
        {
            states = GetComponent<UnitStates>();
            states.nManager = this;

            Assert.IsNotNull(GridBase.GetInstance(), "Missing Singleton!");
            grid = GridBase.GetInstance();

            Assert.IsNotNull(WorldUIManager.GetInstance(), "Missing Singleton!");
            wUI = WorldUIManager.GetInstance();
        }
        void Update()
        {
            if (states.selected)
            {
                if (!hasMovingPath && !holdPath)
                {
                    ClearListOfNodes();
                    movementNodes = wUI.FindAvailableNodes(states);

                    for (int i = 0; i < movementNodes.Count; i++)
                    {
                        if (movementNodes[i].distance < states.stats.basicMoveDistance)
                        {
                            basicMoveNodes.Add(movementNodes[i].nodeActual);
                            wUI.UpdateNodeStatus(movementNodes[i].nodeActual, NodeReferences.TileType.blue);
                        }
                        else
                        {
                            doubleMoveNodes.Add(movementNodes[i].nodeActual);
                            wUI.UpdateNodeStatus(movementNodes[i].nodeActual, NodeReferences.TileType.yellow);
                        }
                    }
                    hasMovingPath = true;
                }
                if (holdPath)
                {
                    if (movementNodes.Count>0)
                    {
                        wUI.ClearNodeList();
                        movementNodes.Clear();
                    }
                }
            }
            else
            {
                if (hasMovingPath)
                {
                    wUI.ClearNodeList();
                    hasMovingPath = false;
                }
            }
        }

        public void ClearListOfNodes()
        {
            movementNodes.Clear();
            basicMoveNodes.Clear();
            doubleMoveNodes.Clear();
        }
    }
}
