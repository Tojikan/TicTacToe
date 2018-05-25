using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles the flow of the game and calls different functions as needed
public class GameManager : MonoBehaviour
{
    private static bool currentPlayer;                                      //Tracks whose turn it currently is
    public static bool CurrentPlayer
    {
        get { return currentPlayer; }
        set { currentPlayer = value; }
    }                                     //Property Accessor
    public BoardGeneration boardGeneration;                                //Generate the board

    // Update is called once per frame
    void Update()
    {


    }

    public void InitGame(int dimension)
    {
        
        BoardState.BoardDimension = dimension;
        boardGeneration.GenerateBoard();
        BoardState.SetBoardArray();
        EnableControls();
    }

    public void EnableControls()
    {
        PlayerController.inputEnabled = true;
    }

    public void DisableControls()
    {
        PlayerController.inputEnabled = false;
    }

}
