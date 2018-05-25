using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerTiles", menuName = "Data/Tiles", order = 1)]
public class PlayerTileSet : ScriptableObject
{
    public GameObject[] tiles;
}
