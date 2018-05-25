using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles everything related to the state of the board: stores the location of pieces, adds new positions, and checks if the game is won
public class BoardState : MonoBehaviour
{
    private int[,] boardPositions;
    private int boardDimension;
    private List<Vector2Int> diagonals;

    //set up a new board 2D array
    public void SetBoardArray(int dimension)
    {
        boardPositions = new int[dimension, dimension];
        boardDimension = dimension;
        //set a list of possible diagonals
        diagonals = GetDiagonals(dimension);
        Debug.Log(boardPositions.Length);
    }

    public void AddPosition(Vector2Int position, int player)
    {
        if (SetPositionInBoard(position, player))
        {
            CheckIfGameOver(position, player);
        }       
    }

    //adds a new position to the board given a Vector2(row, column) and an int to represent player - 1 for player 1 and 2 for player 2
    //if the move is valid, this will return true and false if not
    private bool SetPositionInBoard(Vector2Int position, int player)
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

    //to be called after every move to check if the game is won
    private bool CheckIfGameOver(Vector2Int position, int player)
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

    #region Functions for Checking rows, columns and diagonals
    //check along an entire column given parameters of a player to check for and a column position (second indice of the 2D matrix)
    private bool CheckColumn(int col, int player)
    {
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
    private bool CheckRow(int row, int player)
    {
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
    private bool CheckFrontDiagonal(int player)
    {
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
    private bool CheckBackDiagonal(int player)
    {
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
    private List<Vector2Int> GetDiagonals(int dimension)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        for (int i = 0; i < dimension; i++)
        {
            result.Add(new Vector2Int(i, i));
        }
        for (int i = 0; i < dimension; i++)
        {
            result.Add(new Vector2Int(i, dimension - 1 - i));
        }
        foreach (Vector2Int vec in result)
        {
            Debug.Log(vec);
        }
        return result;
    }
    #endregion
}
