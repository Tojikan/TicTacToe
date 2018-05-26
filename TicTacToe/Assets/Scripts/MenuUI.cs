using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Handles the initial data input at the start of the game to change the settings and opening menus
//Works by disabling different panels based upon OnClick events for buttons or by calling functions from game manager
//Enables and disables different UI panel upon clicking - fixed state model for simplicity
public class MenuUI : MonoBehaviour
{
    public GameManager gameManager;                     //drag game manager here to get reference to it
    public GameObject finishScreen;                     //panel that displays the game over message
    public GameObject sizeSelect;                       //panel for selecting grid sizes
    public GameObject iconSelect;                       //panel for selecting player icons
    public GameObject startScreen;                      //StartScreen Panel. Should only show up at the beginning
    public GameObject gameUIPanel;                      //In-game UI during matches. 
    public Text topMessage;                             //drag the text that is the top line of the game over screen
    public Text bottomMessage;                          //bottom line of game over screen
    public Button iconSelectConfirm;                    //drag the icon done button here. Disable/enable if both players haven't or have been selected
    private bool playerOneSelected;                     //bool check if player one has selected an icon yet
    private bool playerTwoSelected;                     //bool check if player two has selected an icon
    private Image image;                                //For disabling/enabling the base UI panel if we start a game

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    //Clears the panel for anything left open in editor view and opens the icon select panel
    private void Start()
    {
        ClearPanel();
        gameUIPanel.SetActive(false);
        startScreen.SetActive(true);
        ShowPanel();
        playerOneSelected = true;
        playerTwoSelected = true;
    }

    //Called once when clicking on start button. Hides startscreen and goes to icon select
    public void StartGameButton()
    {
        startScreen.SetActive(false);
        iconSelect.SetActive(true);
    }

    #region Top buttons - the in-game buttons that go back to menu or surrenders the current players turn


    #endregion


    #region Icon Select - anything related to icon select
    //Update check to see if player one or two HASN'T selected an icon. In that case, we disable the okay button
    private void Update()
    {
        if (!playerOneSelected || !playerTwoSelected)
        {
            iconSelectConfirm.gameObject.SetActive(false);
        }
        else if (playerOneSelected && playerTwoSelected)
        {
            iconSelectConfirm.gameObject.SetActive(true);
        }
    }

    //goes to next section
    public void ToSizeSelect()
    {
        ClearPanel();
        sizeSelect.SetActive(true);
    }

    //OnClick event by UI button. Parameter of the icon value of the button. A bunch of if statements handle logic
    public void IconSelect(int icon)
    {
        //if the icon clicked on is already player one, then we cancel it
        if (icon == gameManager.PlayerOneIcon)
        {
            //Checks that it isn't already selected by player two and that we have a player one selected
            if (icon != gameManager.PlayerTwoIcon && playerOneSelected)
            {
                //By setting to 4, the effect script will remove any effects or indicators. The value is clamped anyway when instantiating tiles later
                gameManager.PlayerOneIcon = 4;
                playerOneSelected = false;
                return;
            }
        }
        //if the icon clicked on is NOT player one and we haven't selected, then set it to player one
        else if (icon != gameManager.PlayerOneIcon && !playerOneSelected && icon != gameManager.PlayerTwoIcon)
        {
            gameManager.PlayerOneIcon = icon;
            playerOneSelected = true;
            return;
        }

        if (icon == gameManager.PlayerTwoIcon)
        {
            if (icon != gameManager.PlayerOneIcon && playerTwoSelected)
            {
                gameManager.PlayerTwoIcon = 4;
                playerTwoSelected = false;
                return;
            }
        }

        else if (icon != gameManager.PlayerTwoIcon && !playerTwoSelected && icon != gameManager.PlayerTwoIcon)
        {
            gameManager.PlayerTwoIcon = icon;
            playerTwoSelected = true;
            return;
        }
    }


    #endregion


    #region finish screen - screen at the finish. The first button will just OnClick to Game Manager while the second button goes back to icon select
    //sets the text in the finish screen and opens it
    public void FinishScreen(string top, string bottom)
    {
        ClearPanel();
        ShowPanel();
        topMessage.text = top;
        bottomMessage.text = bottom;
        finishScreen.SetActive(true);
    }

    //second button - goes back to icon select window.
    public void BackToSetup()
    {
        ClearPanel();
        gameManager.ClearBoard();
        ShowPanel();
        iconSelect.SetActive(true); 
    }

    //starts a new game with same settings
    public void PlayAgain()
    {
        ClearPanel();
        HidePanel();
        gameManager.StartNewGame();
    }


    #endregion
    
    #region Window for setting the grid dimension. Each onClick will set the board dimension
    //set the board dimension in board state. button OnClick 
    public void SetDimensionSize(int dimension)
    {
        //pass the dimension size to the board state
        BoardState.BoardDimension = dimension;
        //hide the select size object so the panel is blank for the future
        ClearPanel();
        HidePanel();
        gameManager.StartNewGame();
    }
    #endregion board size select

    #region panel functions for showing, hiding, and clearing the panel
    //Hide the background panel and show game ui 
    void HidePanel()
    {
        image.enabled = false;
        gameUIPanel.SetActive(true);
    }

    //Show the background panel and hides the game ui
    void ShowPanel()
    {
        image.enabled = true;
        gameUIPanel.SetActive(false);
    }

    //clears the panel of all buttons/text
    void ClearPanel()
    {
            finishScreen.SetActive(false);
            sizeSelect.SetActive(false);
            iconSelect.SetActive(false);
    }

    #endregion
}
