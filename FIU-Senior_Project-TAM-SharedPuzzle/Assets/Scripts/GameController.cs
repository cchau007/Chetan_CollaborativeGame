using UnityEngine;
using System.Collections;
using HoloToolkit.Sharing; //for custom messages
using UnityEngine.Windows.Speech;// for voice commands
using System.Collections.Generic;// for list
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    //variables
    
    /// <summary>
    /// Used to change UI text
    /// </summary>
    public TextEdit tScript;

    /// <summary>
    /// Used to access the timer
    /// </summary>
    private Timer tiScript;

    /// <summary>
    /// Used as settings for the timer
    /// </summary>
    public float waitTime, setupTime, roundTime;

    /// <summary>
    /// Used as flags
    /// </summary>
    public bool isGoal = false, isDiffSetup = false, isResetAllowed = false;

    /// <summary>
    /// Difficulty setting for game
    /// </summary>
    public int difficulty = 0;

    /// <summary>
    /// Used to toggle the mazes based on turns and difficulty
    /// </summary>
    public GameObject Maze1, Maze2, Maze2Plane;

    /// <summary>
    /// Used to track rounds
    /// </summary>
    public int roundCount, maxRounds;

    /// <summary>
    /// We use a this to make decisions based on our userID
    /// </summary>
    public long localUserId;

    /// <summary>
    /// Track who the current player is
    /// </summary>
    public long currentPlayer = 0;

    /// <summary>
    /// Used to enable and disable the ball
    /// </summary>
    public GameObject ball;

    /// <summary>
    /// We use a voice command to enable resetting the game;
    /// </summary>
    KeywordRecognizer keywordRecognizer;


    /// <summary>
    /// Used to reference the menu buttons. 
    /// </summary>
    public GameObject chooseDifficultyButton, easyButton, mediumButton, 
           difficultButton, difficultySelectionCanvas;


    //methods


    /// <summary>
    ///Use this for initialization
    /// </summary>
    void Start () {
        startMenu();
        //setupDifficulty(); //while testing


        // Setup a keyword recognizer to enable resetting the game.
        List<string> keywords = new List<string>();
        keywords.Add("Restart");
      //  keywords.Add("Easy Mode");
      //  keywords.Add("Medium Mode");
      //  keywords.Add("Hard Mode");
        keywordRecognizer = new KeywordRecognizer(keywords.ToArray());
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();

        //multiplayer
        //setup our message handlers
        //then setup methods to handle getting those messages
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.Difficulty] = this.setDifficulty;
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.GoalState] = this.OnGoalStateRecieved;
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.ResetGame] = this.OnReset;

        tScript = GameObject.Find("UIManager").GetComponent<TextEdit>();
        tiScript = GetComponent<Timer>();


        tScript.ChangeText("Setting up Anchor");
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update() {

        if (((int)AppStateManager.Instance.CurrentAppState) == 6)
        {
            StartCoroutine(EndGame());
        }
           
    }

    /// <summary>
    /// Getter for isDiffSetup
    /// </summary>
    /// <returns>A Boolean that is the value of isDiffSetup</returns>
    public bool getisDiffSetup()
    {
        return isDiffSetup;
    }

    /// <summary>
    /// When we want to begin the rounds of play we call this.
    /// </summary>
    /// <returns>An enumerator used to delay in Coroutines</returns>
    public IEnumerator StartRound()
    {
        if (!isDiffSetup)//difficulty was not setup so we need to make our own
        {//use default setup in editor
            Debug.Log("No difficulty recieved. Setting default.");
            isDiffSetup = true;
            setupDifficulty();
        }

        //get our local user id and store it
        //we do this here since we are assured the IDs have been set
        localUserId = CustomMessages.Instance.localUserID;
        //Debug.Log(CustomMessages.Instance.localUserID);

        string msg = SwitchPlayers();

        yield return new WaitForSeconds(setupTime);
        tiScript.timeLeft = roundTime;


        tScript.ChangeText(msg);
        yield return new WaitForSeconds(waitTime);

        if(currentPlayer == localUserId)
        {
            enableMaze();
        }else
        {
            disableMaze();
        }

        tiScript.isTimerStart = true;
    }

    /// <summary>
    /// When we need to switch players after a round we call this.
    /// </summary>
    /// <returns>A string used to inform the player about whose turn it is.</returns>
    private string SwitchPlayers()
    {

        //see which player goes now
        if (currentPlayer == 0)//we just started
        {
            if (LocalUserHasLowestUserId())//are we player 1
            {
                currentPlayer = localUserId; //we go first
                return "Your turn, get ready!";

            }
            else
            {
                //disableMaze();


                foreach (long userid in SharingSessionTracker.Instance.UserIds)
                {
                    if (userid < localUserId)
                    {
                        currentPlayer = userid;
                    }
                }


                return "Other player's turn!";
            }
        }
        else //we need to switch players
        {
            if (currentPlayer == localUserId) //we are current player
            {
                foreach (long userid in SharingSessionTracker.Instance.UserIds)
                {
                    if (userid != localUserId)
                    {
                        currentPlayer = userid;
                    }
                }
                //disableMaze();
                return "Other player's turn!";
            }
            else //we need to become the current player
            {
                currentPlayer = localUserId;
                return "Your turn, get ready!";
            }

        }

    }

    /// <summary>
    /// Called we want a player to be able to control a maze on their turn.
    /// </summary>
    private void enableMaze()
    {

        GameObject.Find("Main Camera").GetComponent<Player>().isMoveAllowed = true;//allow the player to move the maze on the other hololens

        //dont let them see their maze
        ball.GetComponent<MeshRenderer>().enabled = false;
        Maze1.GetComponent<MeshRenderer>().enabled = false;
        Maze2Plane.GetComponent<MeshRenderer>().enabled = false;
        StartCoroutine(stopBall()); //wait to disable the ball since the maze lerps
    }

    /// <summary>
    /// Used to delay stopping the ball so it does not clip through the maze.
    /// </summary>
    /// <returns>An enumerator used to delay in Coroutines</returns>
    private IEnumerator stopBall()
    {
        yield return new WaitForSeconds(2);
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        ball.GetComponent<Rigidbody>().isKinematic = true;
    }

    /// <summary>
    /// Called we want a player to not control a maze on another players turn.
    /// </summary>
    private void disableMaze()
    {
        
        GameObject.Find("Main Camera").GetComponent<Player>().isMoveAllowed = false;// dont allow player to manipulate the maze on the other hololens

        //allow them to see the maze the other player is controling
        ball.GetComponent<MeshRenderer>().enabled = true;
        ball.GetComponent<Rigidbody>().isKinematic = false;


        if (difficulty == 2 || difficulty == 3)
            Maze2Plane.GetComponent<MeshRenderer>().enabled = true;
        else
            Maze1.GetComponent<MeshRenderer>().enabled = true;
       
    }

    /// <summary>
    /// Used to decrement the round number
    /// </summary>
    /// <returns>bool: True if there are still rounds left; False otherwise.</returns>
    public bool NextRound()
    {
        roundCount -= 1;
        if (roundCount > 0)
            return true;
        else
            return false;
    }

    /// <summary>
    /// Called when the game is over, determines if the game was won or lost. Prepares the game to be reset.
    /// </summary>
    /// <returns>An enumerator used to delay in Coroutines</returns>
    private IEnumerator EndGame()
    {
        //end the game
        if (isGoal)
        {
            //UpdateText();
            tScript.ChangeText("Game Over!");
            yield return new WaitForSeconds(setupTime);
            CustomMessages.Instance.SendGoalState(1);
            tScript.ChangeText("You Win!");
            AppStateManager.Instance.CurrentAppState = AppStateManager.AppState.GameOver;
            tiScript.isTimerStart = false;

        }
        else
        {
            //UpdateText();
            tScript.ChangeText("Game Over!");
            yield return new WaitForSeconds(setupTime);
            CustomMessages.Instance.SendGoalState(0);
            tScript.ChangeText("You Lose!");
            AppStateManager.Instance.CurrentAppState = AppStateManager.AppState.GameOver;
            tiScript.isTimerStart = false;
        }

        GameObject.Find("Main Camera").GetComponent<Player>().isMoveAllowed = false;
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        ball.GetComponent<Rigidbody>().isKinematic = true;

        isResetAllowed = true;
    }

    /// <summary>
    /// Used find out if the local user has the lowest userID number
    /// </summary>
    /// <returns>bool: True if local user has lowest ID; False otherwise.</returns>
    bool LocalUserHasLowestUserId()
    {
        long localUserId = CustomMessages.Instance.localUserID;
        foreach (long userid in SharingSessionTracker.Instance.UserIds)
        {
            if (userid < localUserId)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Used to configure the difficulty settings of the game.
    /// </summary>
    public void setupDifficulty()
    {
        difficultySelectionCanvas.SetActive(false); //to not show the buttons once a button is clicked.
        chooseDifficultyButton.SetActive(false);// To hide the choose difficulty button.
        //setup a maze based on the game settings
        if (difficulty == 0) //testing mode
        {
            GameObject.Find("Maze2").SetActive(false);
            roundCount = 3;
            maxRounds = 3;
            roundTime = 5; //change this to make rounds longer
        }
        else if (difficulty == 1)//easy
        {
            GameObject.Find("Maze2").SetActive(false);
            roundCount = 4;
            maxRounds = 4;
            roundTime = 40; //change this to make rounds longer
        }
        else if (difficulty == 2)//medium
        {
            GameObject.Find("Maze").SetActive(false);
            roundCount = 4;
            maxRounds = 4;
            roundTime = 60; //change this to make rounds longer
        }
        else if (difficulty == 3)//expert
        {
            GameObject.Find("Maze").SetActive(false);
            roundCount = 4;
            maxRounds = 4;
            roundTime = 30; //change this to make rounds longer
        }

        isDiffSetup = true;
        return;
    }

    /// <summary>
    /// Set our difficulty based on the settings of another player
    /// </summary>
    /// <param name="msg">The difficulty value sent from another player in the form of an Int</param>
    void setDifficulty(NetworkInMessage msg)
    {
        // We read the user ID but we don't use it here.
        msg.ReadInt64();
        difficulty = CustomMessages.Instance.ReadInt(msg);
        setupDifficulty();
        isDiffSetup = true;
    }

    /// <summary>
    /// Called when the game has ended, we must update our game to the same goal state. Used to track early wins.
    /// </summary>
    /// <param name="msg">The goal state of the other player</param>
    void OnGoalStateRecieved(NetworkInMessage msg)
    {
        int goal;
        // We read the user ID but we don't use it here.
        msg.ReadInt64();
        goal = CustomMessages.Instance.ReadInt(msg);
        if(goal == 1)
        {
            isGoal = true;
            AppStateManager.Instance.CurrentAppState = AppStateManager.AppState.EndGame;
        }
    }

    /// <summary>
    /// Called when a player sent us a request to reset the game.
    /// </summary>
    /// <param name="msg">The userId of the player who sent the msg</param>
    void OnReset(NetworkInMessage msg)
    {
        // We read the user ID but we don't use it here.
        msg.ReadInt64();
        ResetGame();
    }

    /// <summary>
    /// When the keyword recognizer hears a command this will be called.  
    /// This allows the game to be reset, and tells the other player to reset.
    /// </summary>
    /// <param name="args">information to help route the voice command.</param>
    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        ResetGame();
        CustomMessages.Instance.SendResetGame();//tells the other hololens to reset the level
    }


    /// <summary>
    /// Resets the game and tells the other player to do so as well.
    /// </summary>
    private void ResetGame()
    {
        if (isResetAllowed)
        {

            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);//resets the level


            //timer should already be stopped by round ending
            //set state to before rounds started
            //changing to this state will allow the user to repick the maze
            //incorporate selecting difficulty
            isGoal = false;
            isDiffSetup = false;
            isResetAllowed = false;
            currentPlayer = 0;

            AppStateManager.Instance.CurrentAppState = AppStateManager.AppState.WaitingForStageTransform;
            ball.GetComponent<RollBall>().ResetBall();
            startMenu();
        }
    }


    /// <summary>
    /// Used to position the main menu.
    /// </summary>
    void positionMenu()
    {
        chooseDifficultyButton.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2;
    }

    /// <summary>
    /// Used to position the difficulty buttons.
    /// </summary>
    void positionButton()
    {
        easyButton.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2;
        easyButton.transform.position = new Vector3(easyButton.transform.position.x, easyButton.transform.position.y + 0.2f, easyButton.transform.position.z);
        mediumButton.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2;
        difficultButton.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2;
        difficultButton.transform.position = new Vector3(difficultButton.transform.position.x, difficultButton.transform.position.y - 0.2f, difficultButton.transform.position.z);
    }

    /// <summary>
    /// Called when we want to show the main menu.
    /// </summary>
    public void startMenu()
    {
        positionMenu();
        positionButton();
        chooseDifficultyButton.SetActive(true); // To display the choose difficulty button.
        easyButton.SetActive(false); //to hide the easy button.
        mediumButton.SetActive(false); //to hide the medium button.
        difficultButton.SetActive(false); //to hide the difficult button.
    }

    /// <summary>
    /// Called when the player needs to choose a difficulty.
    /// </summary>
    public void difficultySelection()
    {
        chooseDifficultyButton.SetActive(false);// To hide the choose difficulty button.
        easyButton.SetActive(true);//to show the easy button.
        mediumButton.SetActive(true); //to show the medium button.
        difficultButton.SetActive(true); //to show the difficult button.
    }

    /// <summary>
    /// Called when the easy button is pressed.
    /// </summary>
    public void easyButtonPressed() 
    {
        difficulty = 1; // Setting the value to 0 for easy maze.
        setupDifficulty();
        CustomMessages.Instance.SendDifficulty(1);
    }

    /// <summary>
    /// Called when the medium button is pressed.
    /// </summary>
    public void mediumButtonPressed() 
    {
        difficulty = 2; // Setting the value to 1 for medium maze.    
        setupDifficulty();
        CustomMessages.Instance.SendDifficulty(2);
    }

    /// <summary>
    /// Called when the hard button is pressed.
    /// </summary>
    public void difficultButtonPressed()
    {
        difficulty = 3; // Setting the value to 2 for difficult maze.
        setupDifficulty();
        CustomMessages.Instance.SendDifficulty(3);
    }

}
