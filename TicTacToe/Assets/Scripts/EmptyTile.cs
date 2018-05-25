using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class that handles empty tile functions.
//When a tile is created in board generation, it'll get assigned a Vector2Int variable that gives the indices in the 2D array tracked in BoardState.
public class EmptyTile : MonoBehaviour
{
    [SerializeField]
    private Vector2Int tileValue;                    //the value of this tile in regards to the board array
    public Vector2Int TileValue                      //property accessor for tileValue
    { get { return tileValue; }
      set { tileValue = value; }}
    public SpriteRenderer spriteRender;              //The sprite render component

    private void Awake()
    {
        spriteRender = GetComponent<SpriteRenderer>();
    }

    private void OnMouseOver()
    {
        if (GameManager.InputEnabled)
        {

        }
    }
}
