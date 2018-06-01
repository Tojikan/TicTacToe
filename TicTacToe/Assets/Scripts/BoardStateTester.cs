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
    public float timeBetweenMoves = 0.25f;                     //sets how much time between movements when testing
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

    public void StartRowTest(int row)
    {
        StartCoroutine("RowTester", row);
    }

    public void StartColumnTest(int col)
    {
        StartCoroutine("ColumnTester", col);
    }

    public void StartDiagonalTest(int diag)
    {
        StartCoroutine("DiagonalTester", diag);
    }

    public void StartDrawTest()
    {
        StartCoroutine("DrawTester");
    }



    #endregion


    /** Board State Checker oroutines
     * Functions by playing through a row/column/diagonal. 
     * Use a coroutine in order to simulate faster playing. Pass in a parameter of time to wait in between moves
     * The starting player should always win. The second turn player will simply place a piece underneath the starting player and should thus never beat the starting player.
     * If a game doesn't register a victory, it'll spit out a debug.log. The test will spit out a log at the end. 
     * To make sure this is setup properly, the board MUST be cleared and the starting player set in the debugger method that calls this coroutine. That should be done in Debugger before starting the coroutine.
     * **/



    #region Coroutines
    //Checks all rows. Target player is the one who should win
    IEnumerator RowTester(int row)
    {
        //get the row for the other player, either below or above the target row 
        int otherRow = (row + 1 >= BoardState.BoardDimension) ? row - 1 : row + 1;

        //player to win. Used for error checks
        int player = (int)GameManager.CurrentPlayer;

        //iterate over each row in the grid
        for (int i = 0; i < BoardState.BoardDimension; i++)
        {
            //wait between pieces
            yield return new WaitForSeconds(timeBetweenMoves);

            //add piece for target player
            AddTestPosition(row, i);
            if (CheckBoardState("Row", row))
            {
                Debug.Log("Successfully logged Victory for Player :" + (int)GameManager.CurrentPlayer + " in Row " + row);
                GameManager.instance.GameFinishDebug();
                yield break;
            }
            GameManager.instance.SwitchCurrentPlayer();

            //wait between pieces
            yield return new WaitForSeconds(timeBetweenMoves);

            //add piece for other player and then check
            AddTestPosition(otherRow, i);
            if (CheckBoardState("Row", otherRow))
            {
                Debug.LogError("Did not detect a victory for player " + player + ".");
                GameManager.instance.GameFinishDebug();
                yield break;
            }
            GameManager.instance.SwitchCurrentPlayer();
        }
        Debug.LogError("Did not detect a victory for player " + player + ".");
    }

    //Checks all columns
    IEnumerator ColumnTester (int col)
    {
        //get the col for the other player, either below or above the target col 
        int otherCol = (col + 1 >= BoardState.BoardDimension) ? col - 1 : col + 1;

        //player to win. Used for error checks
        int player = (int)GameManager.CurrentPlayer;


        //iterate over each row in the grid
        for (int i = 0; i < BoardState.BoardDimension; i++)
        {
            //wait between pieces
            yield return new WaitForSeconds(timeBetweenMoves);

            //add piece for target player
            AddTestPosition(i, col);
            if (CheckBoardState("Column", col))
            {
                GameManager.instance.GameFinishDebug();
                Debug.Log("Successfully logged Victory for Player :" + (int)GameManager.CurrentPlayer + " in Col " + col);
                yield break;
            }
            GameManager.instance.SwitchCurrentPlayer();

            //wait between pieces
            yield return new WaitForSeconds(timeBetweenMoves);

            //add piece for other player and then check
            AddTestPosition(i, otherCol);
            if (CheckBoardState("Column", otherCol))
            {
                Debug.LogError("Did not detect a victory for player " + player + ".");
                GameManager.instance.GameFinishDebug();
                yield break;
            }
            GameManager.instance.SwitchCurrentPlayer();
        }
        Debug.LogError("Did not detect a victory for player " + player + ".");
    }

    //Check both diagonals. Pass an int (from an Enum in Debugger) to determine which diagonal to select
    IEnumerator DiagonalTester(int diag)
    {
        //store the player we're measuring. The current player should be set to the one we want to check when pressing the button in Debugger.
        int playerCheck = (int)GameManager.CurrentPlayer;

        if (diag == (int)Debugger.DiagSelect.Front)
        {
            for (int col = 0; col < BoardState.BoardDimension; col++)
            {
                //wait
                yield return new WaitForSeconds(timeBetweenMoves);

                //get a row position
                int row = BoardState.BoardDimension - 1 - col;
                //set new position in diagonal
                AddTestPosition(row, col);

                //checks if we reached the end of the row and if so, checks diagonal
                if (BoardState.CheckFrontDiagonal((int)GameManager.CurrentPlayer))
                {
                    Debug.Log("Successfully logged a Front Diagonal Victory for Player :" + (int)GameManager.CurrentPlayer);
                    GameManager.instance.GameFinishDebug();
                    yield break;
                }
                //switch player
                GameManager.instance.SwitchCurrentPlayer();

                //wait
                yield return new WaitForSeconds(timeBetweenMoves);

                //set a position and tile for the other player right next the target player. Since this player goes second, they should never win
                int otherRow = (row - 1 >= 0) ? row - 1 : row + 1;
                AddTestPosition(otherRow, col);
                //switch
                GameManager.instance.SwitchCurrentPlayer();
            }
            Debug.LogError("Failed to register a Front Diagonal victory for player " + playerCheck);
            GameManager.instance.DebugWindow("ERROR: \n No Diagonal Victory registered.");
        }

        if (diag == (int)Debugger.DiagSelect.Back)
        {
            for (int i = 0; i < BoardState.BoardDimension; i++)
            {
                yield return new WaitForSeconds(timeBetweenMoves);

                //set new position in diagonal
                AddTestPosition(i, i);

                //checks if we reached the end of the row and if so, checks diagonal
                if (BoardState.CheckBackDiagonal((int)GameManager.CurrentPlayer))
                {
                    Debug.Log("Successfully logged a Back Diagonal Victory for Player :" + (int)GameManager.CurrentPlayer);
                    GameManager.instance.GameFinishDebug();
                    yield break;
                }
                //switch player
                GameManager.instance.SwitchCurrentPlayer();

                //wait
                yield return new WaitForSeconds(timeBetweenMoves);

                //set a position and tile for the other player right next the target player. Since this player goes second, they should never win
                int otherI = (i + 1 >= BoardState.BoardDimension) ? i - 1 : i + 1;
                AddTestPosition(otherI, i);
                //switch player
                GameManager.instance.SwitchCurrentPlayer();
            }
            Debug.LogError("Failed to register a Back Diagonal victory for player " + playerCheck);
            GameManager.instance.DebugWindow("ERROR: \n No Diagonal Victory registered.");
        }
        Debug.LogError("Invalid Diagonal Input" + playerCheck);
        GameManager.instance.DebugWindow("ERROR: \n Invalid Diagonal Input.");
    }

    //Currently does a single check of same col placements followed by an alternating col at the end for a guaranteed draw. Then tests all the check row/col/diag functions to see if any detect a result
    //basically tests to see if the game detects a victory when it's not there
    IEnumerator DrawTester()
    {
        //store dimension so we can quit calling BoardState all the time
        int dimension = BoardState.BoardDimension;
        //store the player we're checking for
        int playerCheck = (int)GameManager.CurrentPlayer;

        //iterate over each row to generate a pattern
        for (int row = 0; row < dimension; row++)
        {
            //ensure that each row starts with the same pieces
            if (playerCheck != (int)GameManager.CurrentPlayer)
                GameManager.instance.SwitchCurrentPlayer();

            //switches it back at the very last row to make sure we get a draw
            if (row == dimension - 1)
            {
                GameManager.instance.SwitchCurrentPlayer();
            }

            //iterate over the row
            for (int col = 0; col < dimension; col++)
            {
                //set position in the board array and also spawns a tile
                AddTestPosition(row, col);
                GameManager.instance.SwitchCurrentPlayer();
                //wait
                yield return new WaitForSeconds(timeBetweenMoves);

                //check if we won a row or column by accident somehow
                if (CheckBoardState("Row", row) || CheckBoardState("Column", row))
                {
                    Debug.LogError("A victory was detected when there should be none.");
                    GameManager.instance.GameFinishDebug();
                    yield break;
                }
            }
        }
        //checks all rows, cols, and diags for a draw
        if (CheckBoardState("Draw", 0))
        {
            GameManager.instance.GameDrawDebug();
        }
        else if (!CheckBoardState("Draw",0))
        {
            GameManager.instance.DebugWindow("ERROR: \n A draw was not detected!");
        }
    }


    #endregion


    #region Repeated Functions in the coroutines
    //adds a position in the board array and spawns a tile in the corresponding empty tile. Switches player at the end. 
    private void AddTestPosition(int row, int col)
    {
        if (BoardState.BoardPositions[row, col] != 0)
        {
            StopAllCoroutines();
            GameDataRecorder.instance.AddPlayerMove(new Vector2Int(-2, -2));
            GameDataRecorder.instance.RecordGameFinish(4);
            GameManager.instance.DebugWindow("ERROR:  \n  Space is already occupied by another Tile. Exiting Test");
            return;
        }
        BoardState.AddPosition(new Vector2Int(row, col), (int)GameManager.CurrentPlayer);
        GameDataRecorder.instance.AddPlayerMove(new Vector2Int(row, col));
        try
        {
            BoardState.EmptyTileArray[row, col].SpawnTile();
        }
        catch (System.Exception e)
        {
            StopAllCoroutines();
            Debug.LogError(e);
            GameDataRecorder.instance.AddPlayerMove(new Vector2Int(-2, -2));
            GameDataRecorder.instance.RecordGameFinish(4);
            GameManager.instance.DebugWindow("ERROR:  \n  Unable to Spawn Tile. Exiting Test");
            return;
        }
    }

    //Checks a board state. Give it string parameters of "Row", "Column", "Diagonal", or "Draw". Dimension only matters for row and column checks
    //If Draw return a false, then something achieved victory
    private bool CheckBoardState(string check, int dimension)
    {
        if (check != "Row" && check != "Column" && check!= "Diagonal" && check != "Draw")
        {
            Debug.LogError("Invalid board state to check");
            return false;
        }

        if (check == "Row")
        {
            if (BoardState.CheckRow(dimension, (int)GameManager.CurrentPlayer))
            {
                return true;
            }
            return false;
        }


        else if (check == "Column")
        {
            if (BoardState.CheckColumn(dimension, (int)GameManager.CurrentPlayer))
            {
                return true;
            }
            return false;
        }

        else if (check == "Diagonal")
        {
            if (BoardState.CheckBackDiagonal((int)GameManager.CurrentPlayer))
            {
                return true;
            }
            if (BoardState.CheckFrontDiagonal((int)GameManager.CurrentPlayer))
            {
                return true;
            }
            return false;
        }

        else if (check == "Draw")
        {
            for (int i = 0; i < BoardState.BoardDimension; i++)
            {
                if (BoardState.CheckColumn(i, (int)GameManager.Player.P1) || BoardState.CheckRow(i, (int)GameManager.Player.P1) ||
                    BoardState.CheckColumn(i, (int)GameManager.Player.P2) || BoardState.CheckRow(i, (int)GameManager.Player.P2))
                    return false;
                    
            }
            if (BoardState.CheckBackDiagonal((int)GameManager.CurrentPlayer) || BoardState.CheckFrontDiagonal((int)GameManager.CurrentPlayer))
                return false;
            else
                return true;
        }

        return false; 
    }

    #endregion
#endif
}
