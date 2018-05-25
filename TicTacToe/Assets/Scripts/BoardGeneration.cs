using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Generates the TicTacToe board based on a given dimension by laying out empty tiles
public class BoardGeneration : MonoBehaviour
{
    public GameObject largeTile;                    //Drag the large empty tile prefab here for generating a smaller board 3x3 or 4x4
    public GameObject smallTile;                    //Drag the small empty tile prefab here for generating larger boards 5x5 to 9x9
    const float tileSpacing = 0.02f;                //space in between rows and columns
    private Vector2 tileBounds;                     //x and y size for the bounds of empty tile used for positioning new tiles
    private GameObject emptyTile = null;            //the selected tile used for generating the game board. Is set in SetTileSize.
    private BoardState boardState;                  //the component that stores the state of the board and checks victory. Set in Awake



    private void Awake()
    {
        boardState = FindObjectOfType<BoardState>();
    }


    #region public function to be called
    //main public method called by game to generate the board. pass parameter of the board size
    public void GenerateBoard(int dimension)
    {
        ClearBoard();                           //destroy any existing children of this object
        SetTileSize(dimension);                 //select large or small tile 
        GetTileBounds();                        //get tile size
        GenerateTiles(dimension);               //instantiates new tiles as child of this game object
        boardState.SetBoardArray(dimension);    //sets the board array
    }
    #endregion 

    #region private functions that do all the work

    //checks to see if the dimensions are valid and then sets the empty tile according to dimension size.
    private void SetTileSize(int dimension)
    {
        if (dimension < 3 || dimension > 9)
        {
            Debug.LogError("Invalid dimension. Dimensions should be between 3 and 9. Defaulting to 3x3");
            emptyTile = smallTile;
            return;
        }

        if (dimension == 3 || dimension == 4)
        {
            emptyTile = largeTile;
            return;
        }

        if (dimension >= 5 && dimension < 10)
        {
            emptyTile = smallTile;
            return;
        }
    }

    //Generates 
    private void GenerateTiles(int dimension)
    {
        Debug.Log("Generating Tiles of Dimension: " + dimension);
        //get the position where to lay the first empty tile
        Vector3 currentPos = GetTileStartPos(dimension);
        //rows
        for (int i = 0; i < dimension; i++)
        {
            //columns
           for (int j = 0; j < dimension; j++)
            {
                //instantiate new tile as a child of this object and then reset position
                GameObject newTile = Instantiate(emptyTile, currentPos, Quaternion.identity, gameObject.transform);
                newTile.GetComponent<EmptyTile>().TileValue = new Vector2Int(i, j);
                currentPos.x += tileBounds.x + tileSpacing;
            }
           //set x and y for a new row
            currentPos.x = GetTileStartPos(dimension).x;
            currentPos.y -= tileBounds.y + tileSpacing;
        }
    }
    
    //Finds the top right corner position of a new gameboard with given dimension for use in positioning new tiles
    private Vector3 GetTileStartPos(int dimension)
    {
        return new Vector3(
                            -((tileBounds.x / 2) * (dimension-1)),
                            ((tileBounds.y / 2) * (dimension-1)),
                            0);
    }

    //sets the bounds based on the selected tile's sprite bounds
    private void GetTileBounds()
    {
        tileBounds = emptyTile.GetComponent<SpriteRenderer>().bounds.size;
    }

    //clears all of the empty tiles
    private void ClearBoard()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    #endregion
}
