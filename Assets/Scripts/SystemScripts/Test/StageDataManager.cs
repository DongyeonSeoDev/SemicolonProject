using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

namespace Water
{
    public class StageDataManager : MonoBehaviour
    {
        public StageGround stageGround;

        private readonly string tileBasePath = "TileBase/";

        public void SaveStage()
        {
            Tilemap[] tilemaps = stageGround.transform.GetComponentsInChildren<Tilemap>(); 
            for(int i=0; i < tilemaps.Length; i++)
            {
                for (int y = stageGround.limitMinPosition.y; y <= stageGround.limitMaxPosition.y; y++)
                {
                    for (int x = stageGround.limitMinPosition.x; x <= stageGround.limitMaxPosition.x; x++)
                    {
                        Vector3Int pos = tilemaps[i].WorldToCell(new Vector3(x, y, 0));
                        TileBase tile = tilemaps[i].GetTile(pos);
                        Debug.Log(tile.name);
                    }
                }
            }
        }

        public void MakeBlock(StageBaseData data)
        {
            for(int i=0; i<data.tilemaps.Count; i++)
            {
                Tilemap tilemap = stageGround.transform.Find(data.tilemaps[i].tilemap).GetComponent<Tilemap>();
                for(int j=0; j<data.tilemaps[i].tileGroups.Count; j++)
                {
                    TileBaseGroup tbg = data.tilemaps[i].tileGroups[j];
                    TileBase tileBase = Resources.Load<TileBase>(tileBasePath + tbg.tileBase);
                    for(int k = 0; k<tbg.tileDataList.Count; k++)
                    {
                        TileData tileData = tbg.tileDataList[k];
                        tilemap.SetTile(tilemap.WorldToCell(new Vector3(tileData.x, tileData.y, 0)), tileBase);
                    }
                }
            }

           // tilemap.SetTile(tilemap.WorldToCell(new Vector3(tileData.x, tileData.y, 0)), 
             //   Resources.Load<TileBase>(tileBasePath));
        }

        /*private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P)) SaveStage();
        }*/
    }
}