---------------Tic Tac Pro V0.2------------------- 
by Andrew Qian

------How To-----------
1. Start Game in /Build or in Unity Editor navigating to the main scene in the assets folder with Unity
2. Select character icons. Click on an already selected one to deselect (you can't proceed unless both are clicked)
3. Select Grid Size.
4. Play Game. Handoff mouses between turns
5. Probably get a draw
6. Start a new game by the menu at the end of a game or by the menu in the top left
7. Enjoy!!!

Player 1 goes first by default. After that it's loser goes first. 

The game supports grid sizes from 3x3 up to 8x8 and lets you pick from 4 player icons
You can play directly from the Standalone player in the Build folder or from by finding the 'Main' Scene in the Assets folder. Game only has one scene. 

All matches and move history are saved and can be accessed through the Debug Window.




/***Game Architecture***/
The game was designed with the ability to select different grid sizes and with the ability to save move history in mind.
There's a few key components here: 

- Game Manager: is the key component that tracks player's turns and controls the flow of the game. Its methods are called by pressing buttons on the UI menu. The switching of the current player is especially important as that is how the game checks for victories

- UI Menu: The in-game menu lets you set game parameters and set up new games.

- Board State: static class that handles the main game logic. It stores the state of the board and where players have placed their pieces. This class also checks the state of the board to see if anyone wins or if there is a draw.

- GameDataRecorder: Records data regarding each match into a struct called MatchData. MatchData stores information related to the game parameters and has two arrays that store the moves for the first player to move and the second player to move. This static class will save data after every match and create a new struct when a game is started.
		-  NOTE: that first player and second player names of the array DO NOT refer to P1 and P2 but rather the first player who moved in the match. You must use the first player variable to determine who actually went first.
		-  Game Result Codes are as follows:  
 			- 0: Winner undetermined. This is the default initial value. You could also arrive at this by starting a new game in the midst of a current game or using the Debug Window features
 			- 1: Player 1 victory
 			- 2: Player 2 victory
 			- 3: Draw
 			- 4: Error - Something went wrong writing to the BoardState Array. Used earlier to test parts of the game but it's still there.  
		- Player moves are stored as Vector2s ranging from 0,0 up.
			- If you get a -1,-1: that means the player surrendered
			- If you get a -2, -2: that means the game was ended that turn with no surrender.

- Player Controller: Allows mouse click by players. The input itself is a bool check which is set in Game Manager. Otherwise, it'll call the EmptyTile spawn functions and BoardState to record moves.

- Board Generator: Generates the board and also tells Board State to set up a new array when a new board is generated.

- EmptyTile: Changed from the previous version. Each empty tile space is its own object and has its own method to spawn a playertile when it is clicked on. After clicking on it, the empty tile is 'consumed' and deletes itself. This now handles the visual aspect of the game.

- The only Scriptable Object data container is the Player Icons set - just a  simple array of prefabs

- Debug Window (ONLY IN UNITY EDITOR) - This Debugger lets you control the state of the game from a custom editor window. This window has 3 main functions - printing match history from GameDataRecorder to the Unity console, generating new boards whenever you want, and conducting Board State tests. This tool should only function during play mode in the Unity Editor. You can set parameters for all three functions in their respective section. To open a window, navigate to the Windows/Debug Window option in the nav bar. 
	- The Print Match Data feature lets you print match history to the Unity Console from GameDataRecorder. You can enter a specific match to print or you can print all match history
	- The Board Generator lets you generate new boards without having to navigate through the UI. You can use this from any screen in the game. You can also set the starting player and the player icons.
	- The Board States tests replicate player actions by spawning tiles and recording to BoardState. You can set parameters for what you want to check (rows, columns, diagonals, draws, or All of them) and it'll play through a short 	and simple game to test the game. It is designed to fully simulate a match of the game. You can test multiple board states with the Test All feature but pressing any UI button or Debug Window function (other than printing match 	data) will stop any simulation. 

- BoardStateTester (ONLY IN UNITY EDITOR) - this is the component that actually conducts the board state tests. It consists of a bunch of Unity Coroutines and iterates over the current game board. This is attached to its own monobehavior because the DebugWindow is a Editor Window not a Monobehavior.

- Rest are other simple classes that manages specific job in the game. Player Icons are set via a ScriptableObject - just a simple array of prefabs.


- MatchData is the struct that all match information is saved in. It'll store things such as the player icons and which player started the match. The player moves are recorded into two separate arrays which represent the moves of the 


TL:DR
----------
- UI calls a bunch of methods in Game Manager
- Game Manager calls a bunch of different stuff from everywhere
- BoardState stores the state of the game.
- When you click on an empty tile, it'll spawn a tile and record a value into BoardState to mark it as clicked. The visual and game logic have NOTHING to do with each other so beware of errors here
- GameDataRecorder is like the CIA and reads everything
- DebugWindow is a pseudo-Game Manager that calls functions from everywhere





All Music and Sound Assets are downloaded with open licenses and can probably be found by googling open game music/sounds. 
/*************/

