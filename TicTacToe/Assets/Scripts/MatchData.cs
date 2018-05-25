using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Data struct that stores the data for a single match
//Designed to allow replication of any game choices. Each player has a list assigned to them and moves are assigned to list. The startplayer variable lets you determine which list should be used first. Can replicate the icons and the board dimensions 
public struct MatchData
{
    public List<Vector2Int> playerOneMoves;         //array for all player one moves. Each index shows the turn order
    public List<Vector2Int> playerTwoMoves;         //array for all player two moves
    public int pOneIcon;                            //icon for player one
    public int pTwoIcon;                            //icon for player two
    public int gameResult;                          //records who won this game: 0 for no winner; 1 for player 1; 2 for player 2; 3 is for draw; 4 is for match ended in error
    public int boardDimension;                      //records the dimension of the board for this particular match
    public int startingPlayer;                      // which player started the match. ZERO INDEXED: 0 for player 1, 1 for player 2

    //constructor. lists and the winner don't need to be set upon match finish
    public MatchData(int pOneIcon, int pTwoIcon, int boardDimension, int startingPlayer)
    {
        playerOneMoves = new List<Vector2Int>();
        playerTwoMoves = new List<Vector2Int>();
        gameResult = 0;
        this.pOneIcon = pOneIcon;
        this.pTwoIcon = pTwoIcon;
        this.boardDimension = boardDimension;
        this.startingPlayer = startingPlayer;
    }
}
