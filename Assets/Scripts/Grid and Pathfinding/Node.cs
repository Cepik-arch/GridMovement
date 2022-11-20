using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using UnitControl;
using UnityEngine;

namespace GridMaster
{
    public class Node
    {
        //node position
        public int x;
        public int y;
        public int z;

        //node cost foor pathfinding
        public float hCost;
        public float gCost;

        public float fCost
        {
            //fCost = hCost + gCost
            get { return hCost + gCost;} 
        }

        public Node parentNode;
        public bool isWalkable = true;

        //get position of the node from world object
        public GameObject worldObject;
        public NodeReferences nodeRef;
        public UnitStates unitOnNode;

        public NetBiosNodeType nodeType;
        public enum NodeType
        {
            ground,
            air
        };

    }
}
