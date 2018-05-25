using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles everything related to the state of the board: stores the location of pieces, adds new positions, and checks if the game is won
public static class BoardState
{
    private static int boardDimension;                            //The dimensions of the board grid
    public static int BoardDimension                              //Property accessor
    {
        get { return boardDimension; }
        set { boardDimension = value; }
    }                          
    private static List<Vector2Int> diagonals;                    //List that contains the positions of grid squares that are on a diagonal - used for checking if we should check diagonally
    private static int[,] boardPositions;                         //2D array that represents the grid positions of the gameboard when it comes to checking victory
    private static int boardCount = 0;                            //Count of all the already selected board positions - used to check for draws. Incremented when new position added

    //set up a new board 2D array
    public static void SetBoardArray()
    {
        boardPositions = new int[boardDimension, boardDimension];
        //set a list of possible diagonals
        diagonals = GetDiagonals();
        Debug.Log(boardPositions.Length);
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
    private static bool SetPositionInBoard(Vector2Int position, int player)
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

    //to be called after every move to check if the game is won. 
    private static bool CheckIfGameOver(Vector2Int position, int player)
    {
        if (CheckColumn(position.y, player))
        {
            Debug.Log("Player " + player + " has won!");
            return true;
        }

        if (CheckRow(position.x, player))
        {
            Debug.Log("Player " + player + " has won!");
            return true;
        }


        //check diagonally
        if (diagonals.Contains(position))
        {
            if (CheckFrontDiagonal(player) || CheckBackDiagonal(player))
            {
                Debug.Log("Player " + player + " has won!");
                return true;
            }
        }
        Debug.Log("No Victory Yet");
        return false;
    }

    //Checks if we have a draw by matching the count of selected grid cells to the number of total positions
    private static bool CheckIfDraw()
    {
        if (boardCount >= boardPositions.Length)
        {
            Debug.Log("Draw!");
            return true;
        }
        else
            return false;
    }

    #region Functions for Checking rows, columns and diagonals
    //check along an entire column given parameters of a player to check for and a column position (second indice of the 2D matrix)
    private static bool CheckColumn(int col, int player)
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
    private static bool CheckRow(int row, int player)
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

    //check the diagonal from top left to bottom right such as \
    private static bool CheckFrontDiagonal(int player)
    {
        //on this diagonal, each one increments by 1,1
        for (int i = 0; i < boardDimension; i++)
        {
            if (boardPositions[i, i] != player)
            {
                return false;
            }
        }
        return true;
    }

    //check the diagonal from bottom left to top right such as /
    private static bool CheckBackDiagonal(int player)
    {
        //on this diagonal, each one increments by i, d - 1 -i
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
        foreach (Vector2Int vec in result)
        {
            Debug.Log(vec);
        }
        return result;
    }
    #endregion
}
