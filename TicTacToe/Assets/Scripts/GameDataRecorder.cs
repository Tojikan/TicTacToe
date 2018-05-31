using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//class that handles the recording of all player moves and such.
//Singleton instance that lets you call this from anywhere to record things as they happen.
//Game Manager initializes the values of the game and records the game results
//Player Controller will record moves
public class GameDataRecorder : MonoBehaviour
{
    public static GameDataRecorder instance = null;                             //Make this class a singleton pattern
    private List<MatchData> matchList;                                          //list of all matches in the session of the game. 
    public List<MatchData> MatchList
    {
        get
        {
            return matchList;
        }
    }                                         //read property


    private void Awake()
    {
        //creates a singleton pattern to ensure only one instance of this class can exist at a time
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        //initialize list
        matchList = new List<MatchData>();
    }

    #region add new data
    //Creates a new match and adds it to the end of the list of matches
    public void AddNewMatch(int p1, int p2, int board, int player)
    {
        MatchData newMatch = new MatchData(p1, p2, board, player);
        matchList.Add(newMatch);
    }


    //add a new move to the current match player array
    //positions are stored as a Vector2Int that refers to the RxC on the board array. 
    //-1: surrender           -2:  indicates new game was setup
    public void AddPlayerMove(Vector2Int position)
    {
        //get the current match
        MatchData currentMatch = matchList[matchList.Count - 1];
        //depending on current player, 
        if (GameManager.CurrentPlayer == GameManager.Player.P1)
        {
            currentMatch.playerOneMoves.Add(position);
        }
        else
        {
            currentMatch.playerTwoMoves.Add(position);
        }
    }

    //records the finish state of the game
    //0: Draw          1: P1          1: P2        3: New Game Started      4: Error
    public void RecordGameFinish (int game)
    {
        Debug.Log("Recorded a game finish of " + game);
        MatchData match = matchList[(matchList.Count - 1)];
        match.GameResult = game;
        matchList[(matchList.Count - 1)] = match;
    }
    #endregion

    #region report data to console
    //used for testing purposes to see if recording game moves. Call this with the number of the match.
    public void ReportGame(int num)
    {
        MatchData match = matchList[num];

        //simply just get the data
        string message = "========\n" + "GAME" + num + "\n";
        message += "Board Dimension of " + match.boardDimension + "\n";
        message += ("Player 1 icon is " + match.pOneIcon) + "\n";
        message += ("Player 2 icon is " + match.pTwoIcon) + "\n";
        message += ("Game Result was " + match.GameResult + ". 0 = no winner, 1 = p1, 2 = p2, 3 = draw, 4 = error") + "\n";
        message += ("Starting player was " + (match.startingPlayer) + ". ") + "\n";

        //figures out which is the highest count between both player arrays to determine who made the last move
        int c;
        if (match.playerOneMoves.Count > match.playerTwoMoves.Count)
        {
            c = match.playerOneMoves.Count;
        }
        else
        {
            c = match.playerTwoMoves.Count;
        }

        //container lists to store the data obtained from the record
        List<Vector2Int> firstPlayerMoves;
        List<Vector2Int> secondPlayerMoves;
        //figure out who went first and then adds that to the container
        if (match.startingPlayer == 0)
        {
            firstPlayerMoves = match.playerOneMoves;
            secondPlayerMoves = match.playerTwoMoves;
        }
        else
        {
            firstPlayerMoves = match.playerTwoMoves;
            secondPlayerMoves = match.playerOneMoves;
        }

        //iterate over the containers to log the message
        for (int i = 0; i < c; i++)
        {

            if (i < firstPlayerMoves.Count)
            {
                if (firstPlayerMoves[i].x == -1)
                    message += "Turn " + i + ": First Player Surrenders";
                else if (firstPlayerMoves[i].x == -2)
                    message += "Turn " + i + ": Game was ended on Player One's turn.";
                else
                    message += ("Turn " + i + ": Player " + match.startingPlayer + " moves to " + firstPlayerMoves[i] + "\n"); 
            }
            if (i < secondPlayerMoves.Count)
            {
                if (secondPlayerMoves[i].x == -1)
                    message += "Turn " + i + ": Second Player Surrenders";
                else if (secondPlayerMoves[i].x == -2)
                    message += "Turn " + i + ": Game was ended on Player Two's turn.";
                else
                    message += ("Turn " + i + ": Second Player moves to " + secondPlayerMoves[i] + "\n");
            }
        }
        Debug.Log(message);
    }
    #endregion
}
