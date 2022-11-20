using GridMaster;
using Pathfinding;
using System.Collections.Generic;
using UnitControl;
using UnityEngine;
using UnityEngine.Assertions;
using WorldUI;

namespace Player
{
    public class PlayerInteractions : MonoBehaviour
    {

        public UnitController activeUnit;

        public bool hasPath;
        public bool holdPath;

        PathfindMaster pathFinder;
        GridBase grid;
        GameManager gameManager;
        WorldUIManager wUIManager;

        Node prevNode;

        public bool visualizePath;
        public GameObject lineGO;
        LineRenderer line;
        Node startNode;
        Node targetNode;

        public void Start()
        {
            Assert.IsNotNull(GridBase.GetInstance(), "Missing Singleton!");
            grid = GridBase.GetInstance();
            Assert.IsNotNull(PathfindMaster.GetInstance(), "Missing Singleton!");
            pathFinder = PathfindMaster.GetInstance();
            Assert.IsNotNull(GameManager.GetInstance(), "Missing Singleton!");
            gameManager = GameManager.GetInstance();
            Assert.IsNotNull(WorldUIManager.GetInstance(), "Missing Singleton!");
            wUIManager = WorldUIManager.GetInstance();
        }

        void Update()
        {   /*
            if (grid == null && pathFinder == null)
            {
                grid = GridBase.GetInstance();
                pathFinder = PathfindMaster.GetInstance();
                gameManager = GameManager.GetInstance();
                wUIManager = WorldUIManager.GetInstance();
            }
            */
            FindUnit();
            MoveUnit();
        }
        void FindUnit()
        {
            if (!activeUnit)
            {
                if (Input.GetMouseButton(0))
                {
                    Node node = FindNodeFromMousePosition();
                    if (node != null)
                    {
                        Debug.Log(node.x.ToString() + " " + node.y.ToString() + " " + node.z.ToString());
                        if (node.unitOnNode != null)
                        {
                            if (gameManager.CompareIDwithActivePlayer(node.unitOnNode.playerID))
                            {
                                activeUnit = node.unitOnNode.controller;
                                activeUnit.states.selected = true;
                                //activeUnit.states.currentNode = node; // moje
                            }
                        }
                    }
                }
            }
        }
        void MoveUnit()
        {
            if (activeUnit && !activeUnit.states.move)
            {
                //adding path to unit
                if (!hasPath)
                {
                    startNode = grid.NodeFromWorldPosition(activeUnit.transform.position);
                    targetNode = FindNodeFromMousePosition();

                    if (!activeUnit.states.nManager.basicMoveNodes.Contains(targetNode)
                        && !activeUnit.states.nManager.doubleMoveNodes.Contains(targetNode))
                    {
                        targetNode = null;
                    }

                    //if obstacle is on the targetNode then targetNode is null
                    if (targetNode != null && startNode != null)
                    {
                        if (prevNode != targetNode && !holdPath)
                        {
                            pathFinder.RequestPathfind(startNode, targetNode, PopulatePathOfActiveUnit);
                            prevNode = targetNode;
                            hasPath = true;
                            //Debug.Log(targetNode.x.ToString() + " " + targetNode.y.ToString() + " " + targetNode.z.ToString());
                        }
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    targetNode = FindNodeFromMousePosition();
                    if (targetNode.unitOnNode == null)
                    {
                        holdPath = !holdPath;
                    }
                    else
                    {
                        if (gameManager.CompareIDwithActivePlayer(targetNode.unitOnNode.playerID))
                        {
                            if (targetNode.unitOnNode.controller != activeUnit)
                            {
                                wUIManager.ClearNodeList();
                                activeUnit.states.selected = false;
                                activeUnit.states.nManager.hasMovingPath = false;
                                activeUnit = targetNode.unitOnNode.controller;
                                activeUnit.states.selected = true;
                                activeUnit.states.nManager.hasMovingPath = false;
                            }
                        }
                    }

                }

                if (activeUnit.shortPath.Count < 1)
                    holdPath = false;

                if (visualizePath)
                {
                    if (line == null)
                    {
                        GameObject go = Instantiate(lineGO, transform.position, Quaternion.identity) as GameObject;
                        line = go.GetComponent<LineRenderer>();
                    }
                    else
                    {
                        line.positionCount = activeUnit.shortPath.Count;

                        for (int i = 0; i < activeUnit.shortPath.Count; i++)
                        {
                            line.SetPosition(i, activeUnit.shortPath[i].worldObject.transform.position);
                        }
                    }
                }
            }
        }
        public void PopulatePathOfActiveUnit(List<Node> nodes)
        {
            activeUnit.currentPath.Clear();
            activeUnit.shortPath.Clear();

            activeUnit.currentPath.Add(
                grid.NodeFromWorldPosition(activeUnit.transform.position)
                );

            for (int i = 0; i < nodes.Count; i++)
            {
                activeUnit.currentPath.Add(nodes[i]);
            }

            activeUnit.EvaluatePath();
            activeUnit.ResetMovingVariables();
            hasPath = false;
        }
        public Node FindNodeFromMousePosition()
        {
            Node retVal = null;
            //set position based on mouse and camera view
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 200))
            {
                retVal = grid.NodeFromWorldPosition(hit.point);
            }
            return retVal;
        }

    }
}
