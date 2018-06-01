using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//Debugger Window tool to allow for quick testing and debugging
//Works by calling various functions in GameManager and BoardStateTester
//Can generate new boards, run a board state check, and log match reports from Game Data Recorder
//Create a new window via Window/DebugWindow
public class Debugger : EditorWindow
{
    private enum Player
    {
        Player1 = 1,
        Player2 = 2
    }                                           //base enum for selecting players
    private Player playerToWin;                                      //select the player you want to check wins for on the board state checker
    private Player playerToStart;                                    //select the player you want to start when generating a new board
    private enum GridSelect
    {
        Three = 3, Four = 4, Five = 5, Six = 6, Seven = 7, Eight = 8
    }                                       //base enum for selecting a new grid
    private GridSelect newGridSize;                                  //select the dimensions for a newly generated grid
    private enum IconSelect
    {
        CHIP, CLUB, DIAMOND, HEART
    }                                       //base enum for selecting an icon
    private IconSelect PlayerOne;                                    //select player one icon for new board generation
    private IconSelect PlayerTwo;                                    //select player two icon for new board generation
    private float timeBetweenMoves = 1f;                             //gives the time in seconds between placing new pieces during a board state check
    private int matchToLoad = 0;                                     //index of match to console log the match data
    private bool failTest = false;                                   //set it so you can intentionally fail board state test
    Vector2 scrollPos;                                               //scroll bar position

    //create menu item for window
    [MenuItem("Window/Debug Window")]
    public static void ShowWindow()
    {
        GetWindow<Debugger>("Debug Window");
    }

    private void OnGUI()
    {


        //scroll pos
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);

        //vertical space
        GUILayout.Space(20);

        #region display match data
        GUILayout.FlexibleSpace();
        GUILayout.Label("Print Match Data", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Print specified match data (zero index) or all match data into the console. ", MessageType.Info);
        EditorGUILayout.Space();
        matchToLoad = EditorGUILayout.IntField("Enter a match number to Print.", matchToLoad);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Print Single Match Data", GUILayout.MaxWidth(150), GUILayout.MinHeight(40)))
        {
            PrintMatchData(matchToLoad);
        }
        //column test
        if (GUILayout.Button("Print All Matches", GUILayout.MaxWidth(150), GUILayout.MinHeight(40)))
        {
            PrintAllMatches();
        }
        GUILayout.EndHorizontal();
        #endregion

        /**divider line for spacing**/
        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.FlexibleSpace();
        /**end divider line **/

        #region New Board Generation
        /**Section for generating new board **/
        EditorGUILayout.Space();
        GUILayout.Label("Generate a New Board", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Generate a new board. Functions similarly to starting a new game. " +
            "Enter play mode, select settings, and press the button to generate a new board.", MessageType.Info);
        EditorGUILayout.Space();
        //Enter data
        newGridSize = (GridSelect)EditorGUILayout.EnumPopup("New Board Dimensions: ", newGridSize);
        PlayerOne = (IconSelect)EditorGUILayout.EnumPopup("Player 1 Icon: ", PlayerOne);
        PlayerTwo = (IconSelect)EditorGUILayout.EnumPopup("Player 2 Icon: ", PlayerTwo);
        playerToStart = (Player)EditorGUILayout.EnumPopup("Player to Start: ", playerToStart);
        //Generate Button
        if (GUILayout.Button("Generate New Board"))
        {
            //check if game manager exists
            if (GameManager.instance == null)
            {
                Debug.LogError("Error: Game Manager not detected. Perhaps you need to start a game first?");
                return;
            }
            //check if icons are different
            if (PlayerOne == PlayerTwo)
            {
                Debug.LogError("Error: Cannot set both player icons to the same icon");
                return;
            }
            else
            {
                BoardGenerator();
            }
        }
        GUILayout.FlexibleSpace();
        /**end section**/
        #endregion


        /**divider line for spacing**/
        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.FlexibleSpace();
        /**end divider line **/


        #region Board State testing
        /**Section for testing board states **/
        GUILayout.FlexibleSpace();
        GUILayout.Label("Board State Tester", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Test if Board States are properly detected. Select a player to check victory for. Then select time in seconds between simulated moves. " +
            "Board must be generated or it will use default values.", MessageType.Info);

        //change player to win 
        playerToWin = (Player)EditorGUILayout.EnumPopup("Player to Win: ", playerToWin);
        //set time between moves
        timeBetweenMoves = EditorGUILayout.FloatField("Time Between Moves: ", timeBetweenMoves);
        failTest = EditorGUILayout.Toggle("Fail Draw Test: ", failTest);
        EditorGUILayout.Space();
        //row test
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Test Rows", GUILayout.MaxWidth(150), GUILayout.MinHeight(40)))
        {
            TestRows();
        }
        //column test
        if (GUILayout.Button("Test Columns", GUILayout.MaxWidth(150), GUILayout.MinHeight(40)))
        {
            TestColumns();
        }
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        //diagonal test
        if (GUILayout.Button("Test Diagonals", GUILayout.MaxWidth(150), GUILayout.MinHeight(40)))
        {
            TestDiagonal();
        }
        //draw test
        if (GUILayout.Button("Test Draw", GUILayout.MaxWidth(150), GUILayout.MinHeight(40)))
        {
            TestDraw();
        }
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        #endregion

        //vertical space
        GUILayout.Space(20);


        EditorGUILayout.EndScrollView();
    }

    #region Match printing methods
    //Print ALL matches in the data
    private void PrintAllMatches()
    {
        if (GameDataRecorder.instance == null)
        {
            Debug.LogError("No Game Data Recorder found. Did you forget to start a game?");
            return;
        }

        //check if we actually have any
        if (GameDataRecorder.instance.MatchList.Count == 0)
        {
            Debug.LogError("Error: No MatchData found");
            return;
        }
        Debug.Log("Printing All Matches");
        //for loop
        for (int i = 0; i < GameDataRecorder.instance.MatchList.Count; i++)
        {
            GameDataRecorder.instance.ReportGame(i);
        }
    }

    //print a specified match
    private void PrintMatchData(int matchToLoad)
    {
        if (GameDataRecorder.instance == null)
        {
            Debug.LogError("No Game Data Recorder found. Did you forget to start a game?");
            return;
        }

        //check if the match exists or if we have matches
        if (matchToLoad >= GameDataRecorder.instance.MatchList.Count || GameDataRecorder.instance.MatchList.Count == 0)
        {
            Debug.LogError("Invalid or nonexistent Match");
            return;
        }
        else
        {
            Debug.Log("Printing Match " + matchToLoad);
            GameDataRecorder.instance.ReportGame(matchToLoad);
        }
    }



    #endregion

    #region Board State Testing methods

    private void TestColumns()
    {
        //see that we're running the application in editor first so we can't run in edit mode
        if (Application.isPlaying && Application.isEditor)
        {
            InitializeTest();
            //start the test.
            BoardStateTester.instance.StartColumnTest(timeBetweenMoves);
        }
        else
        {
            Debug.LogError("Either the application is not playing or it's not being played in the Editor.");
        }
    }

    //tests all the rows. Handles all the preliminary things 
    private void TestRows()
    {
        //see that we're running the application in editor first so we can't run in edit mode
        if (Application.isPlaying && Application.isEditor)
        {
            InitializeTest();
            //start the test.
            BoardStateTester.instance.StartRowTest(timeBetweenMoves);
        }
        else
        {
            Debug.LogError("Either the application is not playing or it's not being played in the Editor.");
        }
    }

    private void TestDraw()
    {
        //see that we're running the application in editor first so we can't run in edit mode
        if (Application.isPlaying && Application.isEditor)
        {
            InitializeTest();
            //Set the fail test bool
            BoardStateTester.instance.failTest = failTest;
            //start the test.
            BoardStateTester.instance.StartDrawTest(timeBetweenMoves);
        }
        else
        {
            Debug.LogError("Either the application is not playing or it's not being played in the Editor.");
        }
    }

    private void TestDiagonal()
    {
        //see that we're running the application in editor first so we can't run in edit mode
        if (Application.isPlaying && Application.isEditor)
        {
            InitializeTest();
            //start the test.
            BoardStateTester.instance.StartDiagonalTest(timeBetweenMoves);
        }
        else
        {
            Debug.LogError("Either the application is not playing or it's not being played in the Editor.");
        }
    }

    //Repeated bit of code that occurs before every test.
    private void InitializeTest()
    {
        //If we have match data in the list, then end it
        if (GameDataRecorder.instance.MatchList.Count > 0)
        {
            GameDataRecorder.instance.AddPlayerMove(new Vector2Int(-2, -2));
            GameDataRecorder.instance.RecordGameFinish(0);
        }
        //change player if necessary
        CheckPlayer((int)playerToWin);
        //stop any running checks
        BoardStateTester.instance.StopAllCoroutines();
        //Hide all UI buttons so no clicks in the middle of a test
        GameManager.instance.UIManager.HideButtons();
        //Initial new board
        CreateNewBoard();
    }
    #endregion

    #region board generation methods


    //this is called in the second part of the debugger window which lets you generate a new board based on the selection set in the debugger window. It should generally follow the same steps as generating a board
    //during normal gameplay. Since it reuses the same code in normal game generation, the debugger board generator replicates 
    private void BoardGenerator()
    {
        //basic set up
        GameManager.instance.DisableControls();
        //stop any running tests just in case
        BoardStateTester.instance.StopAllCoroutines();
        //add the enums selected in the debugger window
        GameManager.instance.PlayerOneIcon = (int)PlayerOne;
        GameManager.instance.PlayerTwoIcon = (int)PlayerTwo;
        BoardState.BoardDimension = (int)newGridSize;
        CreateNewBoard();
        //Record end of current match if a match is going on
        if (GameDataRecorder.instance.MatchList.Count > 0)
        {
            GameDataRecorder.instance.AddPlayerMove(new Vector2Int(-2, -2));
            GameDataRecorder.instance.RecordGameFinish(0);
            GameDataRecorder.instance.ReportGame(GameDataRecorder.instance.MatchList.Count - 1);
        }
        //Switch player
        CheckPlayer((int)playerToStart);
        GameManager.instance.UIManager.ShowGameUI();
        //add new match to recorder
        GameDataRecorder.instance.AddNewMatch(GameManager.instance.PlayerOneIcon, GameManager.instance.PlayerTwoIcon, BoardState.BoardDimension, (int)GameManager.CurrentPlayer);
        //start a new game
        GameManager.instance.EnableControls();
    }

    //This method is called for testing board state and for the Debugger board generator. This will generate the visuals and start a new array but without the added features that BoardGenerator adds.
    private void CreateNewBoard()
    {
        GameManager.instance.ClearBoard();
        //Generate board and array
        GameManager.instance.boardGeneration.GenerateBoard();
        BoardState.SetBoardArray();
    }
    #endregion

    //switches player if the int parameter doesn't match the int parameter of Game Manager. Keeps CurrentPlayer a read-only and also gets used in Board generation and State Testing in the debugger
    private void CheckPlayer(int player)
    {
        if ((int)GameManager.CurrentPlayer != player)
        {
            GameManager.instance.SwitchCurrentPlayer();
        }
    }
}
