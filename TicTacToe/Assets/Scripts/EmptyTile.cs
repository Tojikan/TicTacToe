using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyTile : MonoBehaviour
{
    [SerializeField]
    private Vector2Int tileValue;                    //the value of this tile in regards to the board array
    public Vector2Int TileValue                      //property accessor for tileValue
    { get { return tileValue; }
      set { tileValue = value; }}
}
