using GridMaster;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Node = GridMaster.Node;

namespace WorldUI
{

    public class WorldUIManager : MonoBehaviour
    {
        GridBase grid = null;

        void Start()
        {
            grid = GridBase.GetInstance();
            Assert.IsNotNull(GridBase.GetInstance(), "Missing Singleton!");
        }
        void Update()
        {
        }

        List<MovementNodes> movementNodes = new List<MovementNodes>();

        #region Path Visualization
        public void UpdateNodeStatus(Node node, NodeReferences.TileType target)
        {
            node.nodeRef.ChangeTileMaterial(target);
        }

        public void UpdateListNodesStatus(List<Node> node, NodeReferences.TileType target)
        {
            for (int i = 0; i < node.Count; i++)
            {
                if (node[i].isWalkable)
                {
                    node[i].nodeRef.ChangeTileMaterial(target);
                }
            }
        }

        public List<MovementNodes> FindAvailableNodes(UnitControl.UnitStates states)
        {
            //int distMax = states.stats.basicMoveDistance + states.stats.doubleMoveDistance;
            movementNodes = GetNeighbourNodes(states, 10, true);
            return movementNodes;
        }

        private List<MovementNodes> GetNeighbourNodes(UnitControl.UnitStates states, int dist, bool isNextVertical = false)
        {
            List<MovementNodes> neighbours = new List<MovementNodes>();
            for (int x = -dist; x <= dist; x++)
            {
                for (int yTmp = -dist; yTmp <= dist; yTmp++)
                {
                    for (int z = -dist; z <= dist; z++)
                    {

                        int y = yTmp;
                        if (!isNextVertical)
                        {
                            y = 0;
                        }
                        if (x == 0 && y == 0 && z == 0)
                        {
                            //is curent node
                        }
                        else
                        {
                            int X = states.currentNode.x + x;
                            int Y = states.currentNode.y + y;
                            int Z = states.currentNode.z + z;

                            Node node = grid.GetNode(X, Y, Z);

                            if (node != null)
                            {
                                if (node.isWalkable)
                                {
                                    int distance = GetDistance(states.currentNode, node);

                                    if (distance < states.stats.basicMoveDistance + states.stats.doubleMoveDistance)
                                    {
                                        MovementNodes moveNode = new MovementNodes();
                                        moveNode.nodeActual = node;
                                        moveNode.distance = distance;

                                        if (!neighbours.Contains(moveNode))
                                        {
                                            neighbours.Add(moveNode);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return neighbours;
        }
        public void ClearNodeList()
        {
            for (int i = 0; i < movementNodes.Count; i++)
            {
                movementNodes[i].nodeActual.nodeRef.ChangeTileMaterial(NodeReferences.TileType.none);
            }
            movementNodes.Clear();
        }

        private int GetDistance(Node posA, Node posB)
        {
            //We find the distance between each node

            int distX = Mathf.Abs(posA.x - posB.x);
            int distZ = Mathf.Abs(posA.z - posB.z);
            int distY = Mathf.Abs(posA.y - posB.y);

            if (distX > distZ)
            {
                return 14 * distZ + 10 * (distX - distZ) + 10 * distY;
            }

            return 14 * distX + 10 * (distZ - distX) + 10 * distY;
        }

        [Serializable]
        public class MovementNodes : IEquatable<MovementNodes>
        {
            public Node nodeActual; 

            public int distance; 

            public bool Equals(MovementNodes obj)
            {
                return (obj.nodeActual.x == nodeActual.x &&
                    obj.nodeActual.y == nodeActual.y &&
                    obj.nodeActual.z == nodeActual.z);
            }
        }

        #endregion

        #region Singleton
        private static WorldUIManager instance = null;
        public static WorldUIManager GetInstance()
        {
            return instance;
        }
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion

    }
}
