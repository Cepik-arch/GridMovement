using GridMaster;
using Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace UnitControl
{
    public class UnitController : MonoBehaviour
    {
        public UnitStates states;
        GridBase grid;
        public Vector3 startingPosition;

        public Node currentNode;

        public bool movePath;

        int indexPath = 0;

        public List<Node> currentPath = new List<Node>();
        public List<Node> shortPath = new List<Node>();

        public float speed = 5;

        float startTime;
        float fractLenght;
        Vector3 startPos;
        Vector3 targetPos;
        bool updatedPos;

        public void Start()
        {
            Assert.IsNotNull(GridBase.GetInstance(), "Missing Singleton!");
            grid = GridBase.GetInstance();
            states = GetComponent<UnitStates>();
            states.controller = this;
            PlaceOnNodeImmediate(startingPosition);

            states.currentNode = grid.GetNodeFromVector3(startingPosition);
        }

        public void Update()
        {
            states.move = movePath;

            // if unit has movepath, it will move
            if (movePath)
            {
                states.nManager.holdPath = true;

                UpdatePositions();
                FindCurrentNode();
                MovementAndRotation();
            }
            else
            {
                if (states.nManager.holdPath)
                {
                    states.nManager.holdPath = false;
                    states.nManager.hasMovingPath = false;
                    shortPath.Clear();
                    currentPath.Clear();

                }
            }

        }

        void FindCurrentNode()
        {
            Node curNode = grid.NodeFromWorldPosition(transform.position);
            StoreRefNode(curNode);

        }

        void UpdatePositions()
        {
            if (!updatedPos)
            {
                //Debug.Log("updatePos is false");
                if (indexPath < shortPath.Count - 1)
                {
                    indexPath++;
                }
                else
                {
                    movePath = false;
                }

                currentNode = grid.NodeFromWorldPosition(transform.position);
                startPos = currentNode.worldObject.transform.position;
                targetPos = shortPath[indexPath].worldObject.transform.position;

                fractLenght = Vector3.Distance(startPos, targetPos);
                startTime = Time.time;
                updatedPos = true;
            }
        }

        void MovementAndRotation()
        {
            float distCover = (Time.time - startTime) * states.movingSpeed;

            if (fractLenght == 0)
            {
                fractLenght = 0.1f;
            }

            float fracJourney = distCover / fractLenght;

            if (fracJourney > 1)
            {
                updatedPos = false;
            }

            Vector3 targetPosition = Vector3.Lerp(startPos, targetPos, fracJourney);
            Vector3 direction = targetPos - startPos;
            direction.y = 0;

            if (!Vector3.Equals(direction, Vector3.zero))
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * states.maxSpeed);
            }
            transform.position = targetPosition;
            

        }

        public void EvaluatePath()
        {
            Vector3 curDirection = Vector3.zero;

            for (int i = 1; i < currentPath.Count; i++)
            {
                Vector3 nextDirection = new Vector3(
                    currentPath[i - 1].x - currentPath[i].x,
                    currentPath[i - 1].y - currentPath[i].y,
                    currentPath[i - 1].z - currentPath[i].z
                    );

                if (!Vector3.Equals(nextDirection, curDirection))
                {
                    shortPath.Add(currentPath[i - 1]);
                    shortPath.Add(currentPath[i]);
                }
                curDirection = nextDirection;
            }
            shortPath.Add(currentPath[currentPath.Count - 1]);
        }
        public void ResetMovingVariables()
        {
            updatedPos = false;
            indexPath = 0;
            fractLenght = 0;
        }
        public void PlaceOnNodeImmediate(Vector3 nodePose)
        {
            int x = Mathf.CeilToInt(nodePose.x);
            int y = Mathf.CeilToInt(nodePose.y);
            int z = Mathf.CeilToInt(nodePose.z);

            Node node = grid.GetNode(x, y, z);

            transform.position = node.worldObject.transform.position;
            StoreRefNode(node);
        }
        
        Node prevNode;
        public void StoreRefNode(Node targetNode)
        {
            if (prevNode != null)
            {
                if (targetNode != prevNode)
                {
                    prevNode.nodeRef.ChangeTileMaterial(NodeReferences.TileType.none);
                    prevNode.isWalkable = true;
                    prevNode.unitOnNode = null;
                    prevNode = null;

                    targetNode.nodeRef.ChangeTileMaterial(NodeReferences.TileType.red);
                    targetNode.isWalkable = false;
                    targetNode.unitOnNode = states;
                    prevNode = targetNode;
                }
            }
            else
            {
                targetNode.nodeRef.ChangeTileMaterial(NodeReferences.TileType.red);
                targetNode.isWalkable = false;
                targetNode.unitOnNode = states;
                prevNode = targetNode;
            }
            states.currentNode = prevNode;
        }

    }
}
