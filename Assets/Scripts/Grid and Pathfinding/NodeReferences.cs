using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace GridMaster
{
    public class NodeReferences : MonoBehaviour
    {
        public MeshRenderer tileRender;
        public Material[] tileMaterials;
        
        public void ChangeTileMaterial(TileType type)
        {
            tileRender.enabled = true;

            switch(type)
            {
                case TileType.red:
                    tileRender.material = tileMaterials[0];
                    break;
                case TileType.blue:
                    tileRender.material = tileMaterials[1];
                    break;
                case TileType.yellow:
                    tileRender.material = tileMaterials[2];
                    break;
                case TileType.none:
                    tileRender.enabled = false;
                    break;

            }
        }

        public TileType tileType;
        public enum TileType
        {
            red,
            blue,
            yellow,
            none
        }



    }
}
