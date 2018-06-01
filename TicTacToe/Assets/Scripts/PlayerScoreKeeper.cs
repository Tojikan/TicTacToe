using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Class that handles effects for the player score keepers on the side of the screen
public class PlayerScoreKeeper : MonoBehaviour
{
    public Text playerText;                                                 //drag the player text
    private Color faded = new Color(255, 255, 255, 0);                     //color for fading out
    private Color clear = new Color(255, 255, 255, 255);                    //color for clear text

    //Set in gameManager by current player in a property accessor. Fade out the score when not player turn. 
    public void FadeScore(bool playerTurn)
    {
        if (playerTurn)
        {
            playerText.color = clear;
        }

        else if (!playerTurn)
        {
            playerText.color = faded;
        }
        
    }
}
