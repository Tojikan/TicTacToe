using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Handles everything related to the state of the board: stores the location of pieces, adds new positions, and checks if the game is won
 * Consists of a 2D array. Each emptytile, when created during board generations, gets assigned a value which corresponds to an index in a 2D array.
 * When adding player pieces to the board, it'll add a 1 or 2 depending on p1 or p2. THIS IS NOT ZERO INDEXED
 * Also stores references to the empty tiles. This list is created during Board Generation and is used in the debugger
**/

public static class BoardState
{
    private static int boardDimension;                            //The dimensions of the board grid
    public static int BoardDimension                              //Property accessor. Also clamps the grid dimensions
    {
        get { return boardDimension; }
        set {
            boardDimension = value;
            Mathf.Clamp(boardDimension, 3, 9);
        }
    }
    private static List<Vector2Int> diagonals;                    //List that contains the positions of grid squares that are on a diagonal - used for checking if we should check diagonally
    private static int[,] boardPositions;                         //2D array that represents the grid positions of the gameboard when it comes to checking victory
    public static int[,] BoardPositions                          //property accessor for reading the board array
    {get { return boardPositions; }}
#if UNITY_EDITOR
    private static EmptyTile[,] emptyTileArray;                  //2D array that stores references to all empty tiles. Generated during board generation and only run in Unity Editor
    public static EmptyTile[,] EmptyTileArray
    {
        get { return emptyTileArray; }
    }                //Property accessor for reading the tile array into the debugger
#endif
    private static int boardCount = 0;                            //Count of all the already selected board positions - used to check for draws. Incremented when new position added
    

    #region setup and manipulate the board array that the game actually tracks for checking wins and losses

    //set up a new board 2D array which tracks the position of the player pieces in regards to checking for victory or draw. Called in GameManager
    public static void SetBoardArray()
    {
        //new array
        boardPositions = new int[boardDimension, boardDimension];
        //set a list of possible diagonals
        diagonals = GetDiagonals();
        //boardcount for counting for draw
        boardCount = 0;
    }

    //Adds a new position into the board array and checks if the game is over
    public static void AddPosition(Vector2Int position, int player)
    {
        if (SetPositionInBoard(position, player))
        {
            boardCount++;
            CheckIfGameOver(position, player);
            CheckIfDraw();
        }       
    }


    //adds a new position to the board given a Vector2(row, column) and an int to represent player - 1 for player 1 and 2 for player 2
    //if the move is valid, this will return true and false if not
    public static bool SetPositionInBoard(Vector2Int position, int player)
    {
        if (player != 1 && player != 2)
        {
            Debug.Log("Invalid player value");
            return false;
        }

        if (boardPositions[position.x, position.y] != 0)
        {
            Debug.Log("Player " + boardPositions[position.x, position.y] + " is already occupying this space");
            return false;
        }
        else
        {
            boardPositions[position.x, position.y] = player;
            return true;
        }
    }
    #endregion

#if UNITY_EDITOR
    #region setup and manipulate the list that tracks the empty tiles that are present on the screen. Used only Board State Tests
    /**
     * Because the board array is generated from a Vector 2D assigned separately to each empty tile upon instantiation, we can piggy back on those values to generate
     * another 2D array of empty tiles using those same values as indices. As a result, they should match
     * **/

    //Generates a tile array of size
    public static void GenerateTileArray()
    {
        emptyTileArray = new EmptyTile[boardDimension,boardDimension];
    }
    
    //Adds a new tile to the array by getting indices from its tile value
    public static void AddToTileArray(EmptyTile emptyTile)
    {
        emptyTileArray[emptyTile.TileValue.x, emptyTile.TileValue.y] = emptyTile;
    }

    //Trigger a tile drop
    public static void TriggerTileDrop(int row, int column)
    {
        emptyTileArray[row, column].SpawnTile();
    }



    #endregion
#endif

    #region Check functions
    //to be called after every move to check if the game is won. Returns true if someone has won
    public static bool CheckIfGameOver(Vector2Int position, int player)
    {
        if (CheckColumn(position.y, player))
        {
            return true;
        }

        if (CheckRow(position.x, player))
        {
            return true;
        }

        //check diagonally
        if (diagonals.Contains(position))
        {
            if (CheckFrontDiagonal(player) || CheckBackDiagonal(player))
            {
                return true;
            }
        }
        return false;
    }

    //Checks if we have a draw by matching the count of selected grid cells to the number of total positions
    public static bool CheckIfDraw()
    {
        if (boardCount >= boardPositions.Length)
        {
            return true;
        }
        else
            return false;
    }

    #endregion

    #region Functions for Checking rows, columns and diagonals
    //check along an entire column given parameters of a player to check for and a column position (second indice of the 2D matrix)
    public static bool CheckColumn(int col, int player)
    {
        //iterate through each and match it with the player parameter (1 or 2)
        for (int i = 0; i < boardDimension; i++)
        {
            if (boardPositions[i, col] != player)
            {
                return false;
            }
        }
        return true;
    }


    //check along an entire row given parameters of a player to check for and a row position (first indice of the 2D matrix)
    public static bool CheckRow(int row, int player)
    {
        //iterate through each and match it with the player parameter (1 or 2)
        for (int i = 0; i < boardDimension; i++)
        {
            if (boardPositions[row, i] != player)
            {
                return false;
            }
        }
        return true;
    }

    //check the diagonals, first checking like this: \
    public static bool CheckBackDiagonal(int player)
    {
        //top-left to bottom right, each one increments by 1,1
        for (int i = 0; i < boardDimension; i++)
        {
            if (boardPositions[i, i] != player)
            {
                return false;
            }
        }
        return true;
    }

    //check on a diagonal line like this /
    public static bool CheckFrontDiagonal(int player)
    {
        //from bottom left to top-right, each tile increments by i, d - 1 -i
        for (int i = 0; i < boardDimension; i++)
        {
            if (boardPositions[i, boardDimension - 1 - i] != player)
            {
                return false;
            }
        }
        return true;
    }

    //returns an array of all positions that can result in a diagonal line victory - so we only check diagonally if the position is in this array.
    private static List<Vector2Int> GetDiagonals()
    {
        List<Vector2Int> result = new List<Vector2Int>();
        for (int i = 0; i < boardDimension; i++)
        {
            result.Add(new Vector2Int(i, i));
        }
        for (int i = 0; i < boardDimension; i++)
        {
            result.Add(new Vector2Int(i, boardDimension - 1 - i));
        }
        return result;
    }
    #endregion
}
