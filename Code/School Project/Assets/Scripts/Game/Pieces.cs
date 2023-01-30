using System;
using UnityEngine;

//This class stores the information of each piece which is instatiated aswell as the possible moves of each piece

public class Pieces : MonoBehaviour
{

    // References
    public GameObject controller; //later assigned in "assignPiece()"
    public GameObject moveTiles; //creates a tile to allow pieces to move to (associated with MoveTiles class)
    //Creating the references for sprites, each assigned to a piece (used in assignPiece())
    public Sprite BB, BKi, BKn, BP, BQ, BR, WB, WKi, WKn, WP, WQ, WR;

    //Creating Board positions (x and y), both assigned -1 as they are not on the board yet. They range between (0-7) created an
    //8x8 board in total. They are assigned later on in the "MainGame" class.
    public int xPos = -1;
    public int yPos = -1;

    //The colour of the piece
    public string playerColour;
    //Private as I do not want code to be able to change the pieceWorth of pieces after they are declared
    private int pieceWorth = 0;
    //If the pawn has moved or not (while it is quite unneccessary for it to be a variable stored in non-pawn pieces, this makes it much easier
    //and does not affect any other pieces anyways
    public bool hasMoved = false;

    public bool inCheck = false;

    //Used to allow pawns to en passant
    public bool enPassantAttack = false;
    public int enPassantXPos = -1;
    public int enPassantYPos = -1;

    public void assignPiece()
    {
        //the GameObject "ChessPiecesController" is assigned a tag "GameController", this makes it
        //so you do find this object with functions
        controller = GameObject.FindGameObjectWithTag("GameController");
        //when an instance of a class is created, the name is parsed and with this can be used with a switch case to assing the
        //corresponding sprite value (these are located in the file "Sprites". This referes to this instance of the class and the
        //name is the name parsed in the MainGame class
        switch (name)
        {
            //Gets the component "SpriteRender" and has a variable called sprite which is used to assign the sprite value; we
            //then replace the sprite to the corresponding sprite value found in folder "Sprites". We also assign the
            //values for each piece (King is 0 as it technically has an infinite worth as once you get it you win anyway)
            //aswell as the colour of the piece
            case "BB":
                playerColour = "B";
                pieceWorth = 3;
                GetComponent<SpriteRenderer>().sprite = BB;
                break;
            case "BKi":
                playerColour = "B";
                pieceWorth = 90;
                GetComponent<SpriteRenderer>().sprite = BKi;
                break;
            case "BKn":
                playerColour = "B";
                pieceWorth = 3;
                GetComponent<SpriteRenderer>().sprite = BKn;
                break;
            case "BP":
                playerColour = "B";
                pieceWorth = 1;
                GetComponent<SpriteRenderer>().sprite = BP;
                break;
            case "BQ":
                playerColour = "B";
                pieceWorth = 9;
                GetComponent<SpriteRenderer>().sprite = BQ;
                break;
            case "BR":
                playerColour = "B";
                pieceWorth = 5;
                GetComponent<SpriteRenderer>().sprite = BR;
                break;
            case "WB":
                playerColour = "W";
                pieceWorth = 3;
                GetComponent<SpriteRenderer>().sprite = WB;
                break;
            case "WKi":
                playerColour = "W";
                pieceWorth = 90;
                GetComponent<SpriteRenderer>().sprite = WKi;
                break;
            case "WKn":
                playerColour = "W";
                pieceWorth = 3;
                GetComponent<SpriteRenderer>().sprite = WKn;
                break;
            case "WP":
                playerColour = "W";
                pieceWorth = 1;
                GetComponent<SpriteRenderer>().sprite = WP;
                break;
            case "WQ":
                playerColour = "W";
                pieceWorth = 9;
                GetComponent<SpriteRenderer>().sprite = WQ;
                break; 
            case "WR":
                playerColour = "W";
                pieceWorth = 5;
                GetComponent<SpriteRenderer>().sprite = WR;
                break;
        }
        setCoordPos();
    }

    public void setCoordPos()
    //This function transforms the position of the GameObjects, where we must translate the positions which
    //are on the board (0-7) to co-ordinates on the screen so they are placed correctly. 
    {
        transform.position = new Vector3((xPos * 0.66f) - 2.3f, (yPos * 0.66f) - 2.3f);
    }

    public int getPieceWorth()
    {
        return pieceWorth;
    }
    public void possibleMoves(bool isPlace)
    {
        //isPlace true => moving piece
        //isPlace false => used to update attack positions
        //Gets the possible moves for a piece
        switch (name)
        {
            case "WB":
            case "BB":
                bishopMove(isPlace);
                break;
            case "WKi":
            case "BKi":
                kingMove(isPlace);
                break;
            case "WKn":
            case "BKn":
                knightMove(isPlace);
                break;
            case "WP":
                if (!hasMoved)
                {
                    pawnNotMove(0, 2, isPlace);
                }
                pawnMove(xPos, yPos + 1, isPlace);
                break;
            case "BP":
                if (!hasMoved)
                {
                    pawnNotMove(0, -2, isPlace);
                }
                pawnMove(xPos, yPos - 1, isPlace);
                break;
            case "WQ":
            case "BQ":
                queenMove(isPlace);
                break;
            case "WR":
            case "BR":
                rookMove(isPlace);
                break;
        }
    }

    public void bishopMove(bool isPlace)
    {
        straightLineMove(1, 1, isPlace);
        straightLineMove(-1, -1, isPlace);
        straightLineMove(-1, 1, isPlace);
        straightLineMove(1, -1, isPlace);
    }

    public void knightMove(bool isPlace)
    {
        inputMove(xPos + 2, yPos + 1, isPlace);
        inputMove(xPos + 2, yPos - 1, isPlace);
        inputMove(xPos - 2, yPos + 1, isPlace);
        inputMove(xPos - 2, yPos - 1, isPlace);
        inputMove(xPos + 1, yPos + 2, isPlace);
        inputMove(xPos - 1, yPos + 2, isPlace);
        inputMove(xPos + 1, yPos - 2, isPlace);
        inputMove(xPos - 1, yPos - 2, isPlace);

    }

    public void kingMove(bool isPlace)
    {
        inputMove(xPos + 1, yPos + 1, isPlace);
        inputMove(xPos, yPos + 1, isPlace);
        inputMove(xPos + 1, yPos - 1, isPlace);
        inputMove(xPos + 1, yPos, isPlace);
        inputMove(xPos - 1, yPos, isPlace);
        inputMove(xPos - 1, yPos + 1, isPlace);
        inputMove(xPos, yPos - 1, isPlace);
        inputMove(xPos - 1, yPos - 1, isPlace);

        if (!inCheck && isPlace)
        {

            Main board = controller.GetComponent<Main>();
            //check for castle
            if (playerColour == "B" && !hasMoved)
            {
                if (board.boardPositions[7, 7] != null)
                {//kingside castle
                    if (!board.boardPositions[7, 7].GetComponent<Pieces>().hasMoved && board.boardPositions[6, 7] == null && board.boardPositions[5, 7] == null 
                        && board.whiteAttackBoard[5,7] != true)
                    {
                        castle(7,7);
                    }
                }
                if (board.boardPositions[0, 7]!= null)
                {//queenside castle
                    if (!board.boardPositions[0, 7].GetComponent<Pieces>().hasMoved && board.boardPositions[1, 7] == null && board.boardPositions[2, 7] == null 
                        && board.boardPositions[3, 7] == null && board.whiteAttackBoard[3,7] != true)
                    {
                        castle(0,7);
                    }
                }
            }
            else if (playerColour == "W" && !hasMoved)
            {
                if (board.boardPositions[0, 0] != null)
                {//queenside castle
                    if (!board.boardPositions[0, 0].GetComponent<Pieces>().hasMoved && board.boardPositions[1, 0] == null && board.boardPositions[2, 0] == null
                        && board.boardPositions[3, 0] == null && board.blackAttackBoard[3, 0] != true)
                    {
                        castle(0,0);
                    }
                }
                if (board.boardPositions[7, 0] != null)
                {//kingside castle
                    if (!board.boardPositions[7, 0].GetComponent<Pieces>().hasMoved && board.boardPositions[6, 0] == null && board.boardPositions[5, 0] == null 
                        && board.blackAttackBoard[5,0] != true)
                    {
                        castle(7,0);
                    }
                }
            }
            //Seeing that it cant move anywhere (stalemate)
            GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
            if (movePlates.Length == 0 && isPlace)
            {//king have no moves
                if (playerColour == "W")
                {//iterate through all pieces, getting their moves
                    for (int currentPiece = 0; currentPiece < 16; currentPiece++)
                    {
                        if (board.whitePieces[currentPiece] != null && board.whitePieces[currentPiece].GetComponent<Pieces>().name != "WKi")
                        {
                            board.whitePieces[currentPiece].GetComponent<Pieces>().possibleMoves(true);
                        }
                    }
                    //get all the new moves
                    GameObject[] newMovePlates = GameObject.FindGameObjectsWithTag("MovePlate");
                    if (newMovePlates.Length == 0)
                    {//if there are no legal moves, its stalemate, draw the game
                        DestroyTiles();
                        board.drawEnd();
                    }
                }
                else
                {
                    for (int currentPiece = 0; currentPiece < 16; currentPiece++)
                    {
                        if (board.blackPieces[currentPiece] != null && board.blackPieces[currentPiece].GetComponent<Pieces>().name != "BKi")
                        {
                            board.blackPieces[currentPiece].GetComponent<Pieces>().possibleMoves(true);
                        }
                    }
                    GameObject[] newMovePlates = GameObject.FindGameObjectsWithTag("MovePlate");
                    if (newMovePlates.Length == 0)
                    {
                        DestroyTiles();
                        board.drawEnd();
                    }
                }
                DestroyTiles();
            }
        }

        else if (inCheck && isPlace)
        {
            //used to check for checkmate
            Main board = controller.GetComponent<Main>();

            GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");

            if (movePlates.Length == 0)
            {
                if (playerColour == "W")
                {
                    for (int currentPiece = 0; currentPiece < 16; currentPiece++)
                    {
                        if (board.whitePieces[currentPiece] != null && board.whitePieces[currentPiece].GetComponent<Pieces>().name != "WKi")
                        {
                            board.whitePieces[currentPiece].GetComponent<Pieces>().possibleMoves(true);
                        }
                    }
                    GameObject[] newMovePlates = GameObject.FindGameObjectsWithTag("MovePlate");
                    if (newMovePlates.Length == 0)
                    {
                        DestroyTiles();
                        board.gameEnd(name);
                    }
                }
                else
                {
                    for (int currentPiece = 0; currentPiece < 16; currentPiece++)
                    {
                        if (board.blackPieces[currentPiece] != null && board.blackPieces[currentPiece].GetComponent<Pieces>().name != "BKi")
                        {
                            board.blackPieces[currentPiece].GetComponent<Pieces>().possibleMoves(true);
                        }
                    }
                    GameObject[] newMovePlates = GameObject.FindGameObjectsWithTag("MovePlate");
                    if (newMovePlates.Length == 0)
                    {
                        DestroyTiles();
                        board.gameEnd(name);
                    }
                }
                DestroyTiles();
            }
        }
    } 

    public void queenMove(bool isPlace)
    {
        //Straight Lines
        straightLineMove(1, 0, isPlace);
        straightLineMove(0, 1, isPlace);
        straightLineMove(-1, 0, isPlace);
        straightLineMove(0, -1, isPlace);
        //Diagonal Lines
        straightLineMove(1, 1, isPlace);
        straightLineMove(-1, -1, isPlace);
        straightLineMove(-1, 1, isPlace);
        straightLineMove(1, -1, isPlace);
    }

    public void rookMove(bool isPlace)
    {
        //Straight Lines
        straightLineMove(1, 0, isPlace);
        straightLineMove(0, 1, isPlace);
        straightLineMove(-1, 0, isPlace);
        straightLineMove(0, -1, isPlace);
    }

    public void castle(int newXPos, int newYPos)
    {
        //used to check if the king can castle or not
        Main board = controller.GetComponent<Main>();
        if (board.boardPositions[newXPos, newYPos].GetComponent<Pieces>().name[1] == 'R' && board.boardPositions[newXPos, newYPos].GetComponent<Pieces>().playerColour == playerColour)
        {
            //used to see if the king can castle king side
            if (playerColour == "W" && board.blackAttackBoard[xPos + 2, yPos] != true && newXPos == 7)
            {
                moveCastleTile(xPos + 2, yPos, board.boardPositions[xPos + 3, yPos], -1);
            }
            else if (playerColour == "B" && board.whiteAttackBoard[xPos + 2, yPos] != true && newXPos == 7)
            {
                moveCastleTile(xPos + 2, yPos, board.boardPositions[xPos + 3, yPos], -1);
            }
            //used to see if the king can castle queen side
            if (playerColour == "W" && board.blackAttackBoard[xPos - 2, yPos] != true && newXPos == 0)
            {
                moveCastleTile(xPos - 2, yPos, board.boardPositions[xPos - 4, yPos], 2);
            }
            else if (playerColour == "B" && board.whiteAttackBoard[xPos - 2, yPos] != true && newXPos == 0)
            {
                moveCastleTile(xPos - 2, yPos, board.boardPositions[xPos - 4, yPos], 2);
            }
        }
    }

    public void pawnMove(int newXPos, int newYPos, bool isPlace)
    {
        Main board = controller.GetComponent<Main>();
        if (isPlace)
        {
            if (board.validBoardPos(newXPos, newYPos) && board.boardPositions[newXPos, newYPos] == null)
            {
                passiveMovePawn(newXPos, newYPos, isPlace);
            }
            if (board.validBoardPos(newXPos + 1, newYPos) && board.boardPositions[newXPos + 1, newYPos] != null && board.boardPositions[newXPos + 1, newYPos].GetComponent<Pieces>().playerColour != playerColour)
            {
                attackTile(newXPos + 1, newYPos, isPlace);
            }
            if (board.validBoardPos(newXPos - 1, newYPos) && board.boardPositions[newXPos - 1, newYPos] != null && board.boardPositions[newXPos - 1, newYPos].GetComponent<Pieces>().playerColour != playerColour)
            {
                attackTile(newXPos - 1, newYPos, isPlace);
            } //used for en passant
            if (enPassantAttack)
            { //if the piece can enpassant
                if (board.boardPositions[enPassantXPos,enPassantYPos] == null)
                {//create move tile
                    enPassantMovePawn(enPassantXPos, enPassantYPos);
                }
            }
        }
        else
        {
            if (board.validBoardPos(newXPos + 1, newYPos))
            {
                setAttackBoardPos(newXPos + 1, newYPos);
            }
            if (board.validBoardPos(newXPos - 1, newYPos))
            {
                setAttackBoardPos(newXPos - 1, newYPos);
            }
        }
        
    }

    public void pawnNotMove(int changeInX, int changeInY, bool isPlace)
    {
        //Used to allow a pawn to move two spaces if the piece has not moved yet
        Main board = controller.GetComponent<Main>();
        if(changeInY == 2)
        {
            if (board.validBoardPos(xPos + changeInX, yPos + changeInY) && board.boardPositions[xPos + changeInX, yPos + changeInY] == null && board.boardPositions[xPos + changeInX, yPos + changeInY - 1] == null)
            {
                passiveMovePawn(xPos + changeInX, yPos + changeInY, isPlace);
            }
        }
        else
        {
            if (board.validBoardPos(xPos + changeInX, yPos + changeInY) && board.boardPositions[xPos + changeInX, yPos + changeInY] == null && board.boardPositions[xPos + changeInX, yPos + changeInY + 1] == null)
            {
                passiveMovePawn(xPos + changeInX, yPos + changeInY, isPlace);
            }
        }

    }

    public void inputMove(int newXPos, int newYPos, bool isPlace)
    {
        //Used to check if a certain location is empty and none other (used for king and knight)
        Main board = controller.GetComponent<Main>();
        if (isPlace)
        {
            if (board.validBoardPos(newXPos, newYPos))
            {
                if ((name == "WKi" && board.blackAttackBoard[newXPos, newYPos] != true) || (name == "BKi" && board.whiteAttackBoard[newXPos, newYPos] != true)
                    || (name != "BKi" && name != "WKi"))
                {
                    GameObject cp = board.boardPositions[newXPos, newYPos];
                    if (cp == null)
                    {
                        passiveMove(newXPos, newYPos, isPlace);
                    }
                    else if (cp.GetComponent<Pieces>().playerColour != playerColour)
                    {
                        attackTile(newXPos, newYPos, isPlace);
                    }
                }
            }
        }
        else
        {
            if (board.validBoardPos(newXPos, newYPos))
            {
                setAttackBoardPos(newXPos, newYPos);
            }
        }

    }

    public void straightLineMove(int changeInX, int changeInY, bool isPlace)
    {
        int possibleXPos = xPos + changeInX;
        int possibleYPos = yPos + changeInY;
        Main board = controller.GetComponent<Main>();
        //use a while loops as we will create a moveTile every time a space is fre to move until it is not.
        while (board.validBoardPos(possibleXPos, possibleYPos) && board.boardPositions[possibleXPos, possibleYPos] == null)
        {
            //creates a move tile
            moveTile(possibleXPos, possibleYPos, isPlace);
            setAttackBoardPos(possibleXPos, possibleYPos);
            possibleXPos += changeInX; possibleYPos += changeInY;
        }

        if (board.validBoardPos(possibleXPos, possibleYPos) && board.boardPositions[possibleXPos, possibleYPos].GetComponent<Pieces>().playerColour != playerColour)
        {
            //creates an attack tile
            attackTile(possibleXPos, possibleYPos, isPlace);
            setAttackBoardPos(possibleXPos, possibleYPos);
            if (board.validBoardPos(possibleXPos + changeInX, possibleYPos + changeInY) && (board.boardPositions[possibleXPos, possibleYPos].GetComponent<Pieces>().name == "WKi" ||
                board.boardPositions[possibleXPos, possibleYPos].GetComponent<Pieces>().name == "BKi"))
            {
                setAttackBoardPos(possibleXPos + changeInX, possibleYPos + changeInY);
            }
        }
        //This is used to also show it can attack the same piece as the same colour, incase that a piece is taken of same colour and that you can still take it
        if (board.validBoardPos(possibleXPos, possibleYPos) && !isPlace)
        {
            setAttackBoardPos(possibleXPos, possibleYPos);
        }

    }

    public void passiveMove(int xPos, int yPos, bool isPlace)
    {
        //Used to create a move tile
        Main Board = controller.GetComponent<Main>();
        if(Board.validBoardPos(xPos,yPos) && Board.boardPositions[xPos, yPos] == null)
        {
            moveTile(xPos, yPos, isPlace);
        }
    }

    public void passiveMovePawn(int x, int y, bool isPlace)
    {
        if (isPlace)
        {
            if (!moveOutCheck(x, y))
            {
                //Used to instantiate a move tile
                GameObject mt = Instantiate(moveTiles, new Vector3((x * 0.66f) - 2.3f, (y * 0.66f) - 2.3f, -3.0f), Quaternion.identity);
                mt.gameObject.tag = "MovePlate";
                MoveTiles mtscript = mt.GetComponent<MoveTiles>();
                mtscript.setCurrentPiece(gameObject);
                mtscript.setBoardPos(x, y);
                int val = Math.Abs(yPos - y);
                if (val == 2)
                {
                    mtscript.enPassant = true;
                }
            }
        }
    }

    public void enPassantMovePawn(int x, int y)
    {
        if (!moveOutCheck(x, y))
        {
            //Used to instantiate an attack tile
            GameObject at = Instantiate(moveTiles, new Vector3((x * 0.66f) - 2.3f, (y * 0.66f) - 2.3f, -3.0f), Quaternion.identity);
            at.gameObject.tag = "MovePlate";
            MoveTiles mtscript = at.GetComponent<MoveTiles>();
            mtscript.attackEnPassant = true;
            mtscript.setCurrentPiece(gameObject);
            mtscript.setBoardPos(x, y);
        }
    }

    public void DestroyTiles()
    {
        //Finds all the gameobjects withb the tag "MovePlate" and destroys them
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for (int i=0; i < movePlates.Length; i++)
        {
            Destroy(movePlates[i]);
        }
    }

    public void moveTile(int x,int y, bool isPlace)
    {
        if (isPlace)
        {
            if (isPlace && !moveOutCheck(x,y))
            {
                //Used to instantiate a move tile
                GameObject mt = Instantiate(moveTiles, new Vector3((x * 0.66f) - 2.3f, (y * 0.66f) - 2.3f, -3.0f), Quaternion.identity);
                mt.gameObject.tag = "MovePlate";
                MoveTiles mtscript = mt.GetComponent<MoveTiles>();
                mtscript.setCurrentPiece(gameObject);
                mtscript.setBoardPos(x, y);
            }
        }
        else
        {
            setAttackBoardPos(x, y);
        }
    }

    public void attackTile(int x, int y, bool isPlace)
    {
        if (isPlace)
        {
            if (isPlace && !moveOutCheck(x,y))
            {
                //Used to instantiate an attack tile
                GameObject at = Instantiate(moveTiles, new Vector3((x * 0.66f) - 2.3f, (y * 0.66f) - 2.3f, -3.0f), Quaternion.identity);
                at.gameObject.tag = "MovePlate";
                MoveTiles mtscript = at.GetComponent<MoveTiles>();
                mtscript.isAttack = true;
                mtscript.setCurrentPiece(gameObject);
                mtscript.setBoardPos(x, y);
            }
        }
        else
        {
            setAttackBoardPos(x, y);
        }
    }

    public void moveCastleTile(int x, int y, GameObject piece, int changeInX)
    {
        //used to instaniate a move tile when the king castles
        GameObject mt = Instantiate(moveTiles, new Vector3((x * 0.66f) - 2.3f, (y * 0.66f) - 2.3f, -3.0f), Quaternion.identity);
        mt.gameObject.tag = "MovePlate";
        MoveTiles mtscript = mt.GetComponent<MoveTiles>();
        mtscript.setCurrentPiece(gameObject);
        mtscript.setBoardPos(x, y);
        mtscript.isCastle = true;
        mtscript.castlePiece = piece;
        mtscript.castleXPos = x;
        mtscript.castleYPos = y;
        mtscript.changeInX = changeInX;

    }

    public bool checkPromote(bool isPlace)
    {
        if(isPlace)
        {
            int indexVal = 16;
            //used to check if a pawn can be promoted
            Main board = controller.GetComponent<Main>();
            if (yPos == 7)
            {
                indexVal = Array.IndexOf(board.whitePieces, gameObject);
                board.setPiece(xPos, yPos, "W", indexVal, "default", false);
                return true;
            }
            else if (yPos == 0)
            {
                indexVal = Array.IndexOf(board.blackPieces, gameObject);
                board.setPiece(xPos, yPos, "B", indexVal, "default", false);
                return true;
            }
        }
        return false;

    }

    public void setAttackBoardPos(int x, int y)
    {
        Main board = controller.GetComponent<Main>();
        if(playerColour == "W")
        {
            board.whiteAttackBoard[x, y] = true;
        } else if (playerColour == "B")
        {
            board.blackAttackBoard[x, y] = true;
        }
    }

    public bool moveOutCheck(int newXPos, int newYPos) //-> true = cannot move, false = possible move
    {   //Check if the move is legal / won't cause check on own side
        bool isAttack = false;
        //replicating the piece
        int refXPos = xPos;
        int refYPos = yPos;
        int index = 0;
        GameObject refObj = gameObject;

        Main board = controller.GetComponent<Main>();
        if (name!= "WKi" && name!= "BKi")
        // The kings do not need to use this script
        {
            if (playerColour== "W") 
                //If the piece is white
            {
                if (board.boardPositions[newXPos,newYPos] != null) 
                    //check if attack move
                {
                    isAttack = true;
                    index = Array.IndexOf(board.blackPieces, board.boardPositions[newXPos, newYPos]);
                    //get the piece that it is attacking
                    refObj = board.blackPieces[index]; 
                    board.blackPieces[index] = null;
                    //removes the piece
                    board.boardPositions[newXPos, newYPos] = null;
                }
                else if (enPassantAttack && board.boardPositions[newXPos, newYPos] == null && 
                    board.boardPositions[newXPos,newYPos-1].GetComponent<Pieces>().playerColour != playerColour)
                { //similar script as before but refers to when a piece can enpassent
                    isAttack = true;
                    index = Array.IndexOf(board.blackPieces, board.boardPositions[newXPos, newYPos - 1]);
                    refObj = board.blackPieces[index];
                    board.blackPieces[index] = null;
                    board.boardPositions[newXPos, newYPos + 1] = null;
                }
                //Moves the piece to the corresponding location
                board.boardPositions[xPos, yPos] = null;
                xPos = newXPos;
                yPos = newYPos;
                board.assignBoardPos(gameObject);
                //creates a new attackboard, to check if in check
                board.setupAttackBoard();
                if (checkInCheck("W"))
                { //resets the board to the previous position
                    board.boardPositions[newXPos, newYPos] = null;
                    if (isAttack)
                    {
                        //reassigns old piece to board
                        board.blackPieces[index] = refObj;
                        board.assignBoardPos(board.blackPieces[index]);
                    }
                    xPos = refXPos;
                    yPos = refYPos;
                    board.assignBoardPos(gameObject);
                    return true;
                }
                else
                {//resets the board to the previous position
                    board.boardPositions[newXPos, newYPos] = null;
                    if (isAttack)
                    {
                        board.blackPieces[index] = refObj;
                        board.assignBoardPos(board.blackPieces[index]);
                        //reassigns the place 
                    }
                    xPos = refXPos;
                    yPos = refYPos;
                    board.assignBoardPos(gameObject);
                    return false;
                }
            }
            else if (playerColour == "B")
            {//else for black side
                if (board.boardPositions[newXPos, newYPos] != null)
                {
                    isAttack = true;
                    index = Array.IndexOf(board.whitePieces, board.boardPositions[newXPos, newYPos]);
                    refObj = board.whitePieces[index];
                    board.whitePieces[index] = null;
                    board.boardPositions[newXPos, newYPos] = null;
                }
                else if (enPassantAttack && board.boardPositions[newXPos, newYPos] == null && board.boardPositions[newXPos, newYPos + 1].GetComponent<Pieces>().playerColour != playerColour)
                {
                    isAttack = true;
                    index = Array.IndexOf(board.whitePieces, board.boardPositions[newXPos, newYPos + 1]);
                    refObj = board.whitePieces[index];
                    board.whitePieces[index] = null;
                    board.boardPositions[newXPos, newYPos + 1] = null;
                }
                board.boardPositions[xPos, yPos] = null;
                xPos = newXPos;
                yPos = newYPos;
                board.assignBoardPos(gameObject);
                board.setupAttackBoard();
                if (checkInCheck("B"))
                {
                    board.boardPositions[newXPos, newYPos] = null;
                    if (isAttack)
                    {
                        board.whitePieces[index] = refObj;
                        board.assignBoardPos(board.whitePieces[index]);

                    }
                    xPos = refXPos;
                    yPos = refYPos;
                    board.assignBoardPos(gameObject);
                    return true;
                }
                else
                {
                    board.boardPositions[newXPos, newYPos] = null;
                    if (isAttack)
                    {
                        board.whitePieces[index] = refObj;
                        board.assignBoardPos(board.whitePieces[index]);

                    }
                    xPos = refXPos;
                    yPos = refYPos;
                    board.assignBoardPos(gameObject);
                    return false;
                }
            }
            else
            {
                return false;
            }

        }
        else
        {
            return false;
        }
    }

    public bool checkInCheck(string colour)
    {
        Main board = controller.GetComponent<Main>();
        if (colour == "W")
        {//gets position of king in attack board, if true, the king is in check
            if (board.blackAttackBoard[board.whitePieces[4].GetComponent<Pieces>().xPos, board.whitePieces[4].GetComponent<Pieces>().yPos])
            {
                return true;
            }
        }
        else if (colour == "B")
        {
            if (board.whiteAttackBoard[board.blackPieces[4].GetComponent<Pieces>().xPos, board.blackPieces[4].GetComponent<Pieces>().yPos])
            {
                return true;
            }
        }
        return false;
    }


    private void OnMouseUp()
    {
        //When the user clicks it will destroy all tiles that had originally been there (if the player clicked on a piece and it didnt move), and 
        //show all possible moves for the piece that was clicked on this is done by giving the prefab a 2d box collider
        Main board = controller.GetComponent<Main>();
        DestroyTiles();
        board.setupAttackBoard();
        if (!board.gameOver && board.canMove)
        {//if the user can move
            if (!board.againstAI && board.currentPlayer[0].ToString() == playerColour.ToLower())
            {
                possibleMoves(true);
            }
            //if against AI (only white can select moves)
            else if (board.againstAI)
            {
                if (board.currentPlayer == "white" && playerColour == "W")
                {
                    possibleMoves(true);
                }
            }
        }
    }

}