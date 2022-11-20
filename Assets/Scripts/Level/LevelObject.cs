using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelControl
{
    public class LevelObject : MonoBehaviour
    {
        public lvlObjectType objType;

        public enum lvlObjectType
        {
            floor,
            obstacle,
            wall
        }


    }
}
