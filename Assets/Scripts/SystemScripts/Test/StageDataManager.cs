using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

namespace Water
{
    public class StageDataManager : MonoBehaviour
    {
        public StageGround stageGround;

        private readonly string tileBasePath;

        public void SaveStage()
        {
            Tilemap bgTilemap = stageGround.transform.Find("BackgroundTilemap").GetComponent<Tilemap>();    
            for(int y = stageGround.limitMinPosition.y; y <= stageGround.limitMaxPosition.y; y++)
            {
                for(int x = stageGround.limitMinPosition.x; x <= stageGround.limitMaxPosition.x; x++)
                {
                    Vector3Int pos = bgTilemap.WorldToCell(new Vector3(x, y, 0));
                    TileBase tile = bgTilemap.GetTile(pos);
                    Debug.Log(tile.name);
                }
            }
        }

        public void MakeBlock(TileData tileData)
        {
            Tilemap tilemap = stageGround.transform.Find(tileData.tilemap).GetComponent<Tilemap>();
            tilemap.SetTile(tilemap.WorldToCell(new Vector3(tileData.x, tileData.y, 0)), 
                Resources.Load<TileBase>(tileBasePath));
        }

        /*private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P)) SaveStage();
        }*/
    }
}