using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Class that handles effects for the player score keepers on the side of the screen
public class PlayerScoreKeeper : MonoBehaviour
{
    public Text scoreText;                                                  //drag score text
    public Text playerText;                                                 //drag the player text
    public Image playerImage;                                               //drag the image 
    public PlayerIconSet iconSet;                                           //drag the player icon set to here that stores player tiles
    private Sprite[] spriteSet;                                             //Array to store the sprites from the icon set during Awake. Easy to add more icons
    private Color faded = new Color(255, 255, 255, 50);                     //color for fading out
    private Color clear = new Color(255, 255, 255, 255);                    //color for clear text



    private void Awake()
    {
        //gets the sprite info from the iconSet and adds it to an array
        spriteSet = new Sprite[iconSet.largeIcons.Length];
        for (int i = 0; i < spriteSet.Length; i++)
        {
            spriteSet[i] = iconSet.largeIcons[i].transform.GetComponentInChildren<SpriteRenderer>().sprite;
        }
    }

    //sets the image below player. This is set in the property accessor everytime the player icon number in gameManager changes
    public void SetImage (int i)
    {
        playerImage.sprite = spriteSet[i];
    }

    //Set in gameManager by current player in a property accessor. Fade out the score when not player turn. 
    public void FadeScore(bool playerTurn)
    {
        if (playerTurn)
        {
            scoreText.color = faded;
            playerText.color = faded;
            playerImage.color = faded;
        }

        else
        {
            scoreText.color = clear;
            playerText.color = clear;
            playerImage.color = clear;
        }
        
    }
}
