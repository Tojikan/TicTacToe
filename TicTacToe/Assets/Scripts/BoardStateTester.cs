using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardStateTester : MonoBehaviour
{
#if UNITY_EDITOR
    public static BoardStateTester instance = null;                                   //singleton for simple reference getting as this will only be called by the Debugger

    //singleton
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }


    /** Board State Checker oroutines
     * Functions by playing through a row/column/diagonal. 
     * Use a coroutine in order to simulate faster playing. Pass in a parameter of time to wait in between moves
     * The starting player should always win. The second turn player will simply place a piece underneath the starting player and should thus never beat the starting player.
     * If a game doesn't register a victory, it'll spit out a debug.log. The test will spit out a log at the end. 
     * To make sure this is setup properly, the board MUST be cleared and the starting player set in the debugger method that calls this coroutine. That should all be set before starting the coroutine.
     * **/

    public void StartRowTest(float wait)
        {
            StartCoroutine("RowTester", wait);
        }

    //Checks all rows
    IEnumerator RowTester(float seconds)
    {
        int playerCheck = (int)GameManager.CurrentPlayer;
        GameManager.instance.DisableControls();
        
        if (BoardState.BoardPositions == null || BoardState.EmptyTileArray == null)
        {
            Debug.LogError("Board and Tile arrays not detected: did you forget to generate a board?");
            yield return null;
        }
        string consoleLog = "==========ROW TEST RESULT===========" + "\n \n" + "PLAYER TO WIN: " + (int)GameManager.CurrentPlayer + " \n";

        for (int row = 0; row < BoardState.BoardDimension; row++)
        {
            consoleLog += "ROW " + row + ": ";
            for (int col = 0; col < BoardState.BoardDimension; col++)
            {
                BoardState.SetPositionInBoard(new Vector2Int(row, col), (int)GameManager.CurrentPlayer);
                BoardState.EmptyTileArray[row, col].SpawnTile();
                GameManager.instance.SwitchCurrentPlayer();
                yield return new WaitForSeconds(seconds);

                if (col == BoardState.BoardDimension - 1 && BoardState.CheckRow(row, playerCheck))
                {
                    Debug.Log("Registered victory for Player: " + playerCheck);
                    consoleLog += "Success!";
                    GameManager.instance.BoardStateTestReset();
                    GameManager.instance.SwitchCurrentPlayer();
                    break;
                }
                else if (col == BoardState.BoardDimension - 1 && !BoardState.CheckRow(row, playerCheck))
                {
                    Debug.LogError("Failed to register a victory for player " + playerCheck);
                    consoleLog += "Failure";
                    GameManager.instance.BoardStateTestReset();
                    break;
                }

                int otherRow = row + 1;
                if (otherRow >= BoardState.BoardDimension)
                    otherRow = 0;
                BoardState.SetPositionInBoard(new Vector2Int(otherRow, col), (int)GameManager.CurrentPlayer);
                BoardState.EmptyTileArray[otherRow, col].SpawnTile();
                GameManager.instance.SwitchCurrentPlayer();
                yield return new WaitForSeconds(seconds);
            }
            consoleLog += " \n";
        }
        Debug.Log(consoleLog);
        GameManager.instance.EnableControls();
        GameManager.instance.ClearBoard();
    }
#endif
}
