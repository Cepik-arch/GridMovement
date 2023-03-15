using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LevelControl;
using static GridMaster.NodeReferences;

namespace GridMaster
{
    public class GridBase : MonoBehaviour
    {
        //Setting up the grid
        public int maxX;
        public int maxY;
        public int maxZ;

        //Offset relates to the world positions only
        public float offsetX;
        public float offsetY;
        public float offsetZ;

        public Node[,,] grid; // our grid

        public GameObject gridFloorPrefab;

        public Vector3 startNodePosition;
        public Vector3 endNodePosition;

        public int enabledY;
        List<GameObject> YCollisions = new List<GameObject>();

        public int agents;

        LevelManager lvlManager;

        public void InitGrid(LevelControl.LevelInitializer.GridStats gridStats)
        {
            maxX = gridStats.maxX;
            maxY = gridStats.maxY;
            maxZ = gridStats.maxZ;

            offsetX = gridStats.offsetX;
            offsetY = gridStats.offsetY;
            offsetZ = gridStats.offsetZ;

            lvlManager = LevelManager.GetInstance();

            CreateGrid();
            CreateMouseCollision();
            CloseAllMouseCollisions();

            YCollisions[enabledY].SetActive(true);
        }

        void Start()
        {

        }

        //Just a quick and dirty way to visualize the path
        //public bool start;
        void Update()
        {
         /*   if (start)
            {
                start = false;
                //Create the new pathfinder class
                // Pathfinding.Pathfinder path = new Pathfinding.Pathfinder();

                //to test the avoidance, just make a node unwalkable
                grid[1, 0, 1].isWalkable = false;

                //pass the target nodes
                Node startNode = GetNodeFromVector3(startNodePosition);
                Node end = GetNodeFromVector3(endNodePosition);

                //path.startPosition = startNode;
                //path.endPosition = end;

                //find the path
                //List<Node> p = path.FindPath();
                startNode.worldObject.SetActive(false);

                for (int i = 0; i < agents; i++)
                {
                    Pathfinding.PathfindMaster.GetInstance().RequestPathfind(startNode, end, ShowPath);
                }
            }*/
        }

        public void ShowPath(List<Node> path)
        {
            foreach (Node n in path)
            {
                n.worldObject.SetActive(false);
            }

            //Debug.Log("agent complete");
        }

        public Node GetNode(int x, int y, int z)
        {
            //Used to get a node from a grid,
            //If it's greater than all the maximum values we have
            //then it's going to return null

            Node retVal = null;

            if (x < maxX && x >= 0 &&
                y >= 0 && y < maxY &&
                z >= 0 && z < maxZ)
            {
                retVal = grid[x, y, z];
            }

            return retVal;
        }

        public Node NodeFromWorldPosition(Vector3 worldPosition)
        {
            float worldX = worldPosition.x;
            float worldY = worldPosition.y;
            float worldZ = worldPosition.z;

            worldX /= offsetX;
            worldY /= offsetY;
            worldZ /= offsetZ;

            int x = Mathf.RoundToInt(worldX);
            int y = Mathf.RoundToInt(worldY);
            int z = Mathf.RoundToInt(worldZ);

            if(x > maxX - 1)
                x = maxX - 1;
            if(y > maxY - 1)
                y = maxY - 1;
            if(z > maxZ - 1)
                z = maxZ - 1;
            if(x < 0)
                x = 0;
            if(y < 0)
                y = 0;
            if(z < 0)
                z = 0;

            return grid[x, y, z];
        }

        public Node GetNodeFromVector3(Vector3 pos)
        {
            int x = Mathf.RoundToInt(pos.x);
            int y = Mathf.RoundToInt(pos.y);
            int z = Mathf.RoundToInt(pos.z);

            Node retVal = GetNode(x, y, z);
            return retVal;
        }

        void CreateGrid()
        {
            //The typical way to create a grid
            grid = new Node[maxX, maxY, maxZ];

            for (int i = 0; i < maxY; i++)
            {

                lvlManager.lvlObjects.Add(new ObjsPerFloor());
                lvlManager.lvlObjects[i].floorIndex = i;

            }

            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    for (int z = 0; z < maxZ; z++)
                    {
                        //Apply the offsets and create the world object for each node
                        float posX = x * offsetX;
                        float posY = y * offsetY;
                        float posZ = z * offsetZ;
                        GameObject go = Instantiate(gridFloorPrefab, new Vector3(posX, posY, posZ),
                            Quaternion.identity) as GameObject;
                        //Rename it
                        go.transform.name = x.ToString() + " " + y.ToString() + " " + z.ToString();
                        //and parent it under this transform to be more organized
                        go.transform.parent = transform;

                        //Create a new node and update it's values
                        Node node = new Node();
                        node.x = x;
                        node.y = y;
                        node.z = z;
                        node.worldObject = go;
                        node.nodeRef = go.GetComponentInChildren<NodeReferences>();
                        node.isWalkable = false;
                        node.nodeRef.tileRender.enabled = false;

                        RaycastHit[] hits = Physics.BoxCastAll(
                            new Vector3(posX, posY, posZ)
                           ,new Vector3(0.5f,0.5f,0.5f)
                           ,Vector3.up);

                        for (int i = 0; i < hits.Length; i++)
                        {
                            if (hits[i].transform.GetComponent<LevelObject>())
                            {
                                LevelObject lvlObj = hits[i].transform.GetComponent<LevelObject>();

                                if (!lvlManager.lvlObjects[y].objs.Contains(lvlObj.gameObject))
                                {
                                    lvlManager.lvlObjects[y].objs.Add(lvlObj.gameObject);
                                }

                                node.nodeRef.tileRender.enabled = true;

                                //sets if object is walkable and how is node shown
                                if(lvlObj.objType == LevelObject.lvlObjectType.obstacle)
                                {
                                    node.isWalkable = false;
                                    node.nodeRef.ChangeTileMaterial(TileType.red);
                                    break;
                                }

                                if (lvlObj.objType == LevelObject.lvlObjectType.floor)
                                {
                                    node.isWalkable = true;
                                    node.nodeRef.ChangeTileMaterial(TileType.none); 
                                }

                            }
                        }

                        //then place it to the grid
                        grid[x, y, z] = node;
                    }
                }
            }
        }

        void CreateMouseCollision()
        {
            for (int y = 0; y < maxY; y++)
            {
                GameObject go = new GameObject();
                go.transform.name = "Collision for Y " + y.ToString();
                go.AddComponent<BoxCollider>();
                go.GetComponent<BoxCollider>().size = new Vector3(maxX * offsetX, .1f, maxZ * offsetZ);

                go.transform.position = new Vector3((maxX * offsetX) / 2 - offsetX / 2,
                    y * offsetY,
                    (maxZ * offsetZ)/2 - offsetZ);

                YCollisions.Add(go);

            }
        }
        
        void CloseAllMouseCollisions()
        {
            for (int i = 0; i < YCollisions.Count; i++)
            {
                YCollisions[i].SetActive(false);
            }
        }

        public List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            // Check all 26 adjacent nodes (including diagonals)
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        if (x == 0 && y == 0 && z == 0)
                        {
                            continue; // Skip the current node
                        }

                        int checkX = node.x + x;
                        int checkY = node.y + y;
                        int checkZ = node.z + z;

                        // Check if the adjacent node is within the grid bounds
                        if (checkX >= 0 && checkX < maxX && checkY >= 0 && checkY < maxY && checkZ >= 0 && checkZ < maxZ)
                        {
                            neighbours.Add(grid[checkX, checkY, checkZ]);
                        }
                    }
                }
            }

            return neighbours;
        }


        //Singleton
        public static GridBase instance;
        void Awake()
        {
            instance = this;
            Debug.Log("Instance GridBase");
            Debug.Log(instance.ToString());
            DontDestroyOnLoad(gameObject);
        }

        public static GridBase GetInstance()
        {

            return instance;
        }

    }
}