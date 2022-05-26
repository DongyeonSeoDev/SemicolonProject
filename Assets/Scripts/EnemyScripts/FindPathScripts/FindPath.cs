using UnityEngine;
using UnityEngine.Tilemaps;

public class FindPath : MonoBehaviour
{
    public Tilemap tilemap;

    private void Start()
    {
        BoundsInt cellBounds = tilemap.cellBounds;

        for (int i = cellBounds.min.x; i <= cellBounds.max.x; i++)
        {
            for (int j = cellBounds.min.y; j <= cellBounds.max.y; j++)
            {
                if ((tilemap.GetTile(new Vector3Int(i, j, 0)) != null) || (tilemap.GetTile(new Vector3Int(i - 1, j, 0)) != null) || (tilemap.GetTile(new Vector3Int(i, j - 1, 0)) != null) || (tilemap.GetTile(new Vector3Int(i - 1, j - 1, 0)) != null))
                {
                    Debug.Log(new Vector2(i, j));
                }
            }
        }
    }
}