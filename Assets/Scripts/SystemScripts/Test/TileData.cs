using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

namespace Water
{
    [Serializable]
    public class TileData
    {
        public int x;
        public int y;

        public TileData() { }
        public TileData(Vector3Int pos)
        {
            x = pos.x;  
            y = pos.y;  
        }
    }

    [Serializable]
    public class TileBaseGroup
    {
        public string tileBase;
        public List<TileData> tileDataList;
    }

    [Serializable]
    public class TilemapData
    {
        public string tilemap;
        public List<TileBaseGroup> tileGroups;
    }

    [Serializable]
    public class StageBaseData
    {
        public List<TilemapData> tilemaps;
        
    }
}