using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class Debugger : EditorWindow
{
    private enum Player
    {
        Player1 = 1,
        Player2 = 2
    }
    private Player playerToWin;
    private Player playerToStart;
    private enum GridSelect
    {
        Three = 3, Four = 4, Five = 5, Six = 6, Seven = 7, Eight = 8
    }
    private GridSelect newGridSize;
    private enum IconSelect
    {
        CHIP, CLUB, DIAMOND, HEART
    }
    private IconSelect PlayerOne;
    private IconSelect PlayerTwo;
    private float timeBetweenMoves = 1f;
    private string debugLog;
    private string consoleReport;
    private string windowReport;

    #region GUI Region

    [MenuItem("Window/Debug Window")]
    public static void ShowWindow()
    {
        GetWindow<Debugger>("Debug Window");
    }

    private void OnGUI()
    {
        /**Section for testing board states **/
        GUILayout.FlexibleSpace();
        GUILayout.Label("Test Board States", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        playerToWin = (Player)EditorGUILayout.EnumPopup("Player to Win: ", playerToWin);
        timeBetweenMoves = EditorGUILayout.FloatField("Time Between Moves: ", timeBetweenMoves);
        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Test Rows", GUILayout.MaxWidth(150), GUILayout.MinHeight(40)))
        {
            TestRows();
        }
        if (GUILayout.Button("Test Columns", GUILayout.MaxWidth(150), GUILayout.MinHeight(40)))
        {
            TestColumns();
        }
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Test Diagonals", GUILayout.MaxWidth(150), GUILayout.MinHeight(40)))
        {
            TestDiagonal();
        }
        if (GUILayout.Button("Test Draw", GUILayout.MaxWidth(150), GUILayout.MinHeight(40)))
        {
            TestDraw();
        }
        GUILayout.EndHorizontal();

        /**divider line **/
        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.FlexibleSpace();

        /**Section for generating new board **/
        EditorGUILayout.Space();
        GUILayout.Label("Generate a New Board", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("This box will let you generate a new board anytime during the game without having to go through the menu. " +
            "This functions very similarly to starting a new game. The game must be in play mode in order for this to run. Set your settings on the menu and click the button to generate new board.", MessageType.Info);
        EditorGUILayout.Space();
        newGridSize = (GridSelect)EditorGUILayout.EnumPopup("New Board Dimensions: ", newGridSize);
        PlayerOne = (IconSelect)EditorGUILayout.EnumPopup("Player 1 Icon: ", PlayerOne);
        PlayerTwo = (IconSelect)EditorGUILayout.EnumPopup("Player 2 Icon: ", PlayerTwo);
        playerToStart = (Player)EditorGUILayout.EnumPopup("Player to Start: ", playerToStart);


        if (GUILayout.Button("Generate New Board"))
        {
            if (GameManager.instance == null)
            {
                Debug.LogError("Error: Game Manager not detected. Perhaps you need to start a game first?");
                return;
            }

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
    }



    #endregion

    #region Testing board state function calls

    private void TestColumns()
    {
    }

    private void TestRows()
    {
        if (Application.isPlaying && Application.isEditor)
        {
            if (GameDataRecorder.instance.MatchList.Count > 0 )
            {
                GameDataRecorder.instance.AddPlayerMove(new Vector2Int(-2, -2));
                GameDataRecorder.instance.RecordGameFinish(0);
            }
            CheckPlayer((int)playerToWin);
            BoardStateTester.instance.StopAllCoroutines();
            GameManager.instance.UIManager.DebugBoardStateTesting();
            CreateNewBoard();
            BoardStateTester.instance.StartRowTest(timeBetweenMoves);
        }
    }

    private void TestDraw()
    {
        throw new NotImplementedException();
    }

    private void TestDiagonal()
    {
        throw new NotImplementedException();
    }
    #endregion

    #region board generation


    //this is called in the second part of the debugger window which lets you generate a new board based on the selection set in the debugger window. It should generally follow the same steps as generating a board
    //during normal gameplay. Since it reuses the same code in normal game generation, the debugger board generator replicates 
    private void BoardGenerator()
    {
        //basic set up
        GameManager.instance.DisableControls();
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
        //add new match to recorder
        GameDataRecorder.instance.AddNewMatch(GameManager.instance.PlayerOneIcon, GameManager.instance.PlayerTwoIcon, BoardState.BoardDimension, (int)GameManager.CurrentPlayer);
        //start a new game
        GameManager.instance.EnableControls();
    }

    //This method is called for testing board state and for the Debugger board generator. This will generate the visuals and start a new array but without the added features that BoardGenerator adds.
    private void CreateNewBoard()
    {
        GameManager.instance.ClearBoard();
        //special UI method to clear all screens regardless and set up the button UI
        GameManager.instance.UIManager.SetUpDebugBoardUI();
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
