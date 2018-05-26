using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Changes the effect depending on whether the icon is selected or not
public class IconSelectEffect : MonoBehaviour
{
    public int iconNumber;                                                      //Assign each icon a value corresponding to its value in the player tile array   
    public Text playerText;                                                     //player indicator text, which is shown whenever the icon is selected
    public GameManager gameManager;                                             //drag reference to game manager here which informs this which icon is currently selected.
    private Outline outline;                                                    //Outline to highlight a choice


    private void Awake()
    {
        outline = GetComponent<Outline>();
    }

    // Use this for initialization
    void Start ()
    {
        playerText.text = "";
        outline.enabled = false;
	}
	
    //Checks with gameManager to see if this icon is currently selected or not. Updates effects as needed
    void Update()
    {
        if (gameManager.PlayerOneIcon != iconNumber && gameManager.PlayerTwoIcon != iconNumber)
        {
            playerText.text = "";
            outline.enabled = false;
            return;
        }

        else if (gameManager.PlayerOneIcon == iconNumber) 
        {
            playerText.text = "P1";
            outline.enabled = true;
            outline.effectColor = Color.red;
        }

        else if (gameManager.PlayerTwoIcon == iconNumber)
        {
            playerText.text = "P2";
            outline.enabled = true;
            outline.effectColor = Color.blue;
        }
    }
}
