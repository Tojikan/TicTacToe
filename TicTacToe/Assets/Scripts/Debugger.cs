
#if UNITY_EDITOR
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
    public enum DiagSelect
    {
        Front, Back
    }                                        //base for selecting which diagonal to test
    private DiagSelect SelectDiag;                                   //select which diagonal 
    private int matchToLoad = 0;                                     //index of match to console log the match data
    private int dimensionToSelect;                                   //used for the int slider to select which row/column to test
    private float betweenMoves = 0.25f;                              //sets the time between moves in the tester
    private float betweenReset = 1f;                                 //sets the time between resets in the tester for nested routines
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
        GUILayout.Space(30);

        #region display match data
        GUILayout.FlexibleSpace();
        GUILayout.Label("Print Match Data", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Print specified match data (zero index) or all match data into the console. ", MessageType.Info);
        EditorGUILayout.Space();
        matchToLoad = EditorGUILayout.IntField("Enter a match number:", matchToLoad);
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

        //vertical space
        GUILayout.Space(15);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.FlexibleSpace();
        /**end divider line **/

        #region New Board Generation
        /**Section for generating new board **/
        EditorGUILayout.Space();
        GUILayout.Label("Generate a New Board", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Generate a new board. Functions similarly to starting a new game. " +
            "Enter play mode, select settings, and press the button to generate a new board. Can generate at any time during the game but will end any match being played.", MessageType.Info);
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

        //vertical space
        GUILayout.Space(15);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.FlexibleSpace();
        /**end divider line **/


        #region Board State testing
        /**Section for testing board states **/
        //vertical space
        GUILayout.Space(15);
        GUILayout.Label("Board State Tester", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Test different board states. Select which player should win. Use the slider to select a specific row or column to test. Use the drop down to select which diagonal to check for. You can also set the speed between test moves" +
            " Board must be generated before using. Will end any match currently being played or exit out of menu", MessageType.Info);

        //Test Settings
        playerToWin = (Player)EditorGUILayout.EnumPopup("Player to Win: ", playerToWin);
        dimensionToSelect = EditorGUILayout.IntSlider("Row/Column to Test: ", dimensionToSelect, 0, BoardState.BoardDimension - 1);
        SelectDiag = (DiagSelect)EditorGUILayout.EnumPopup("Diagonal: ", SelectDiag);
        betweenMoves = EditorGUILayout.FloatField("Time Between Moves: ", betweenMoves);
        
        EditorGUILayout.Space();
        //row test
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Test Row", GUILayout.MaxWidth(150), GUILayout.MinHeight(40)))
        {
            TestRows();
        }
        //column test
        if (GUILayout.Button("Test Column", GUILayout.MaxWidth(150), GUILayout.MinHeight(40)))
        {
            TestColumns();
        }
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        //diagonal test
        if (GUILayout.Button("Test Diagonal", GUILayout.MaxWidth(150), GUILayout.MinHeight(40)))
        {
            TestDiagonal();
        }
        //draw test
        if (GUILayout.Button("Test Draw", GUILayout.MaxWidth(150), GUILayout.MinHeight(40)))
        {
            TestDraw();
        }
        GUILayout.EndHorizontal();
        EditorGUILayout.Space(); EditorGUILayout.Space();

        EditorGUILayout.HelpBox("Pressing any UI buttons, generating a map from the debug window , or starting a new test will immediately end any test. When testing all, just simply let it run through. Set the time between test either here or in the BoardStateTester inspector to set the speed between tests", MessageType.Warning);
        betweenReset = EditorGUILayout.FloatField("Time between tests: ", betweenReset);

        GUILayout.BeginHorizontal();
        //test all rows
        if (GUILayout.Button("Test All Rows", GUILayout.MaxWidth(150), GUILayout.MinHeight(40)))
        {
            TestAllRows();
        }
        //test all columns
        if (GUILayout.Button("Test All Columns", GUILayout.MaxWidth(150), GUILayout.MinHeight(40)))
        {
            TestAllCols();
        }

        //all diag test
        if (GUILayout.Button("Test ALL Diags", GUILayout.MaxWidth(150), GUILayout.MinHeight(40)))
        {
            TestAllDiags();
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("TEST ALL", GUILayout.MinHeight(40)))
        {
            TestAll();
        }


        //vertical space
        GUILayout.Space(30);


        EditorGUILayout.EndScrollView();
        #endregion
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
            BoardStateTester.instance.StartColumnTest(dimensionToSelect);
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
            BoardStateTester.instance.StartRowTest(dimensionToSelect);
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
            //start the test.
            BoardStateTester.instance.StartDrawTest();
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
            BoardStateTester.instance.StartDiagonalTest((int)SelectDiag);
        }
        else
        {
            Debug.LogError("Either the application is not playing or it's not being played in the Editor.");
        }
    }

    private void TestAllRows()
    {
        //see that we're running the application in editor first so we can't run in edit mode
        if (Application.isPlaying && Application.isEditor)
        {
            InitializeTest();
            //start the test.
            BoardStateTester.instance.StartAllRowTests();
        }
        else
        {
            Debug.LogError("Either the application is not playing or it's not being played in the Editor.");
        }
    }

    private void TestAllCols()
    {
        //see that we're running the application in editor first so we can't run in edit mode
        if (Application.isPlaying && Application.isEditor)
        {
            InitializeTest();
            //start the test.
            BoardStateTester.instance.StartAllColTests();
        }
        else
        {
            Debug.LogError("Either the application is not playing or it's not being played in the Editor.");
        }
    }

    private void TestAllDiags()
    {
        //see that we're running the application in editor first so we can't run in edit mode
        if (Application.isPlaying && Application.isEditor)
        {
            InitializeTest();
            //start the test.
            BoardStateTester.instance.StartAllDiagTests();
        }
        else
        {
            Debug.LogError("Either the application is not playing or it's not being played in the Editor.");
        }
    }


    private void TestAll()
    {
        //see that we're running the application in editor first so we can't run in edit mode
        if (Application.isPlaying && Application.isEditor)
        {
            InitializeTest();
            //start the test.
            BoardStateTester.instance.StartAllTests();
        }
        else
        {
            Debug.LogError("Either the application is not playing or it's not being played in the Editor.");
        }
    }


    //Repeated bit of code that occurs before every test.
    private void InitializeTest()
    {
        //change player if necessary
        CheckPlayer((int)playerToWin);

        //stop any running checks
        BoardStateTester.instance.StopAllCoroutines();
        GameManager.instance.StartNewGame();
        GameManager.instance.DisableControls();
        BoardStateTester.instance.timeBetweenMoves = betweenMoves;
        BoardStateTester.instance.timeBetweenResets = betweenReset;
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
        //Switch player
        CheckPlayer((int)playerToStart);
        GameManager.instance.StartNewGame();
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
#endif
