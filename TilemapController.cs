using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapController : MonoBehaviour
{
    Tilemap tileMap;
    public TileBase tile;

    // Start is called before the first frame update
    void Start()
    {
        tileMap = GetComponent<Tilemap>();
    }
    
    public void DestroyDot(Vector3 pos)
    {
        Vector3Int cellPos = tileMap.WorldToCell(pos);
        tileMap.SetTile(cellPos, null);
    }

    public void RespawnTile(Vector3 pos)
    {
        Vector3Int cellPos = tileMap.WorldToCell(pos);
        tileMap.SetTile(cellPos, tile);
    }
}