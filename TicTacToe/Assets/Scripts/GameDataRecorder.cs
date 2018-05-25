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

    //Creates a new match and adds it to the end of the list of matches
    public void AddNewMatch(int p1, int p2, int board, int player)
    {
        MatchData newMatch = new MatchData(p1, p2, board, player);
        matchList.Add(newMatch);
    }


    //add a new move to the current match player array
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

    public void RecordGameFinish (int game)
    {
        MatchData match = matchList[matchList.Count - 1];
        match.gameResult = game;
    }


    //used for testing purposes to see if recording game moves. Call this with the number of the match.
    public void RecreateGame(int num)
    {
        MatchData match = matchList[num];
        Debug.Log("------------------------------------");
        Debug.Log("GAME" + num);
        Debug.Log("Board Dimension of " + match.boardDimension);
        Debug.Log("Player 1 icon is " + match.pOneIcon);
        Debug.Log("Player 2 icon is " + match.pTwoIcon);
        Debug.Log("Game Result was " + match.gameResult + ". 0 = no winner, 1 = p1, 2 = p2, 3 = draw, 4 = error");
        Debug.Log("Starting player was " + match.startingPlayer + ". If this value is not 1 or 0, something wrong happened");
        Debug.Log("Winner was " + match.gameResult);

        int c;
        if (match.playerOneMoves.Count > match.playerTwoMoves.Count)
            c = match.playerOneMoves.Count;
        else
            c = match.playerTwoMoves.Count;
        List<Vector2Int> firstPlayerMoves;
        List<Vector2Int> secondPlayerMoves;
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

        for (int i = 0; i < c; i++)
        {
            if (i < firstPlayerMoves.Count)
                Debug.Log("Player " + match.startingPlayer + " moves to " + firstPlayerMoves[i]);
            if (i < secondPlayerMoves.Count )
                Debug.Log("Second Player moves to " + secondPlayerMoves[i]);
        }
        Debug.Log("------------------------------------");
    }
}
