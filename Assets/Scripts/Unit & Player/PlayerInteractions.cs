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

        public bool moveUnit;
        public bool hasPath;
        public bool holdPath;

        PathfindMaster pathFinder;
        GridBase grid;
        GameManager gameManager;
        WorldUIManager wUIManager;

        Node prevNode;
        Node startNode;
        Node targetNode;

        public bool visualizePath;
        public GameObject lineGO;
        LineRenderer line;

        public GameObject moveIndicatorPrefav;
        GameObject moveIndicator;


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

            moveIndicator = Instantiate(moveIndicatorPrefav, transform.position, Quaternion.identity) as GameObject;
            DisableMoveIndicator();
        }

        void Update()
        {
            FindUnit();
            VisualizePath();
            MoveUnit();
            UnitActualMovement();
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
                            }
                        }
                    }
                }
            }

        }
        private void MoveUnit()
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

                            //calculate move AP cost
                            activeUnit.states.currentMoveCost = (activeUnit.states.nManager.doubleMoveNodes.Contains(targetNode)) ? 2 : 1;

                            hasPath = true;
                        }
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    targetNode = FindNodeFromMousePosition();
                    if (targetNode != null)
                    {
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

                }

                if (activeUnit.shortPath.Count < 1)
                    holdPath = false;
            }
        }

        private void UnitActualMovement()
        {
            if (activeUnit && activeUnit.states.selected && moveUnit)
            {
                if (activeUnit.states.actions > 0)
                {
                    if (!activeUnit.movePath)
                    {
                        activeUnit.movePath = true;
                        holdPath = false;
                        DisableMoveIndicator();

                        //reduce AP from unit
                        activeUnit.states.UpdateActionPoints(activeUnit.states.actions - activeUnit.states.currentMoveCost);
                    }
                }
                moveUnit = false;
            }
        }

        private void VisualizePath()
        {
            if (line == null)
            {
                GameObject go = Instantiate(lineGO, transform.position, Quaternion.identity) as GameObject;
                line = go.GetComponent<LineRenderer>();
            }

            visualizePath = ControlVisualizationOfPath();

            if (visualizePath)
            {
                line.gameObject.SetActive(true);
                line.positionCount = activeUnit.shortPath.Count;

                for (int i = 0; i < activeUnit.shortPath.Count; i++)
                {
                    line.SetPosition(i, activeUnit.shortPath[i].worldObject.transform.position);
                }

                if(activeUnit.shortPath.Count > 0)
                {
                    EnableMoveIndicator(activeUnit.shortPath[activeUnit.shortPath.Count - 1].worldObject.transform.position);
                }
                else
                {
                    line.gameObject.SetActive(false);
                    DisableMoveIndicator();
                }

            }
            else
            {
                line.gameObject.SetActive(false);
                DisableMoveIndicator();
            }
        }

        private bool ControlVisualizationOfPath()
        {
            bool retVal = true;

            if (activeUnit == null)
            {
                retVal = false;
            }
            else
            {
                if (activeUnit.movePath)
                    retVal = false;
            }
            return retVal;
        }

        public void ChangeActiveUnit(UnitController targetUnit)
        {
            wUIManager.ClearNodeList();

            if (activeUnit != null)
            {
                activeUnit.states.selected = false;
                activeUnit.states.nManager.hasMovingPath = false;
            }

            activeUnit = targetUnit;
            activeUnit.states.selected = true;
            activeUnit.states.nManager.hasMovingPath = false;
        }

        public void ClearActiveUnit()
        {
            if (activeUnit != null)
            {
                wUIManager.ClearNodeList();

                activeUnit.states.selected = false;
                activeUnit.states.nManager.hasMovingPath = false;
                activeUnit = null;
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

        public void DisableMoveIndicator()
        {
            moveIndicator.SetActive(false);
            visualizePath = false;
        }

        public void EnableMoveIndicator(Vector3 targetPos)
        {
            moveIndicator.transform.position = targetPos;
            moveIndicator.SetActive(true);
        }

        #region Singleton
        public static PlayerInteractions instance;
        public static PlayerInteractions GetInstance()
        {
            return instance;
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
            }

        }
        #endregion
    }
}
