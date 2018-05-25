using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controller class that handles the flow of the game. Calls different functions in the other classes and manages the order of execution
//Also tracks the current player at the time and handles all the transitions/animations between game parts
public class GameManager : MonoBehaviour
{
    public GameObject selectSizeWindow;                                   //Drag UI Object that selects the size of the game grid
    public BoardGeneration boardGeneration;                               //Drag component that generates the board here
    public float playerInputInterval = 0.2f;                              //Time in between re-enabling mouse input between player turns
    public enum Player {P1, P2}                                           //This enum sets who the current player is
    private static Player currentPlayer;                                  //the current player variable
    public static Player CurrentPlayer
    {
        get { return currentPlayer; }
    }                                 //for read only from outside of class
    private static bool inputEnabled = false;                             //static bool to set if the player input is enabled
    public static bool InputEnabled
    { get { return inputEnabled; } }                                    //for read only from outside of class
    [SerializeField]
    private int playerOneIcon;                                            //Store the selected icon for player one. Matched to the index of the icon in the player icon set
    public int PlayerOneIcon
    {
        get { return playerOneIcon; }
    }                                           //can read and set
    [SerializeField]
    private int playerTwoIcon;                                            //Ditto for player two
    public int PlayerTwoIcon
    {
        get
        {
            return playerTwoIcon;
        }

        set
        {
            playerTwoIcon = value;
        }
    }                                           //can read and set

    private void Start()
    {
        //initialize to player one at the start of the game
        currentPlayer = Player.P1;
        //default values for game
        BoardState.BoardDimension = 3;
        playerOneIcon = 0;
        playerTwoIcon = 1;
    }

    //Starts a new match
    public void StartNewGame(int dimension)
    {
        //prevent player input between games 
        DisableControls();
        //Hide the select window
        selectSizeWindow.SetActive(false);
        //Generate the board and the board array
        BoardState.BoardDimension = dimension;
        boardGeneration.GenerateBoard();
        BoardState.SetBoardArray();
        //Create a new match in the game data
        GameDataRecorder.instance.AddNewMatch(playerOneIcon, playerTwoIcon, BoardState.BoardDimension, (int)currentPlayer);
        EnableControls();
    }

    //allow mouse clicks on empty tiles to register
    public void EnableControls()
    {
        inputEnabled = true;
    }

    //disallow mouse clicks on empty tiles to register
    public void DisableControls()
    {
        inputEnabled = false;
    }

    //checks if the game is over by calling the BoardState. This should be called after every player move.
    public void CheckBoardPositions(Vector2Int tileValue)
    {
        if (BoardState.CheckIfGameOver(tileValue,(int)currentPlayer + 1))
        {
            GameOver();
            return;
        }

        if (BoardState.CheckIfDraw())
        {
            GameDraw();
            return;
        }

        EndTurn();
    }

    //switch player and triggers the animation
    private void EndTurn()
    {
        SwitchCurrentPlayer();
        //the turn animation also has an animation event that re-enables input as it was disabled in PlayerController
        SwitchTurnAnimation();
        //Check after 2 seconds to see if input is still disabled in case animation event didn't trigger.
        Invoke("CheckInputEnabled", playerInputInterval);
    }

    //switch to next player
    private void SwitchCurrentPlayer()
    {
        if (currentPlayer == Player.P1)
            currentPlayer = Player.P2;
        else
            currentPlayer = Player.P1;
    }

    //Start the turn switch animation
    private void SwitchTurnAnimation()
    {
        return;
    }

    //Just check to make sure movement is enabled in case something goes wrong with the animation that re-enables movement.
    private void CheckInputEnabled()
    {
        if (!inputEnabled)
        {
            inputEnabled = true;
        }
    }

    //Called when a tie game is detected
    private void GameDraw()
    {
       GameDataRecorder.instance.RecordGameFinish(3);
       GameDataRecorder.instance.RecreateGame(GameDataRecorder.instance.MatchList.Count - 1);
        return;
    }

    //Called when a game over is detected
    private void GameOver()
    {
        GameDataRecorder.instance.RecordGameFinish((int)currentPlayer);
        for (int i = 0; i < GameDataRecorder.instance.MatchList.Count; i++)
        { GameDataRecorder.instance.RecreateGame(i); }
        return;
    }

    //Ends the current game if we throw an error when trying to record moves into the board array
    public void GameError()
    {
        GameDataRecorder.instance.RecordGameFinish(4);
        return;
    }
}
