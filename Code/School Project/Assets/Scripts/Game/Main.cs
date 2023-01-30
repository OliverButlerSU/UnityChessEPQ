using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.IO; //read write files
using System; //Date Time

//This class stores the information of the board (where pieces are) aswell as is the main class which instantiates all
//the pieces. It also has functions for ending the game aswell and validation of data.

public class Main : MonoBehaviour
{
    public GameObject ChessPieces; //used to reference ChessPiece prefab

    //Creating GameObject arrays
    //stores the locations of the gameobjects on an 8x8 board on a 2D array (xPos,yPos)
    public GameObject[,] boardPositions = new GameObject[8, 8];
    //used to store the current instatiated pieces
    public GameObject[] whitePieces = new GameObject[16];
    public GameObject[] blackPieces = new GameObject[16];
    //used to store the points gained from each 
    public int blackPoints;
    public int whitePoints;
    //stores current amount of pieces on board
    public int blackPiecesLength = 16;
    public int whitePiecesLength = 16;
    //used to store the current player and if the game is over and if playing against AI
    public string currentPlayer = "white";
    public bool canMove = true;

    public bool againstAI = false;
    public bool isOnlineGame = false;

    //stores attack positions of each colour
    public bool[,] whiteAttackBoard = new bool[8, 8];
    public bool[,] blackAttackBoard = new bool[8, 8];

    //stores values for end game:
    public bool gameOver = false;

    //used to count for 50 move rule
    public float moves = 0.0f;

    //used to store moves
    public List<string> moveHistory = new List<string>();

    //used for threefold repition
    public int[,] threeFoldHistory = new int[101,64];
    public int historyLength = 0;

    public void startGame()
    {
        //Makes all UI not needed invisible
        GameObject.FindGameObjectWithTag("PlayAgainUI").transform.localScale = new Vector3(0, 0, 0);
        GameObject.FindGameObjectWithTag("PromoteMenu").transform.localScale = new Vector3(0, 0, 0);
        //this assigns each piece and places them into the associated array
        whitePieces = new GameObject[] {
            createPiece("WR",0,0), createPiece("WKn",1,0), createPiece("WB",2,0), createPiece("WQ",3,0), createPiece("WKi",4,0),
            createPiece("WB",5,0), createPiece("WKn",6,0), createPiece("WR",7,0),
            createPiece("WP",0,1), createPiece("WP",1,1), createPiece("WP",2,1), createPiece("WP",3,1), createPiece("WP",4,1),
            createPiece("WP",5,1), createPiece("WP",6,1), createPiece("WP",7,1) };
        blackPieces = new GameObject[] {
            createPiece("BR", 0, 7), createPiece("BKn", 1, 7), createPiece("BB", 2, 7), createPiece("BQ", 3, 7), createPiece("BKi", 4, 7),
            createPiece("BB", 5, 7), createPiece("BKn", 6, 7), createPiece("BR", 7, 7),
            createPiece("BP", 0, 6), createPiece("BP", 1, 6), createPiece("BP", 2, 6), createPiece("BP", 3, 6), createPiece("BP", 4, 6),
            createPiece("BP", 5, 6), createPiece("BP", 6, 6), createPiece("BP", 7, 6) };

        for (int currentPiece = 0; currentPiece < 16; currentPiece++)
        {
            assignBoardPos(whitePieces[currentPiece]);
            assignBoardPos(blackPieces[currentPiece]);
        }

        //gets the attack positions for each side
        setupAttackBoard();

        //Starts white timer
        GameObject whiteTimer = GameObject.FindGameObjectWithTag("WhiteTimer");
        whiteTimer.GetComponent<Timer>().cyclePause();

        if (!againstAI)
        {
            //If you are not playing against an AI, the AI button is not needed
            GameObject.FindGameObjectWithTag("AIButton").transform.localScale = new Vector3(0, 0, 0);
        }
        convertBoardToInt();
    }

    public GameObject createPiece(string name, int xPos, int yPos)
    {   //Creates a piece, assigning the name, xpos and ypos to it
        GameObject currentGO = Instantiate(ChessPieces, new Vector3(0, 0, -1), Quaternion.identity);
        Pieces cp = currentGO.GetComponent<Pieces>();
        cp.name = name;
        cp.xPos = xPos;
        cp.yPos = yPos;
        cp.assignPiece();
        return currentGO;
    }
    public void assignBoardPos(GameObject currentPiece)
    {
        //gets xpos and ypos of the piece
        Pieces cp = currentPiece.GetComponent<Pieces>();
        boardPositions[cp.xPos, cp.yPos] = currentPiece;
    }

    public bool validBoardPos(int x, int y)
    {
        //used to see if the positions which are used are valid in the board (must be between 0-7)
        if (x < 0 || y < 0 || x > 7 || y > 7)
        {
            return false;
        }
        return true;
    }

    public void setPiece(int x, int y, string colour, int indexVal, string pieceName, bool isSelecting)
    {
        //used in promoting the pawn once it reaches the other side, it creates a queen at the given position

        if (colour == "B" && againstAI)
        { //if it is the AI, it will automatically choose queen
            Destroy(blackPieces[indexVal]);
            blackPieces[indexVal] = createPiece("BQ", x, y);
            assignBoardPos(blackPieces[indexVal]);
        }
        else
        {
            if (!isSelecting)
            { //makes it so you cannot move until you choose a new piece
                canMove = false;
                //maximise menu
                GameObject.FindGameObjectWithTag("PromoteMenu").transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                string piece = "";
                piece += colour;
                piece += pieceName;
                if (colour == "B")
                { //selecting black piece and assigns to board
                    blackPieces[indexVal] = createPiece(piece, x, y);
                    assignBoardPos(blackPieces[indexVal]);
                }
                else
                { //selecting white piece and assigns to board
                    whitePieces[indexVal] = createPiece(piece, x, y);
                    assignBoardPos(whitePieces[indexVal]);
                }
                //allows user to move
                canMove = true;
            }
        }
        setupAttackBoard();
    }

    public void changeCurrentPlayer()
    {
        //used to change the current player
        switch (currentPlayer)
        {
            case ("white"):
                currentPlayer = "black";
                break;
            case ("black"):
                currentPlayer = "white";
                break;
        }
        //rotates timers
        GameObject whiteTimer = GameObject.FindGameObjectWithTag("WhiteTimer");
        whiteTimer.GetComponent<Timer>().cyclePause();
        GameObject blackTimer = GameObject.FindGameObjectWithTag("BlackTimer");
        blackTimer.GetComponent<Timer>().cyclePause();
    }

    public void gameEnd(string takePiece)
    {
        stopTimer();
        //called once the game ends and assigns values for the winner text
        gameOver = true;
        int winnerPoints = 0;
        string Winner = "white";
        int loserPoints = 0;
        string Loser = "black";
        switch (takePiece)
        {
            case ("WKi"): //black wins
            case ("white"):
                Winner = "black";
                Loser = "white";
                winnerPoints = blackPoints;
                loserPoints = whitePoints;
                moveHistory.Add("0-1");
                break;
            case ("BKi"): //white wins
            case ("black"):
                moveHistory.Add("1-0");
                winnerPoints = whitePoints;
                loserPoints = blackPoints;
                break;
        }
        writeHistoryToFile();
        //Finds certain gameobjects and changes the properties of them either disabling them, making them
        //visible or changing the text
        GameObject.FindGameObjectWithTag("ResignButton").SetActive(false);
        GameObject.FindGameObjectWithTag("DrawButton").SetActive(false);
        GameObject.FindGameObjectWithTag("PlayAgainUI").transform.localScale = new Vector3(1, 1, 1);
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text = 
            "The winner is " + Winner + " with a collection of " + winnerPoints.ToString() + " points. Compared to " + Loser + 
            " with a collection of " + loserPoints.ToString() + " points. Would you like to play again?";
    }

    public void drawEnd()
    {
        moveHistory.Add("½-½");
        stopTimer();
        writeHistoryToFile();
        //Used when there is a draw. Finds different game objects and assigns properties similar in
        //gameEnd() function
        GameObject.FindGameObjectWithTag("ResignButton").SetActive(false);
        GameObject.FindGameObjectWithTag("DrawButton").SetActive(false);
        GameObject.FindGameObjectWithTag("PlayAgainUI").transform.localScale = new Vector3(1, 1, 1);
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text =
            "It is a draw. White obtained " + whitePoints.ToString() + " points. Compared to Black obtaining " + blackPoints.ToString() + " points. Would you like to play again?";
    }

    public void stopTimer()
    {
        //stops both timers
        GameObject.FindGameObjectWithTag("WhiteTimer").GetComponent<Timer>().stopTimer();
        GameObject.FindGameObjectWithTag("BlackTimer").GetComponent<Timer>().stopTimer();
        GameObject.FindGameObjectWithTag("TimerUI").SetActive(false);
    }

    public void checkDraw()
    {
        //used to check if there is a draw due to insufficient material
        if (whitePiecesLength == 1 && blackPiecesLength == 1)//king vs king
        {
            drawEnd();
        } //everything below includes king
        else if ((whitePiecesLength == 2 && blackPiecesLength == 1) || (whitePiecesLength == 1 && blackPiecesLength == 2))
            //if one player has one piece and the other has two (this is to optimise the amount of checks)
        {
            if (((whitePieces[2] != null || whitePieces[5] != null) && blackPiecesLength == 1) || ((blackPieces[2] != null || blackPieces[5] != null) && whitePiecesLength == 1)) 
                //knight vs king
            {
                drawEnd();
            } else if (((whitePieces[1] != null || whitePieces[6] != null) && blackPiecesLength == 1) || ((blackPieces[1] != null || blackPieces[6] != null) && whitePiecesLength == 1)) 
                //bishop vs king
            {
                drawEnd();
            }
        } else if (whitePiecesLength == 2 && blackPiecesLength == 2)
        { //boths players have two pieces
            if ((whitePieces[0] != null || whitePieces[7] != null) && (blackPieces[0] != null || blackPieces[7] != null)) 
                // rook vs rook
            {
                drawEnd();
            }
            else if ((whitePieces[2] != null && blackPieces[5] != null) || (whitePieces[5] != null && blackPieces[2] != null)) 
                // bishop vs bishop (both being on the same square colour)
            {
                drawEnd();
            }
            else if ((whitePieces[0] != null || whitePieces[7] != null) && (blackPieces[2] != null || blackPieces[1] != null || blackPieces[5] != null || blackPieces[6] != null)) 
                //rook vs bishop
            {
                drawEnd();
            }
            else if ((blackPieces[0] != null || blackPieces[7] != null) && (whitePieces[2] != null || whitePieces[1] != null || whitePieces[5] != null || whitePieces[6] != null))  
                // rook vs knight
            {
                drawEnd();
            }
        } else if ((whitePiecesLength == 3 && blackPiecesLength ==1) || (blackPiecesLength == 3 && whitePiecesLength == 1))
        { //three pieces vs one
            if ((whitePieces[1] != null && whitePieces != null && blackPiecesLength == 1) || (blackPieces[1] != null && blackPieces[6] != null && whitePiecesLength == 1)) 
                //two knights vs king
            {
                drawEnd();
            }

        }
    }

    public void setupAttackBoard()
    {
        //Used to create the attack board which stores the positions of each sides attacking positions
        for (int xBoard = 0; xBoard < 8; xBoard++)
        {
            for (int yBoard = 0; yBoard < 8; yBoard++)
            { //resets the board by setting each position to false
                whiteAttackBoard[xBoard, yBoard] = false;
                blackAttackBoard[xBoard, yBoard] = false;
            }
        }

        for (int currentPos = 0; currentPos < 16; currentPos++)
        {
            if (whitePieces[currentPos] != null) //null error stop
            { //gets all possible attacking positions of white side
                whitePieces[currentPos].GetComponent<Pieces>().possibleMoves(false);
            }
            if (blackPieces[currentPos] != null)//null error stop
            { //gets all possible attacking positions of black side
                blackPieces[currentPos].GetComponent<Pieces>().possibleMoves(false);
            }
            
        }

        inCheck();
    }

    public bool inCheck()
    {
        //Returns if in check or not, setting the king in check if it is
        if (currentPlayer == "white") //if the current player is white
        {
            if (blackAttackBoard[whitePieces[4].GetComponent<Pieces>().xPos, whitePieces[4].GetComponent<Pieces>().yPos])
            { //if king is being attacked, set check to true
                whitePieces[4].GetComponent<Pieces>().inCheck = true;
                return true;
            }
            else
            {
                whitePieces[4].GetComponent<Pieces>().inCheck = false;
            }
        }
        else
        {//if the current player is black
            if (whiteAttackBoard[blackPieces[4].GetComponent<Pieces>().xPos, blackPieces[4].GetComponent<Pieces>().yPos])
            {//if king is being attacked, set check to true
                blackPieces[4].GetComponent<Pieces>().inCheck = true;
                return true;
            }
            else
            {
                blackPieces[4].GetComponent<Pieces>().inCheck = false;
            }
        }
        return false;
    }

    public void destroyTiles()
    { //finds all move plates that are tagged "MovePlate"
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for (int i = 0; i < movePlates.Length; i++)
        { //destroys each one
            Destroy(movePlates[i]);
        }
    }


    public void writeHistoryToFile()
    {
        //This will write the list "moveHistory" to a file
        DateTime now = DateTime.Now; //gets current time
        string history = "\n\nGame at: "+ now.ToString("F") + "\nMove history:"; 
        for(int i = 0; i < moveHistory.Count(); i++)
        { //gets all moves and appends them to a string format
            history += " " + moveHistory[i];
        }
        string path = Environment.CurrentDirectory + "\\history.txt";
        if (!File.Exists(path)) //if the file does not exist
        {
            // Create a file to write to.
            string createText = Environment.NewLine;
            File.WriteAllText(path, createText);
        } //Writes the move history to file
        File.AppendAllText(path, history);
    }

    public bool convertBoardToInt()
    { //converts the current board positions to an integer format
        int[] intBoard = new int[64];
        for(int i =0; i < whitePieces.Length; i++)
        { //to distinguish from each piece, the array position is used, and if it is black, it is +17 to distinguish sides
            if(whitePieces[i] != null)
            {
                intBoard[whitePieces[i].GetComponent<Pieces>().xPos + whitePieces[i].GetComponent<Pieces>().yPos *8] = i + 1;
            } if (blackPieces[i] != null)
            {
                intBoard[blackPieces[i].GetComponent<Pieces>().xPos + blackPieces[i].GetComponent<Pieces>().yPos * 8] = i + 17;
            }
        }
        return checkRepition(intBoard);
    }

    public bool checkRepition(int[] boardPositions)
    {//used for threefold repition
        int checkSum = 0;
        for (int currentCheck = 0; currentCheck < historyLength; currentCheck++)
        { //iterates through all stored moves
            int checkCurrent = 0;
            for(int valueCheck = 0; valueCheck < boardPositions.Length; valueCheck++)
            {
                if (threeFoldHistory[currentCheck,valueCheck] == boardPositions[valueCheck])
                { //if a square is equal to the same square on a seperate board
                    checkCurrent++;
                }
            }
            if (checkCurrent == 64)
            { //if all 64 spots are the same
                checkSum++;
                if (checkSum == 2)
                { //if the positon is repeated three times, end the game
                    return true;
                }
            }
        } 
        for (int i = 0; i < 64; i++)
        {//adds the move to history
            threeFoldHistory[historyLength, i] = boardPositions[i];
        }
        historyLength++;
        return false;
    }


}