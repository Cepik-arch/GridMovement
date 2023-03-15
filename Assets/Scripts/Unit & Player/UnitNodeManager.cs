using GridMaster;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using WorldUI;

namespace UnitControl
{
    public class UnitNodeManager : MonoBehaviour
    {
        UnitStates states;

        public bool holdPath;
        public bool hasMovingPath;

        List<WorldUIManager.MovementNodes> movementNodes = new List<WorldUIManager.MovementNodes>();
        public List<Node> basicMoveNodes = new List<Node>();
        public List<Node> doubleMoveNodes = new List<Node>();

        WorldUIManager wUI;

        void Start()
        {
            states = GetComponent<UnitStates>();
            states.nManager = this;

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

                    if (states.actions > 0)
                    {
                        // actions = number of AP
                        switch (states.actions)
                        {
                            case 1:
                                for (int i = 0; i < movementNodes.Count; i++)
                                {
                                    if (movementNodes[i].distance < states.stats.basicMoveDistance)
                                    {
                                        PopulateListOfNodes(doubleMoveNodes, movementNodes[i].nodeActual, NodeReferences.TileType.yellow); 
                                    }
                                }
                                break;
                            case 2: 
                                for (int i = 0; i < movementNodes.Count; i++)
                                {
                                    if (movementNodes[i].distance < states.stats.basicMoveDistance)
                                    {
                                        PopulateListOfNodes(basicMoveNodes, movementNodes[i].nodeActual, NodeReferences.TileType.blue);
                                    }
                                    else
                                    {
                                        PopulateListOfNodes(doubleMoveNodes, movementNodes[i].nodeActual, NodeReferences.TileType.yellow);
                                    }
                                }
                                break;
                        }
                    }
                    hasMovingPath = true;
                }
                if (holdPath)
                {
                    if (movementNodes.Count > 0)
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

        private void PopulateListOfNodes(List<Node> nodes,Node n, NodeReferences.TileType type)
        {
            nodes.Add(n);
            wUI.UpdateListNodesStatus(nodes, type);
        }
    }
}
