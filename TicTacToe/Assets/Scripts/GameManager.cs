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
    public MenuUI UIManager;                                              //Drag the object that controls the UI pop up panel menu for setting game settings
    public PlayerIconSet iconSet;                                         //drag iconset scriptable object here
    public PlayerScoreKeeper p1Score;                                     //Drag p1 score UI element here
    public PlayerScoreKeeper p2Score;                                     //Drag p2 score UI element here
    public float playerInputInterval = 0.2f;                              //Time in between re-enabling mouse input between player turns
    public enum Player {P1, P2}                                           //This enum sets who the current player is
    private static Player currentPlayer;                                  //the current player variable
    public static Player CurrentPlayer
    {
        get { return currentPlayer; }
        set
        {
            currentPlayer = value;
            if (currentPlayer == Player.P1)
            {
                p1Score.FadeScore(true);
                p2Score.FadeScore(false);
            }
            else
            {
                p2Score.FadeScore(true);
                p1Score.FadeScore(false);
            }
        }
    }                                 
    private static bool inputEnabled = false;                             //static bool to set if the player input is enabled
    public static bool InputEnabled
    { get { return inputEnabled; } }                                    //for read only from outside of class
    [SerializeField]
    private int playerOneIcon;                                            //Store the selected icon for player one. Matched to the index of the icon in the player icon set
    public int PlayerOneIcon
    {
        get { return playerOneIcon; }
        set
        {
            Mathf.Clamp(value, 0, iconSet.largeIcons.Length);
            playerOneIcon = value;
            p1Score.SetImage(playerOneIcon);
        }
    }                                           //for setting the player icon image in the score keeper and clamping changes
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
            Mathf.Clamp(value, 0, iconSet.largeIcons.Length);
            playerTwoIcon = value;
            p1Score.SetImage(playerTwoIcon);
        }
    }                                           //for setting the player icon image in the score keeper and clamping changes

    private void Start()
    {
        //initialize to player one at the start of the game
        currentPlayer = Player.P1;
        //default values for game
        BoardState.BoardDimension = 3;
        PlayerOneIcon = 0;
        PlayerTwoIcon = 1;
    }

    #region GameBoard startup and check

    //Starts a new match. Called by buttons in the UIPopUpPanel
    public void StartNewGame()
    {
        //prevent player input between games 
        DisableControls();
        //Hide the select window
        selectSizeWindow.SetActive(false);
        //Generate the board and the board array
        boardGeneration.GenerateBoard();
        BoardState.SetBoardArray();
        //Create a new match in the game data
        GameDataRecorder.instance.AddNewMatch(PlayerOneIcon, PlayerTwoIcon, BoardState.BoardDimension, (int)currentPlayer);
        EnableControls();
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

    #endregion

    #region Player Turns
    //switch player and triggers the animation
    private void EndTurn()
    {
        SwitchCurrentPlayer();
        //the turn animation also has an animation event that re-enables input as it was disabled in PlayerController
        SwitchTurnAnimation();
        //re-enable input
        Invoke("EnableInput", playerInputInterval);
    }

    //switch to next player
    private void SwitchCurrentPlayer()
    {
        if (currentPlayer == Player.P1)
            currentPlayer = Player.P2;
        else if (currentPlayer == Player.P2)
            currentPlayer = Player.P1;
    }

    //Start the turn switch animation
    private void SwitchTurnAnimation()
    {
        return;
    }

    //Enable inputs again after player turns
    private void EnableInput()
    {
        if (!inputEnabled)
        {
            inputEnabled = true;
        }
    }

    private void UISetPlayerAlpha(bool player)
    {

    }

    #endregion

    #region Different Game Finishes
    //Called when a tie game is detected
    private void GameDraw()
    {
        DisableControls();
        GameDataRecorder.instance.RecordGameFinish(3);
        UIManager.FinishScreen("Not Everyone Can Be Winners", "Draw!");
        GameDataRecorder.instance.ReportGame(GameDataRecorder.instance.MatchList.Count - 1);
        return;
    }

    //Called when a game over is detected. Switches current player for the next match
    private void GameOver()
    {
        DisableControls();
        GameDataRecorder.instance.RecordGameFinish((int)currentPlayer);
        UIManager.FinishScreen("Winner Winner Chicken Dinner", "Player " + ((int)currentPlayer + 1));
        GameDataRecorder.instance.ReportGame(GameDataRecorder.instance.MatchList.Count - 1);
        SwitchCurrentPlayer();
        return;
    }

    //Ends the current game if we throw an error when trying to record moves into the board array
    public void GameError()
    {
        DisableControls();
        GameDataRecorder.instance.RecordGameFinish(4);
        Debug.LogError("ERROR: Unable to write move to board grid. Exiting...");
        GameDataRecorder.instance.ReportGame(GameDataRecorder.instance.MatchList.Count - 1);
        UIManager.BackToSetup();
        return;
    }

    //when click on the surrender button switches to next player and sets a new vicctory
    public void Surrender()
    {
        DisableControls();
        GameDataRecorder.instance.AddPlayerMove(new Vector2Int(-1, -1));
        SwitchCurrentPlayer();
        GameDataRecorder.instance.RecordGameFinish((int)currentPlayer);
        UIManager.FinishScreen("Winner Winner Chicken Dinner", "Player " + ((int)currentPlayer + 1));
        GameDataRecorder.instance.ReportGame(GameDataRecorder.instance.MatchList.Count - 1);
    }
    
    //when click on a new game button - goes back to setup
    public void NewGame()
    {
        DisableControls();
        GameDataRecorder.instance.AddPlayerMove(new Vector2Int(-2, -2));
        GameDataRecorder.instance.RecordGameFinish(3);
        UIManager.BackToSetup();
    }

    #endregion

    #region General functionalities for setting the game
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
    //Clears the game board 
    public void ClearBoard()
    {
        foreach (Transform child in boardGeneration.gameObject.transform)
        {
            Destroy(child.gameObject);
        }
    }
    #endregion
}
