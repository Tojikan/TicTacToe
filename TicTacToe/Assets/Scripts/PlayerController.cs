using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles all of the player input and actions on click and such.  
public class PlayerController : MonoBehaviour
{
    public PlayerIconSet playerIcons;                                   //Drag the scriptable object that references the player icon piece prefabs here
    public GameObject board;                                            //Drag the board parent object here (wherever the BoardGeneration script is so we only need one clear board function)

    #region Mouse input for clicking tiles
    void Update()
    {
       //if allowing input
        if (GameManager.InputEnabled)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //get position
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 100f);
                if (hit)
                {
                    //if tile, go through the functions and disable the tile after done. 
                    if (hit.transform.tag == "EmptyTile")
                    {
                        GameManager.instance.DisableControls();
                        AudioManager.instance.PlayTileDrop();
                        TileOnClick(hit);
                        hit.transform.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
    #endregion

    #region Methods that occur on click

    //Contains all of the methods and steps that occur when an empty tile is clicked
    private void TileOnClick(RaycastHit2D hit)
    {
        //Gets the tile value data from the tile that's been clicked
        EmptyTile emptyTile = hit.transform.GetComponent<EmptyTile>();
        Vector2Int tileValue = emptyTile.TileValue;
        
        //try to add to board array - if error, then end the current match.
        try
        {
            BoardState.AddPosition(tileValue, (int)GameManager.CurrentPlayer + 1);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            GameManager.instance.GameError();
        }
        //Record the move to game data
        GameDataRecorder.instance.AddPlayerMove(tileValue);

        //Call the tiles spawn function 
        emptyTile.SpawnTile();

        //Goes back to Game Manager which uses BoardState to check if the game is over and then calls the corresponding win/draw animation
        GameManager.instance.CheckBoardPositions(tileValue);
    }


    #endregion

    #region deprecated

    ////create a new player tile at the current position for the current player's turn. Pass in the Vector3 position of where to spawn (the position of the tile the player clicked on)
    //private void CreateNewPlayerTile(Vector3 position)
    //{
    //    //the selected tile, matched to the index of the playerIcons array object.
    //    int playerTile = (GameManager.CurrentPlayer == GameManager.Player.P1) ? GameManager.instance.PlayerOneIcon : GameManager.instance.PlayerTwoIcon;
    //    playerTile = Mathf.Clamp(playerTile, 0, 3);

    //    GameObject tilePrefab;
    //    //decide which size icon to use for instantiating
    //    if (BoardState.BoardDimension < 5)
    //        tilePrefab = playerIcons.largeIcons[playerTile];
    //    else
    //        tilePrefab = playerIcons.smallIcons[playerTile];

    //    GameObject newPlayerTile = Instantiate(tilePrefab, position, Quaternion.identity, board.transform);
    //}
    #endregion
}
