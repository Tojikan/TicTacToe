using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controller class that handles the flow of the game. Calls different functions in the other classes and manages the order of execution
//Also tracks the current player at the time and handles all the transitions/animations between game parts
//set to singleton for the purpose of making this more accessible and testable by the debugger
public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;                            //singleton instance of GameManager
    public GameObject selectSizeWindow;                                   //Drag UI Object that selects the size of the game grid
    public BoardGeneration boardGeneration;                               //Drag component that generates the board here
    public MenuUI UIManager;                                              //Drag the object that controls the UI pop up panel menu for setting game settings
    public GameObject UIButtons;                                          //Drag the object that contains the top left buttons during games
    public PlayerIconSet iconSet;                                         //drag iconset scriptable object here
    public PlayerScoreKeeper p1Score;                                     //Drag p1 score UI element here
    public PlayerScoreKeeper p2Score;                                     //Drag p2 score UI element here
    public float playerInputInterval = 0.2f;                              //Time in between re-enabling mouse input between player turns
    public enum Player                                                    //This enum sets who the current player is
    { P1 = 1, P2 = 2}                                           
    private static Player currentPlayer;                                  //the current player variable
    public static Player CurrentPlayer
    {
        get { return currentPlayer; }
    }                                 //for reading outside the class
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
            playerOneIcon = value;
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
            playerTwoIcon = value;
        }
    }                                           //for setting the player icon image in the score keeper and clamping changes

    private void Awake()
    {
        //set singleton instance
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }


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
        FadePlayerScores();
        UIManager.ShowGameUI();
        EnableControls();
    }

    //checks if the game is over by calling the BoardState. This should be called after every player move.
    public void CheckBoardPositions(Vector2Int tileValue)
    {
        if (BoardState.CheckIfGameOver(tileValue,(int)currentPlayer))
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
        //re-enable input
        Invoke("EnableInput", playerInputInterval);
    }

    //switch to next player
    public void SwitchCurrentPlayer()
    {
        if (currentPlayer == Player.P1)
            currentPlayer = Player.P2;
        else if (currentPlayer == Player.P2)
            currentPlayer = Player.P1;

        FadePlayerScores();
    }

    //Enable inputs again after player turns
    private void EnableInput()
    {
        if (!inputEnabled)
        {
            inputEnabled = true;
        }
    }

    //fade the player sides to show the current player
    private void FadePlayerScores()
    {
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

    #endregion

    #region Different Game Finishes
    //Called when a tie game is detected
    private void GameDraw()
    {
        //stop inputs
        DisableControls();
        //record game results
        GameDataRecorder.instance.RecordGameFinish(3);
        //display message
        UIManager.FinishScreen("Not Everyone Can Be Winners", "Draw!");
        //log the game to console
        GameDataRecorder.instance.ReportGame(GameDataRecorder.instance.MatchList.Count - 1);
        //play draw music
        AudioManager.instance.PlayDraw();
        return;
    }

    //Called when a game over is detected. Switches current player for the next match
    private void GameOver()
    {
        DisableControls();
        GameDataRecorder.instance.RecordGameFinish((int)currentPlayer);
        UIManager.FinishScreen("Winner Winner Chicken Dinner", "Player " + ((int)currentPlayer));
        GameDataRecorder.instance.ReportGame(GameDataRecorder.instance.MatchList.Count - 1);
        AudioManager.instance.PlayVictory();
        SwitchCurrentPlayer();
        return;
    }

    //Ends the current game if we throw an error when trying to record moves into the board array. Used during testing but keeping just in case.
    public void GameError()
    {
        DisableControls();
        GameDataRecorder.instance.RecordGameFinish(4);
        Debug.LogError("ERROR: Unable to write move to board grid. Exiting...");
        GameDataRecorder.instance.ReportGame(GameDataRecorder.instance.MatchList.Count - 1);
        UIManager.BackToSetup();
        AudioManager.instance.PlayDraw();
        return;
    }

    //when click on the surrender button switches to next player and sets a new vicctory
    public void Surrender()
    {
        DisableControls();
        GameDataRecorder.instance.AddPlayerMove(new Vector2Int(-1, -1));
        SwitchCurrentPlayer();
        GameDataRecorder.instance.RecordGameFinish((int)CurrentPlayer + 1);
        //get the surrendered player number
        int surrendered = (CurrentPlayer == Player.P1) ? 2 : 1; 
        UIManager.FinishScreen("Player " + surrendered + " Surrenders!", "Player " + ((int)currentPlayer) + " Wins!");
        GameDataRecorder.instance.ReportGame(GameDataRecorder.instance.MatchList.Count - 1);
        AudioManager.instance.PlayVictory();
    }
    
    //when click on a new game button. Declares a game end goes back to setup
    public void NewGame()
    {
        DisableControls();
        GameDataRecorder.instance.AddPlayerMove(new Vector2Int(-2, -2));
        GameDataRecorder.instance.RecordGameFinish(0);
        GameDataRecorder.instance.ReportGame(GameDataRecorder.instance.MatchList.Count - 1);
        UIManager.BackToSetup();
    }

#if UNITY_EDITOR
    public void DebugWindow(string message)
    {
        DisableControls();
        ClearBoard();
        UIManager.DebugWindowMessage(message);

    }
#endif

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

#if UNITY_EDITOR
    //This method is called in between cycles when testing board state in the debugger. 
    public void BoardStateTestReset()
    {
        ClearBoard();
        //special UI method to clear all screens regardless and set up the button UI
        UIManager.HideButtons();
        //Generate board and array
        boardGeneration.GenerateBoard();
        BoardState.SetBoardArray();
    }
#endif
    #endregion
}
