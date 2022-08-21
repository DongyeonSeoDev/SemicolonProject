using System;
using UnityEngine.Tilemaps;
using UnityEngine;

namespace Water
{
    [Serializable]
    public class TileData
    {
        public int x;
        public int y;
        public string tilemap;
        public string tileBase;

        public TileData() { }
        public TileData(Vector3Int pos, Tilemap tilemap, TileBase tile)
        {
            x = pos.x;  
            y = pos.y;  
            this.tilemap = tilemap.name;
            this.tileBase = tile.name;
        }
    }
}