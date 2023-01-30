using UnityEngine;
using UnityEngine.SceneManagement;

public class BoardUI : MonoBehaviour
{
    public GameObject controller;


    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
    }

    public void noButton()
    {
        //assigned to the game object "NoButton" in the "PlayAgain" menu to load the scene "Start Menu"
        SceneManager.LoadScene("Start Menu");
    }

    public void yesButton()
    {
        //assigned to the game object "YesButton" in the "PlayAgain"
        //menu to load the scene "Offline Game"
        //which loads the current scene you are on (plays again)
        SceneManager.LoadScene("Offline Game");
    }

    public void drawButton()
    {
        Main main = controller.GetComponent<Main>();
        main.drawEnd();
    }

    public void pauseStartGame()
    {
        Main main = controller.GetComponent<Main>();
        main.canMove = !main.canMove;
    }

    public void resignButton()
    {
        Main main = controller.GetComponent<Main>();
        //assigned to the game object "YesButton" in the "ResignMenu" to call the function
        //which ends the game
        if (main.currentPlayer == "white")
        {
            main.gameEnd("WKi");
        }
        else
        {
            main.gameEnd("BKi");
        }
    }

    public void backButton()
    {
        //loads the next scene in the build manager section
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void startGame()
    {
        controller.GetComponent<Main>().startGame();
    }

    public void startAIGame()
    {
        controller.GetComponent<Main>().againstAI = true;
        controller.GetComponent<Main>().startGame();
    }

    public void startOnlineGame()
    {
        controller.GetComponent<Main>().isOnlineGame = true;
        controller.GetComponent<Main>().startGame();
    }

    public void getQueen()
    {
        //minimise the menu
        GameObject.FindGameObjectWithTag("PromoteMenu").transform.localScale = new Vector3(0,0,0);
        setPiece("Q");
    }

    public void getRook()
    {
        //minimise the menu
        GameObject.FindGameObjectWithTag("PromoteMenu").transform.localScale = new Vector3(0, 0, 0);
        setPiece("R");
    }

    public void getBishop()
    {
        //minimise the menu
        GameObject.FindGameObjectWithTag("PromoteMenu").transform.localScale = new Vector3(0, 0, 0);
        setPiece("B");
    }

    public void getKnight()
    {
        //minimise the menu
        GameObject.FindGameObjectWithTag("PromoteMenu").transform.localScale = new Vector3(0, 0, 0);
        setPiece("Kn");
    }

    public void setPiece(string pieceName)
    {   
        int xpos = 0;
        int ypos = 0;
        //used to check if a pawn can be promoted
        Main board = controller.GetComponent<Main>();
        if (board.currentPlayer == "black")
        { //white selecting piece
            for (int i = 0; i < board.whitePieces.Length; i++)
            {
                if (board.whitePieces[i] != null)
                {
                    if (board.whitePieces[i].GetComponent<Pieces>().name == "WP" && board.whitePieces[i].GetComponent<Pieces>().yPos == 7)
                    { //If the piece is a pawn and is at the end of the board where it can promote
                        ypos = board.whitePieces[i].GetComponent<Pieces>().yPos;
                        xpos = board.whitePieces[i].GetComponent<Pieces>().xPos;
                        Destroy(board.whitePieces[i]); //destroy the pawn
                        board.setPiece(xpos, ypos, "W", i, pieceName, true); //create a new piece at the new xPos and yPos
                    }
                }
            }
        }
        else
        { //black selecting piece
            for (int i = 0; i < board.blackPieces.Length; i++)
            {
                if (board.blackPieces[i] != null)
                {
                    if (board.blackPieces[i].GetComponent<Pieces>().name == "BP" && board.blackPieces[i].GetComponent<Pieces>().yPos == 0)
                    { //If the piece is a pawn and is at the end of the board where it can promote
                        ypos = board.blackPieces[i].GetComponent<Pieces>().yPos;
                        xpos = board.blackPieces[i].GetComponent<Pieces>().xPos;
                        Destroy(board.blackPieces[i]); //destroy the pawn
                        board.setPiece(xpos, ypos, "B", i, pieceName, true); //create a new piece at the new xPos and yPos
                    }
                }
            }
        }
    }


}
