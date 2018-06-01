using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class that handles empty tile functions.
//When a tile is created in board generation, it'll get assigned a Vector2Int variable that gives the indices in the 2D array tracked in BoardState.
//Now handles the spawning of tile prefabs when clicked on
public class EmptyTile : MonoBehaviour
{
    [SerializeField]
    private Vector2Int tileValue;                    //the value of this tile in regards to the board array
    public Vector2Int TileValue                      //property accessor for tileValue
    { get { return tileValue; }
      set { tileValue = value; }}

    //spawns a tile prefab at the this location when clicked depending on player turn. Note that this is separate from what the game uses to track piece positions
    public void SpawnTile()
    {
        //the selected tile, matched to the index of the playerIcons array object.
        int playerTile = (GameManager.CurrentPlayer == GameManager.Player.P1) ? GameManager.instance.PlayerOneIcon : GameManager.instance.PlayerTwoIcon;
        playerTile = Mathf.Clamp(playerTile, 0, 3);
        GameObject tilePrefab;
        //decide which size icon to use for instantiating
        if (BoardState.BoardDimension < 5)
            tilePrefab = GameManager.instance.iconSet.largeIcons[playerTile];
        else
            tilePrefab = GameManager.instance.iconSet.smallIcons[playerTile];

        //instantiate at this location with same rotation and set under the Board Generation Game Object
        Instantiate(tilePrefab, transform.position, Quaternion.identity, GameManager.instance.boardGeneration.gameObject.transform);
        
        //Play Audio
        AudioManager.instance.PlayTileDrop();

        //Destroy After Reading
        Destroy(gameObject);
    }
}
