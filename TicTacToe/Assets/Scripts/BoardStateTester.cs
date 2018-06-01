using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class that handles board state testing. contains all the coroutines
//Should only compile in editor
//Kept as a separate monobehaviour due to usage of coroutines to time board testing
//Called and set from the Debugger Window
public class BoardStateTester : MonoBehaviour
{
#if UNITY_EDITOR
    public static BoardStateTester instance = null;            //singleton for simple reference getting as this will only be called by the Debugger and reduce problems with getting references in edit vs play mode
    public bool failTest;                                      //bool to intentionally fail tests
    //singleton
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    #region CoRoutine Call Methods
    //Ran into some issues trying to start coroutines from an EditorWindow object, so just have the buttons route to these methods that'll call the coroutines.

    public void StartRowTest(float wait)
    {
        StartCoroutine("RowTester", wait);
    }

    public void StartColumnTest(float wait)
    {
        StartCoroutine("ColumnTester", wait);
    }

    public void StartDiagonalTest(float wait)
    {
        StartCoroutine("DiagonalTester", wait);
    }

    public void StartDrawTest(float wait)
    {
        StartCoroutine("DrawTester", wait);
    }



    #endregion


    /** Board State Checker oroutines
     * Functions by playing through a row/column/diagonal. 
     * Use a coroutine in order to simulate faster playing. Pass in a parameter of time to wait in between moves
     * The starting player should always win. The second turn player will simply place a piece underneath the starting player and should thus never beat the starting player.
     * If a game doesn't register a victory, it'll spit out a debug.log. The test will spit out a log at the end. 
     * To make sure this is setup properly, the board MUST be cleared and the starting player set in the debugger method that calls this coroutine. That should all be set before starting the coroutine.
     * **/



    #region Coroutines
    //Checks all rows.
    IEnumerator RowTester(float seconds)
    {
        //counts how many successful row checks we have
        int count = 0;
        //store the player we're measuring. The current player should be set to the one we want to check when pressing the button in Debugger.
        int playerCheck = (int)GameManager.CurrentPlayer;
        //Stop controls
        GameManager.instance.DisableControls();
        
        //A string that logs the results of all row tests.
        string consoleLog = "==========ROW TEST RESULT===========" + "\n \n" + "PLAYER TO WIN: " + (int)GameManager.CurrentPlayer + " \n";

        //iterate over each row in the grid
        for (int row = 0; row < BoardState.BoardDimension; row++)
        {
            //add text to log
            consoleLog += "ROW " + row + ": ";
            //iterate over each column
            for (int col = 0; col < BoardState.BoardDimension; col++)
            {
                AddTestPosition(row, col);

                yield return new WaitForSeconds(seconds);

                //checks if we reached the end of the row and if so, checks row for success
                if (col == BoardState.BoardDimension - 1 && BoardState.CheckRow(row, playerCheck))
                {
                    Debug.Log("Registered victory for Player: " + playerCheck);
                    consoleLog += "Success!";
                    //start over again
                    GameManager.instance.BoardStateTestReset();
                    GameManager.instance.SwitchCurrentPlayer();
                    //increment the success count
                    count++;
                    break;
                }
                //if not success, then we have a failure
                else if (col == BoardState.BoardDimension - 1 && !BoardState.CheckRow(row, playerCheck))
                {
                    Debug.LogError("Failed to register a victory for player " + playerCheck);
                    consoleLog += "Failure";
                    GameManager.instance.BoardStateTestReset();
                    break;
                }

                //set a position and tile for the other player right below the target player. Since this player goes second, they should never win
                int otherRow = row + 1;
                //if we're at the bottom of the grid, the other player should spawn at the top instead of trying to spawn below
                if (otherRow >= BoardState.BoardDimension)
                    otherRow = 0;
                AddTestPosition(otherRow, col);
                yield return new WaitForSeconds(seconds);
            }
            consoleLog += " \n";
        }
        //log the whole report
        Debug.Log(consoleLog);
        //open a debug window that summarizes our findings using Count
        GameManager.instance.DebugWindow("Tested all the Rows to see if Player " + playerCheck + " won " + BoardState.BoardDimension + " times. \n \n" + "Player " + playerCheck + " won " + count + " times.");
        yield return null;
    }

    //Checks all columns
    IEnumerator ColumnTester (float seconds)
    {
        //counts how many successful checks we have
        int count = 0;
        //store the player we're measuring. The current player should be set to the one we want to check when pressing the button in Debugger.
        int playerCheck = (int)GameManager.CurrentPlayer;
        //Stop controls
        GameManager.instance.DisableControls();

        //A string that logs the results of all column tests.
        string consoleLog = "==========COLUMN TEST RESULT===========" + "\n \n" + "PLAYER TO WIN: " + (int)GameManager.CurrentPlayer + " \n";

        //iterate over each column in the grid
        for (int col = 0; col < BoardState.BoardDimension; col++)
        {
            //add text to log
            consoleLog += "COLUMN " + col + ": ";
            //iterate over each column
            for (int row = 0; row < BoardState.BoardDimension; row++)
            {
                //set position in the board array and also spawns a tile
                AddTestPosition(row, col);

                yield return new WaitForSeconds(seconds);

                //checks if we reached the end of the row and if so, checks row for success
                if (row == BoardState.BoardDimension - 1 && BoardState.CheckColumn(col, playerCheck))
                {
                    Debug.Log("Registered victory for Player: " + playerCheck);
                    consoleLog += "Success!";
                    //start over again
                    GameManager.instance.BoardStateTestReset();
                    GameManager.instance.SwitchCurrentPlayer();
                    //increment the success count
                    count++;
                    break;
                }
                //if not success, then we have a failure
                else if (row == BoardState.BoardDimension - 1 && !BoardState.CheckColumn(col, playerCheck))
                {
                    Debug.LogError("Failed to register a victory for player " + playerCheck);
                    consoleLog += "Failure";
                    GameManager.instance.BoardStateTestReset();
                    break;
                }

                //set a position and tile for the other player right next the target player. Since this player goes second, they should never win
                int otherCol = col + 1;
                //if we're at the end of the grid, the other player should spawn at the start instead of trying to spawn next to it
                if (otherCol >= BoardState.BoardDimension)
                    otherCol = 0;
                AddTestPosition(row, otherCol);
                yield return new WaitForSeconds(seconds);
            }
            consoleLog += " \n";
        }
        //log the whole report
        Debug.Log(consoleLog);
        //open a debug window that summarizes our findings using Count
        GameManager.instance.DebugWindow("Tested all the Columns to see if Player " + playerCheck + " won " + BoardState.BoardDimension + " times. \n \n" + "Player " + playerCheck + " won " + count + " times.");
        yield return null;
    }

    //Check both diagonals
    IEnumerator DiagonalTester(float seconds)
    {
        //tracks which diagonal was successful
        bool front = false;
        bool back = false;
        //store the player we're measuring. The current player should be set to the one we want to check when pressing the button in Debugger.
        int playerCheck = (int)GameManager.CurrentPlayer;
        //Stop controls
        GameManager.instance.DisableControls();

        //A string that logs the results of all column tests.
        string consoleLog = "==========DIAGONAL TEST RESULT===========" + "\n \n" + "PLAYER TO WIN: " + (int)GameManager.CurrentPlayer + " \n";

        //add text to log
        consoleLog += "FRONT DIAGONAL /:  ";
        //iterate over each column in the grid
        for (int col = 0; col < BoardState.BoardDimension; col++)
        {
            //set new position in diagonal
            int row = BoardState.BoardDimension - 1 -col;

            AddTestPosition(row, col);

            yield return new WaitForSeconds(seconds);

            //checks if we reached the end of the row and if so, checks diagonal
            if (col == BoardState.BoardDimension - 1 && BoardState.CheckFrontDiagonal(playerCheck))
            {
                Debug.Log("Registered / Victory for Player: " + playerCheck);
                consoleLog += "Success! \n";
                //start over again
                GameManager.instance.BoardStateTestReset();
                GameManager.instance.SwitchCurrentPlayer();
                //set the success bool check
                front = true;
                break;
            }
            //if not success, then we have a failure
            else if (col == BoardState.BoardDimension - 1 && !BoardState.CheckFrontDiagonal(playerCheck))
            {
                Debug.LogError("Failed to register a / victory for player " + playerCheck);
                consoleLog += "Failure \n";
                //we have a fail
                front = false;
                GameManager.instance.BoardStateTestReset();
                break;
            }

            //set a position and tile for the other player right next the target player. Since this player goes second, they should never win
            int otherRow = row - 1;
            AddTestPosition(otherRow, col);
            yield return new WaitForSeconds(seconds);
        }

        //add text to log
        consoleLog += "BACK DIAGONAL \\ :  ";
        //iterate over each column in the grid
        for (int row = 0; row < BoardState.BoardDimension; row++)
        {
            //set new position in diagonal
            int col =  row;
            AddTestPosition(row, col);

            yield return new WaitForSeconds(seconds);

            //checks if we reached the end of the row and if so, checks diagonal
            if (row == BoardState.BoardDimension - 1 && BoardState.CheckBackDiagonal(playerCheck))
            {
                Debug.Log("Registered \\ Victory for Player: " + playerCheck);
                consoleLog += "Success! \n";
                //start over again
                GameManager.instance.BoardStateTestReset();
                GameManager.instance.SwitchCurrentPlayer();
                //set the success bool check
                back = true;
                break;
            }
            //if not success, then we have a failure
            else if (row == BoardState.BoardDimension - 1 && !BoardState.CheckBackDiagonal(playerCheck))
            {
                Debug.LogError("Failed to register a \\ victory for player " + playerCheck);
                consoleLog += "Failure \n";
                //we have a fail
                back = false;
                GameManager.instance.BoardStateTestReset();
                break;
            }

            //set a position and tile for the other player right next the target player. Since this player goes second, they should never win
            int otherRow = row + 1;
            AddTestPosition(otherRow, col);
            yield return new WaitForSeconds(seconds);
        }

        //log the whole report
        Debug.Log(consoleLog);
        //open a debug window that summarizes our findings using Count
        GameManager.instance.DebugWindow("Successful Front Diagonal / : " + front + " \n Successful Back Diagonal  \\ : " + back);
        yield return null;
    }

    //Checks for diagonals
    //Currently does a single check of same col placements followed by an alternating col at the end for a guaranteed draw. Then tests all the check row/col/diag functions to see if any detect a result
    //basically tests to see if the game detects a victory when it's not there
    IEnumerator DrawTester(float seconds)
    {
        //Check to determine if we have a draw or not.
       bool drawCheck = false;
        //store dimension so we can quit calling BoardState all the time
       int dimension = BoardState.BoardDimension;
       //Stop controls
       GameManager.instance.DisableControls();
        //string that reports result of the test
        string consoleLog = "===============DRAW TEST RESULT================= \n";
       //store the initial value of the player to check
       int playerCheck = (int)GameManager.CurrentPlayer;


        //iterate over each row to generate a pattern
        for (int row = 0; row < dimension; row++)
        {
            //ensure that each row starts with the same pieces
            if (playerCheck != (int)GameManager.CurrentPlayer)
                GameManager.instance.SwitchCurrentPlayer();

            //switches it back at the very last row to make sure we get a draw
            //set fail test to false to fail the draw test
            if (row == dimension - 1 && !failTest)
            {
                GameManager.instance.SwitchCurrentPlayer();
            }

            //populate over the row
            for (int col = 0; col < dimension; col++)
            {
                //set position in the board array and also spawns a tile
                AddTestPosition(row, col);
                yield return new WaitForSeconds(seconds);
            }
        }

        //Checks for any draws
        for (int i = 0; i < BoardState.BoardDimension; i++)
        {
            //row check and log
            if (BoardState.CheckRow(i, (int)GameManager.Player.P1) || BoardState.CheckRow(i, (int)GameManager.Player.P2))
            {
                Debug.LogError("Test Failure: A Non-Draw outcome was detected in a row! ");
                consoleLog += "ROW " + i + ": FAILED  \n";
                drawCheck = true;
            }
            else
            {
                consoleLog += "ROW " + i + ": SUCCESSFUL \n";
            }
            //col check and log
            if (BoardState.CheckColumn(i, (int)GameManager.Player.P1) || BoardState.CheckColumn(i, (int)GameManager.Player.P2))
            {
                Debug.LogError("Test Failure: A Non-Draw outcome was detected in a Column! ");
                consoleLog += "COLUMN " + i + ": FAILED  \n";
                drawCheck = true;
            }
            else
            {
                consoleLog += "COLUMN " + i + ": SUCCESSFUL \n";
            }
        }

        //diag check and log
        if (BoardState.CheckFrontDiagonal((int)GameManager.Player.P1) || BoardState.CheckFrontDiagonal((int)GameManager.Player.P2))
        {
            Debug.LogError("Test Failure: A Non-Draw outcome was detected in a Diagonal! ");
            consoleLog += "DIAGONALS : FAILED  \n";
            drawCheck = true;
        }
        else
        {
            consoleLog += "DIAGONALS : SUCCESSFUL \n";
        }

        //log the whole report
        Debug.Log(consoleLog);
        //open a debug window that summarizes our findings using Count
        string message;
        if (drawCheck)
            message = "Draw Test Failure:  \n   The game detected a Victory";
        else
            message = "Draw Test Success:  \n   The game did not detect a victory!";
        GameManager.instance.DebugWindow(message);
        yield return null;

    }


    #endregion


    #region Repeated Functions in the coroutines
    //adds a position in the board array and spawns a tile in the corresponding empty tile. Switches player at the end. 
    private void AddTestPosition(int row, int col)
    {
        BoardState.SetPositionInBoard(new Vector2Int(row, col), (int)GameManager.CurrentPlayer);
        BoardState.EmptyTileArray[row, col].SpawnTile();
        GameManager.instance.SwitchCurrentPlayer();
    }

    #endregion
#endif
}
