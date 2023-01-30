using UnityEngine;
public class MoveTiles : MonoBehaviour
{
    //Used to reference game objects
    public GameObject controller; // references the controller
    public GameObject currentPiece; //references the piece you are moving
    public GameObject castlePiece; //references the castle if you are castling

    public Sprite attackTile;
    //Used to store where the curent piece wants to move to
    public int xBoard;
    public int yBoard;

    //Used in castling
    public bool isCastle = false;
    public int castleXPos;
    public int castleYPos;
    public int changeInX;

    //Used in en Passant
    public bool enPassant = false;
    public bool attackEnPassant = false;

    //Used to check to create an attack or move tile
    public bool isAttack = false;

    //used for moves to be stored onto a file
    public string moveType = "";

    public void Start()
    {
        if (isAttack || attackEnPassant)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1);
            gameObject.GetComponent<SpriteRenderer>().sprite = attackTile;
        }
    }

    public void OnMouseUp()
    {
        doMove();
    }

    public void setBoardPos(int x,int y)
    {
        xBoard = x;
        yBoard = y;
    }

    //used to reference the current piece
    public void setCurrentPiece(GameObject obj)
    {
        currentPiece= obj;
    }

    public void doMove()
    {
        //called when the tile is clicked on (used by assigning component box collider 2d) 
        controller = GameObject.FindGameObjectWithTag("GameController");
        Main board = controller.GetComponent<Main>();
        //Used when an attack tile is made
        if (isAttack)
        {
            moveType = translateNotation(currentPiece.GetComponent<Pieces>().xPos, currentPiece.GetComponent<Pieces>().yPos,
                xBoard, yBoard, true, currentPiece.GetComponent<Pieces>().name);
            attackMove();
        }
        else if (attackEnPassant)
        {
            moveType = translateNotation(currentPiece.GetComponent<Pieces>().xPos, currentPiece.GetComponent<Pieces>().yPos, xBoard, yBoard, true, currentPiece.GetComponent<Pieces>().name);
            if (currentPiece.GetComponent<Pieces>().playerColour == "W")
            {
                //destroy piece below
                GameObject piece = board.boardPositions[xBoard, yBoard - 1];
                Destroy(piece);
            }
            else
            {
                //destroy piece above
                GameObject piece = board.boardPositions[xBoard, yBoard + 1];
                Destroy(piece);
            }
        }
        //Used when a castle tile is made
        else if (isCastle)
        {
            board.historyLength = 0;
            board.boardPositions[castleXPos - changeInX, castleYPos] = null;
            if (changeInX == 2)
            {
                //kingside castle
                moveType = "0-0-0";
                changeInX = 1;
            }
            else
            {
                //queenside castle
                moveType = "0-0";
            }
            //castling / moving the pieces
            castlePiece.GetComponent<Pieces>().xPos = xBoard + changeInX;
            castlePiece.GetComponent<Pieces>().yPos = yBoard;
            castlePiece.GetComponent<Pieces>().setCoordPos();
            board.assignBoardPos(castlePiece);
        }
        else
        {
            //normal move notation translation
            moveType = translateNotation(currentPiece.GetComponent<Pieces>().xPos, currentPiece.GetComponent<Pieces>().yPos,
                xBoard, yBoard, false, currentPiece.GetComponent<Pieces>().name);
        }
        //changes the hasMoved variable for a piece to true if it hasnt moved yet
        if (!currentPiece.GetComponent<Pieces>().hasMoved)
        {
            currentPiece.GetComponent<Pieces>().hasMoved = true;
        }
        //changes the location of the current piece to the wanted location
        board.boardPositions[currentPiece.GetComponent<Pieces>().xPos, currentPiece.GetComponent<Pieces>().yPos] = null;

        currentPiece.GetComponent<Pieces>().xPos = xBoard;
        currentPiece.GetComponent<Pieces>().yPos = yBoard;
        currentPiece.GetComponent<Pieces>().setCoordPos();

        board.assignBoardPos(currentPiece);

        //destroys all tiles
        currentPiece.GetComponent<Pieces>().DestroyTiles();

        //Used for pawn being promoted
        if (currentPiece.GetComponent<Pieces>().name[1] == 'P')
        {
            bool check = currentPiece.GetComponent<Pieces>().checkPromote(true);
            char[] vals = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
            if (check)
            {
                moveType = "P" + vals[xBoard] + (yBoard+1).ToString();
            }
        }

        //used to allow enPassant
        if (enPassant)
        {
            enPassantMove();
        }

        //used to clear all enpassant pieces each turn if not used
        if (board.currentPlayer == "white")
        {
            for (int currentPiece = 8; currentPiece < board.whitePieces.Length; currentPiece++)
            {
                if (board.whitePieces[currentPiece] != null)
                {//sets each pawn to false
                    board.whitePieces[currentPiece].GetComponent<Pieces>().enPassantAttack = false;
                }
            }
        }
        else if (board.currentPlayer == "black")
        {
            for (int currentPiece = 8; currentPiece < board.whitePieces.Length; currentPiece++)
            {
                if (board.blackPieces[currentPiece] != null)
                {//sets each pawn to false
                    board.blackPieces[currentPiece].GetComponent<Pieces>().enPassantAttack = false;
                }
            }
        }

        board.moveHistory.Add(moveType);

        //50 moves rule
        if (currentPiece.GetComponent<Pieces>().name == "WP" || currentPiece.GetComponent<Pieces>().name == "BP" || attackEnPassant || isAttack)
        {//pawns and attacking moves reset the counter
            board.moves = 0.0f;
            board.historyLength = 0;
        }
        else
        {//increase counter (if 50, the game ends)
            board.moves += 0.5f;
            if (board.moves == 50f)
            {
                board.drawEnd();
            }
        }
        board.changeCurrentPlayer();

        board.setupAttackBoard();
        board.checkDraw(); //checks draw by lack of material

        //checks for threefold repition
        if (board.convertBoardToInt())
        {
            board.drawEnd();
        }
    }

   
    
    
    public void enPassantMove()
    {   //sets enpassant variables if a pawn does the certain requirements
        Main board = controller.GetComponent<Main>();
        if (board.validBoardPos(xBoard + 1, yBoard))
        {
            if (board.boardPositions[xBoard + 1, yBoard] != null)
            {
                if (board.boardPositions[xBoard + 1, yBoard].GetComponent<Pieces>().name[1] == 'P' && 
                    board.boardPositions[xBoard + 1, yBoard].GetComponent<Pieces>().playerColour 
                    != currentPiece.GetComponent<Pieces>().playerColour)
                    //if the piece is a pawn of different colour
                {
                    board.boardPositions[xBoard + 1, yBoard].GetComponent<Pieces>().enPassantAttack = true;
                    if (currentPiece.GetComponent<Pieces>().playerColour == "W")
                    {//white pawn
                        //sets the position of where the piece can enpassant
                        board.boardPositions[xBoard + 1, yBoard].GetComponent<Pieces>().enPassantXPos = xBoard;
                        board.boardPositions[xBoard + 1, yBoard].GetComponent<Pieces>().enPassantYPos = yBoard - 1;
                    }
                    else
                    { //black pawn
                        board.boardPositions[xBoard + 1, yBoard].GetComponent<Pieces>().enPassantXPos = xBoard;
                        board.boardPositions[xBoard + 1, yBoard].GetComponent<Pieces>().enPassantYPos = yBoard + 1;
                    }
                }
            }
        }//repeats for the other side of the pawn
        if (board.validBoardPos(xBoard - 1, yBoard))
        {
            if (board.boardPositions[xBoard - 1, yBoard] != null)
            {
                if (board.boardPositions[xBoard - 1, yBoard].GetComponent<Pieces>().name[1] == 'P' && 
                    board.boardPositions[xBoard - 1, yBoard].GetComponent<Pieces>().playerColour 
                    != currentPiece.GetComponent<Pieces>().playerColour)
                {
                    board.boardPositions[xBoard - 1, yBoard].GetComponent<Pieces>().enPassantAttack = true;
                    if (currentPiece.GetComponent<Pieces>().playerColour == "W")
                    {
                        board.boardPositions[xBoard - 1, yBoard].GetComponent<Pieces>().enPassantXPos = xBoard;
                        board.boardPositions[xBoard - 1, yBoard].GetComponent<Pieces>().enPassantYPos = yBoard - 1;
                    }
                    else
                    {
                        board.boardPositions[xBoard - 1, yBoard].GetComponent<Pieces>().enPassantXPos = xBoard;
                        board.boardPositions[xBoard - 1, yBoard].GetComponent<Pieces>().enPassantYPos = yBoard + 1;
                    }
                }
            }
        }
    }

    public void attackMove()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        Main board = controller.GetComponent<Main>();
        if (board.boardPositions[xBoard, yBoard].name == "WKi" || board.boardPositions[xBoard, yBoard].name == "BKi")
        {
            //used to check if you take the king (only used if a glitch occurs as this shouldnt be possible)
            currentPiece.GetComponent<Pieces>().DestroyTiles();
            board.gameEnd(controller.GetComponent<Main>().boardPositions[xBoard, yBoard].name);
        }
        if (board.boardPositions[xBoard, yBoard].GetComponent<Pieces>().playerColour == "W")
        {
            //gets piece worth adding to corresponding colour
            board.blackPoints += board.boardPositions[xBoard, yBoard].GetComponent<Pieces>().getPieceWorth();
            board.whitePiecesLength -= 1;
        }
        else if (board.boardPositions[xBoard, yBoard].GetComponent<Pieces>().playerColour == "B")
        {
            board.whitePoints += board.boardPositions[xBoard, yBoard].GetComponent<Pieces>().getPieceWorth();
            board.blackPiecesLength -= 1;
        }
        GameObject piece = board.boardPositions[xBoard, yBoard];
        Destroy(piece);
    }

    
    public string translateNotation(int oldXAxis, int oldYAxis, int newXAxis, int newYAxis, bool isAttacking,string pieceName)
    {
        //used to translate each move into algebraic notation to later be stored in moves history
        char[] vals = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
        string pieceNameDone = "";
        string attacking = "";
        if (isAttacking)
        {
            attacking = "x";
        }
        //Convert name to notation
        switch (pieceName)
        {
            case "BB":
            case "WB":
                pieceNameDone = "B";
                break;
            case "BKi":
            case "WKi":
                pieceNameDone = "K";
                break;
            case "BKn":
            case "WKn":
                pieceNameDone = "N";
                break;
            case "BP":
            case "WP":
                pieceNameDone = "P";
                break;
            case "BQ":
            case "WQ":
                pieceNameDone = "Q";
                break;
            case "BR":
            case "WR":
                pieceNameDone = "R";
                break;
        }
        //create full string
        return pieceNameDone + vals[oldXAxis] + (oldYAxis+1).ToString() + attacking + vals[newXAxis] + (newYAxis + 1).ToString();
    }
}
