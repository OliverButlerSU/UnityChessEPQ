using UnityEngine;


public class AI : MonoBehaviour
{
    public GameObject controller;

    // Start is called before the first frame update
    private void Start()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
    }


    public void AiButton()
    {
        Main main = controller.GetComponent<Main>();

        if (main.currentPlayer == "black" && main.againstAI == true 
            && main.gameOver == false && main.canMove == true)
        {
            getMove();
        }
    }
    public void getMove()
    {
        //gets moves
        controller = GameObject.FindGameObjectWithTag("GameController");
        for (int currentVal = 0; currentVal < controller.GetComponent<Main>().blackPieces.Length; currentVal++)
        {
            if (controller.GetComponent<Main>().blackPieces[currentVal] != null)
            {
                controller.GetComponent<Main>().blackPieces[currentVal].GetComponent<Pieces>().possibleMoves(true);
            }
        }
        GameObject[] moveTiles = GameObject.FindGameObjectsWithTag("MovePlate");
        //array of the worth of each move
        float[] calculatedMoves = new float[moveTiles.Length];
        for (int moveVal = 0; moveVal < moveTiles.Length; moveVal++)
        {
            //calculate moves
            calculatedMoves[moveVal] = calculateMove(moveTiles[moveVal]);
        }
        //get the highest and do the move
        moveTiles[getHighestVal(calculatedMoves)].GetComponent<MoveTiles>().doMove();

    }

    public int getHighestVal(float[] valArray)
    {
        float highestVal = valArray[0];
        int returnIndex = 0;
        for (int currentVal = 1; currentVal < valArray.Length; currentVal++)
        {
            if (valArray[currentVal] > highestVal)
            {
                highestVal = valArray[currentVal];
                returnIndex = currentVal;
            }
        }
        return returnIndex;
    }

    public float calculateMove(GameObject moveTile)
    {
        float moveVal = 0;
        //if attacking move
        if (moveTile.GetComponent<MoveTiles>().isAttack == true)
        {
            //calculate the worth of the piece being taken
            moveVal += controller.GetComponent<Main>().boardPositions[moveTile.GetComponent<MoveTiles>().xBoard, moveTile.GetComponent<MoveTiles>().yBoard].GetComponent<Pieces>().getPieceWorth() * 10
                + getSwitchTable(moveTile.GetComponent<MoveTiles>().xBoard, moveTile.GetComponent<MoveTiles>().yBoard,
                controller.GetComponent<Main>().boardPositions[moveTile.GetComponent<MoveTiles>().xBoard, moveTile.GetComponent<MoveTiles>().yBoard].GetComponent<Pieces>().name);
        }
        //calculate the change in move worth
        string tempName = moveTile.GetComponent<MoveTiles>().currentPiece.GetComponent<Pieces>().name;
        moveVal += getSwitchTable(moveTile.GetComponent<MoveTiles>().xBoard, moveTile.GetComponent<MoveTiles>().yBoard, tempName)
            - getSwitchTable(moveTile.GetComponent<MoveTiles>().currentPiece.GetComponent<Pieces>().xPos, moveTile.GetComponent<MoveTiles>().currentPiece.GetComponent<Pieces>().yPos, tempName);

        //determine if the piece can be taken if you move to this location using the attack board
        if(controller.GetComponent<Main>().whiteAttackBoard[moveTile.GetComponent<MoveTiles>().xBoard, moveTile.GetComponent<MoveTiles>().yBoard] == true)
        { //(we subtract the value of the piece if so, assuming it would take the piece)
            moveVal -= controller.GetComponent<Main>().boardPositions[moveTile.GetComponent<MoveTiles>().currentPiece.GetComponent<Pieces>().xPos, 
                moveTile.GetComponent<MoveTiles>().currentPiece.GetComponent<Pieces>().yPos].GetComponent<Pieces>().getPieceWorth() * 10;
        }
        return moveVal;
    }

    public float getSwitchTable(int xPos, int yPos, string name)
    {
        switch (name)
        {
            case ("WKi"):
            case ("BKi"):
                return getKingTable(xPos,yPos);
            case ("WKn"):
            case ("BKn"):
                return getKnightTable(xPos, yPos);
            case ("WQ"):
            case ("BQ"):
                return getQueenTable(xPos, yPos);
            case ("WP"):
            case ("BP"):
                return getPawnTable(xPos, yPos);
            case ("WR"):
            case ("BR"):
                return getRookTable(xPos, yPos);
        }
        return 0;
    }

    public float getKnightTable(int xPos, int yPos)
    {
        int newXPos = 7 - xPos;
        int newYPos = 7 - yPos;
        float[,] knightTable =
            { {-5,-4,-3,-3,-3,-3,-4,-5 },
            {-4,-2,  0,  0,  0,  0,-2,-4 },
            {-3,  0, 1, 1.5f, 1.5f, 1,  0,-3 },
            {-3,  0.5f, 1.5f, 2, 2, 1.5f, 0.5f,-3 },
            {-3,  0, 1.5f, 2, 2, 1.5f,  0,-3 },
            {-3,  0.5f, 1, 1.5f, 1.5f, 1,  0.5f,-3 },
            {-4,-2,  0,  0.5f,  0.5f,  0,-2,-4 },
            {-5,-4,-3,-3,-3,-3,-4,-5 } };
        return knightTable[newXPos, newYPos];
    }

    public float getKingTable(int xPos, int yPos)
    {
        int newXPos = 7 - xPos;
        int newYPos = 7 - yPos;
        float[,] kingTable =
            { {-3,-4,-4,-5,-5,-4,-4,-3 },
            {-3,-4,-4,-5,-5,-4,-4,-3 },
            {-3,-4,-4,-5,-5,-4,-4,-3 },
            {-3,-4,-4,-5,-5,-4,-4,-3 },
            {-2,-3,-3,-4,-4,-3,-3,-2 },
            {-1,-2,-2,-2,-2,-2,-2,-1 },
            {2, 2,  0,  0,  0,  0, 2, 2},
            {2, 3, 1,  0,  0, 1, 3, 2 } };
        return kingTable[newXPos, newYPos];
    }

    public float getQueenTable(int xPos, int yPos)
    {
        int newXPos = 7 - xPos;
        int newYPos = 7 - yPos;
        float[,] queenTable =
            { {-2,-1,-1, -0.5f, -0.5f,-1,-1,-2 },
            {-1,  0,  0,  0,  0,  0,  0,-1 },
            {-1,  0,  0.5f,  0.5f,  0.5f,  0.5f,  0, -1 },
            {-0.5f,  0,  0.5f,  0.5f,  0.5f,  0.5f,  0, -0.5f },
            {0,  0,  0.5f,  0.5f,  0.5f,  0.5f,  0, -0.5f },
            {-1,  0.5f,  0.5f,  0.5f,  0.5f,  0.5f,  0,-1 },
            {-1,  0,  0.5f,  0,  0,  0,  0,-1 },
            {-2,-1,-1, -0.5f, -0.5f,-1,-1, -2 } };
        return queenTable[newXPos, newYPos];
    }

    public float getBishopTable(int xPos, int yPos)
    {
        int newXPos = 7 - xPos;
        int newYPos = 7 - yPos;
        float[,] bishopTable =
            { {-2,-1,-1,-1,-1,-1,-1,-2 },
            {-1,  0,  0,  0,  0,  0,  0,-1 },
            {-1,  0,  0.5f, 1, 1,  0.5f,  0,-1 },
            {-1,  0.5f,  0.5f, 1, 1,  0.5f,  0.5f,-1 },
            {-1,  0, 1, 1, 1, 1,  0,-1 },
            {-1, 1, 1, 1, 1, 1, 1,-1 },
            {-1,  0.5f,  0,  0,  0,  0,  0.5f,-1 },
            {-2,-1,-1,-1,-1,-1,-1,-2 } };
        return bishopTable[newXPos, newYPos];
    }

    public float getRookTable(int xPos, int yPos)
    {
        int newXPos = 7 - xPos;
        int newYPos = 7 - yPos;
        float[,] rookTable =
            {{0,  0,  0,  0,  0,  0,  0,  0 },
            {0.5f, 1, 1, 1, 1, 1, 1,  0.5f },
            {-0.5f,  0,  0,  0,  0,  0,  0, -0.5f },
            {-0.5f,  0,  0,  0,  0,  0,  0, -0.5f },
            {-0.5f,  0,  0,  0,  0,  0,  0, -0.5f },
            {-0.5f,  0,  0,  0,  0,  0,  0, -0.5f },
            {-0.5f,  0,  0,  0,  0,  0,  0, -0.5f },
            {0,  0,  0,  0.5f,  0.5f,  0,  0,  0 } };
        return rookTable[newXPos, newYPos];
    }

    public float getPawnTable(int xPos, int yPos)
    {
        int newXPos = 7 - xPos;
        int newYPos = 7 - yPos;
        float[,] pawnTable =
            {{10, 10, 10, 10, 10, 10, 10, 10},
            {5, 5, 5, 5, 5, 5, 5, 5},
            {1, 1, 2, 3, 3, 2, 1, 1},
            {5, 5, 1, 2.5f, 2.5f, 1, 0.5f, 0.5f},
            {0, 0, 0, 2, 2, 0, 0, 0},
            {0.5f, -0.5f, -1, 0, 0, -1, -0.5f, 0.5f},
            {0.5f, 1, 1, -2, -2, 1, 1, 0.5f},
            {10, 10, 10, 10, 10, 10, 10, 10}};
        return pawnTable[newXPos, newYPos];
    }


    //vvv Old Broken Minimax Code vvv


    //public void getMove()
    //{
    //    for (int currentVal = 0; currentVal < controller.GetComponent<Main>().blackPieces.Length; currentVal++)
    //    {
    //        if (controller.GetComponent<Main>().blackPieces[currentVal] != null)
    //        {
    //            controller.GetComponent<Main>().blackPieces[currentVal].GetComponent<Pieces>().possibleMoves(true);
    //        }
    //    }
    //    GameObject[] moveTiles = GameObject.FindGameObjectsWithTag("MovePlate");


    //    float maxVal = -10000000;
    //    int index = 0;
    //    for(int currentTile = 0; currentTile < moveTiles.Length; currentTile++)
    //    {
    //        moveTiles[currentTile].GetComponent<MoveTiles>().doMove();
    //        float checkVal = minimax(0, false);
    //        if(maxVal > checkVal)
    //        {
    //            maxVal = checkVal;
    //            index = currentTile;
    //        }

    //    }
    //    moveTiles[index].GetComponent<MoveTiles>().doMove();

    //}

    //public float minimax(int depth, bool maximizingPlayer)
    //{
    //    if (depth == 0)
    //    {
    //        return calculatePosition();
    //    }

    //    if (maximizingPlayer)
    //    {
    //        float maxEval = -10000000000;
    //        //get all possible moves for the black side using the below arrays
    //        GameObject[] childMoves = getBlackMoves();
    //        for (int currentPos = 0; currentPos < childMoves.Length; currentPos++)
    //        {
    //            childMoves[currentPos].GetComponent<MoveTiles>().doMove();
    //            float evaluedMove = minimax(depth - 1, false);
    //            if (maxEval < evaluedMove)
    //            {
    //                maxEval = evaluedMove;
    //            }
    //        }
    //        return maxEval;
    //    }
    //    else
    //    {
    //        float minEval = 10000000000;
    //        //get all possible moves for the white side using the below arrays
    //        GameObject[] childMoves = getWhiteMoves();
    //        for (int currentPos = 0; currentPos < childMoves.Length; currentPos++)
    //        {
    //            childMoves[currentPos].GetComponent<MoveTiles>().doMove();

    //            float evaluedMove = minimax(depth - 1, true);
    //            if (minEval > evaluedMove)
    //            {
    //                minEval = evaluedMove;
    //            }
    //        }
    //        return minEval;
    //    }
    //}

    //public GameObject[] getBlackMoves()
    //{
    //    for (int currentVal = 0; currentVal < controller.GetComponent<Main>().blackPieces.Length; currentVal++)
    //    {
    //        controller.GetComponent<Main>().blackPieces[currentVal].GetComponent<Pieces>().possibleMoves(true);
    //    }
    //    GameObject[] moveTiles = GameObject.FindGameObjectsWithTag("MovePlate");
    //    return moveTiles;
    //}

    //public GameObject[] getWhiteMoves()
    //{
    //    for (int currentVal = 0; currentVal < controller.GetComponent<Main>().whitePieces.Length; currentVal++)
    //    {
    //        controller.GetComponent<Main>().whitePieces[currentVal].GetComponent<Pieces>().possibleMoves(true);
    //    }
    //    GameObject[] moveTiles = GameObject.FindGameObjectsWithTag("MovePlate");
    //    return moveTiles;
    //}



    //public float calculatePosition()
    //{
    //    //get all pieces, calculate by adding up (black pieces worth + their position on board) - (white piece worth + their position on board)
    //    float blackPoints = 0;
    //    float whitePoints = 0;

    //    for (int currentVal = 0; currentVal < controller.GetComponent<Main>().blackPieces.Length; currentVal++)
    //    {
    //        if (controller.GetComponent<Main>().blackPieces[currentVal] != null)
    //        {
    //            blackPoints += calculatePiece(controller.GetComponent<Main>().blackPieces[currentVal]);
    //        }
    //        if (controller.GetComponent<Main>().whitePieces[currentVal] != null)
    //        {
    //            whitePoints += calculatePiece(controller.GetComponent<Main>().whitePieces[currentVal]);
    //        }
    //    }
    //    return blackPoints - whitePoints;
    //}

    //public float calculatePiece(GameObject moveTile)
    //{
    //    float moveVal = 0;
    //    bool isBlack = (moveTile.GetComponent<MoveTiles>().currentPiece.GetComponent<Pieces>().playerColour == "B") ? true : false;
    //    switch (moveTile.GetComponent<MoveTiles>().currentPiece.GetComponent<Pieces>().name)
    //    {
    //        case ("WKi"): //new pos - old pos
    //        case ("BKi"):
    //            moveVal += getKingTable(moveTile.GetComponent<MoveTiles>().xBoard, moveTile.GetComponent<MoveTiles>().yBoard, isBlack) - getKingTable(moveTile.GetComponent<MoveTiles>().currentPiece.GetComponent<Pieces>().xPos, moveTile.GetComponent<MoveTiles>().currentPiece.GetComponent<Pieces>().yPos, isBlack);
    //            break;
    //        case ("WKn"):
    //        case ("BKn"):
    //            moveVal += getKnightTable(moveTile.GetComponent<MoveTiles>().xBoard, moveTile.GetComponent<MoveTiles>().yBoard, isBlack) - getKnightTable(moveTile.GetComponent<MoveTiles>().currentPiece.GetComponent<Pieces>().xPos, moveTile.GetComponent<MoveTiles>().currentPiece.GetComponent<Pieces>().yPos, isBlack);
    //            break;
    //        case ("WQ"):
    //        case ("BQ"):
    //            moveVal += getQueenTable(moveTile.GetComponent<MoveTiles>().xBoard, moveTile.GetComponent<MoveTiles>().yBoard, isBlack) - getQueenTable(moveTile.GetComponent<MoveTiles>().currentPiece.GetComponent<Pieces>().xPos, moveTile.GetComponent<MoveTiles>().currentPiece.GetComponent<Pieces>().yPos, isBlack);
    //            break;
    //        case ("WP"):
    //        case ("BP"):
    //            moveVal += getPawnTable(moveTile.GetComponent<MoveTiles>().xBoard, moveTile.GetComponent<MoveTiles>().yBoard, isBlack) - getPawnTable(moveTile.GetComponent<MoveTiles>().currentPiece.GetComponent<Pieces>().xPos, moveTile.GetComponent<MoveTiles>().currentPiece.GetComponent<Pieces>().yPos, isBlack);
    //            break;
    //        case ("WR"):
    //        case ("BR"):
    //            moveVal += getRookTable(moveTile.GetComponent<MoveTiles>().xBoard, moveTile.GetComponent<MoveTiles>().yBoard, isBlack) - getRookTable(moveTile.GetComponent<MoveTiles>().currentPiece.GetComponent<Pieces>().xPos, moveTile.GetComponent<MoveTiles>().currentPiece.GetComponent<Pieces>().yPos, isBlack);
    //            break;
    //    }
    //    return moveTile.GetComponent<MoveTiles>().currentPiece.GetComponent<Pieces>().getPieceWorth() + moveVal;
    //}

    //public float getKnightTable(int xPos, int yPos, bool isBlack)
    //{
    //    int newXPos = xPos;
    //    int newYPos = yPos;
    //    if (isBlack)
    //    {
    //        newXPos = 7 - newXPos;
    //        newYPos = 7 - newYPos;
    //    }

    //    float[,] knightTable =
    //        { {-5,-4,-3,-3,-3,-3,-4,-5 },
    //        {-4,-2,  0,  0,  0,  0,-2,-4 },
    //        {-3,  0, 1, 1.5f, 1.5f, 1,  0,-3 },
    //        {-3,  0.5f, 1.5f, 2, 2, 1.5f, 0.5f,-3 },
    //        {-3,  0, 1.5f, 2, 2, 1.5f,  0,-3 },
    //        {-3,  0.5f, 1, 1.5f, 1.5f, 1,  0.5f,-3 },
    //        {-4,-2,  0,  0.5f,  0.5f,  0,-2,-4 },
    //        {-5,-4,-3,-3,-3,-3,-4,-5 } };
    //    return knightTable[newXPos, newYPos];
    //}

    //public float getKingTable(int xPos, int yPos, bool isBlack)
    //{
    //    int newXPos = xPos;
    //    int newYPos = yPos;
    //    if (isBlack)
    //    {
    //        newXPos = 7 - newXPos;
    //        newYPos = 7 - newYPos;
    //    }
    //    float[,] kingTable =
    //        { {-3,-4,-4,-5,-5,-4,-4,-3 },
    //        {-3,-4,-4,-5,-5,-4,-4,-3 },
    //        {-3,-4,-4,-5,-5,-4,-4,-3 },
    //        {-3,-4,-4,-5,-5,-4,-4,-3 },
    //        {-2,-3,-3,-4,-4,-3,-3,-2 },
    //        {-1,-2,-2,-2,-2,-2,-2,-1 },
    //        {2, 2,  0,  0,  0,  0, 2, 2},
    //        {2, 3, 1,  0,  0, 1, 3, 2 } };
    //    return kingTable[newXPos, newYPos];
    //}

    //public float getQueenTable(int xPos, int yPos, bool isBlack)
    //{
    //    int newXPos = xPos;
    //    int newYPos = yPos;
    //    if (isBlack)
    //    {
    //        newXPos = 7 - newXPos;
    //        newYPos = 7 - newYPos;
    //    }
    //    float[,] queenTable =
    //        { {-2,-1,-1, -0.5f, -0.5f,-1,-1,-2 },
    //        {-1,  0,  0,  0,  0,  0,  0,-1 },
    //        {-1,  0,  0.5f,  0.5f,  0.5f,  0.5f,  0, -1 },
    //        {-0.5f,  0,  0.5f,  0.5f,  0.5f,  0.5f,  0, -0.5f },
    //        {0,  0,  0.5f,  0.5f,  0.5f,  0.5f,  0, -0.5f },
    //        {-1,  0.5f,  0.5f,  0.5f,  0.5f,  0.5f,  0,-1 },
    //        {-1,  0,  0.5f,  0,  0,  0,  0,-1 },
    //        {-2,-1,-1, -0.5f, -0.5f,-1,-1, -2 } };
    //    return queenTable[newXPos, newYPos];
    //}

    //public float getBishopTable(int xPos, int yPos, bool isBlack)
    //{
    //    int newXPos = xPos;
    //    int newYPos = yPos;
    //    if (isBlack)
    //    {
    //        newXPos = 7 - newXPos;
    //        newYPos = 7 - newYPos;
    //    }
    //    float[,] bishopTable =
    //        { {-2,-1,-1,-1,-1,-1,-1,-2 },
    //        {-1,  0,  0,  0,  0,  0,  0,-1 },
    //        {-1,  0,  0.5f, 1, 1,  0.5f,  0,-1 },
    //        {-1,  0.5f,  0.5f, 1, 1,  0.5f,  0.5f,-1 },
    //        {-1,  0, 1, 1, 1, 1,  0,-1 },
    //        {-1, 1, 1, 1, 1, 1, 1,-1 },
    //        {-1,  0.5f,  0,  0,  0,  0,  0.5f,-1 },
    //        {-2,-1,-1,-1,-1,-1,-1,-2 } };
    //    return bishopTable[newXPos, newYPos];
    //}

    //public float getRookTable(int xPos, int yPos, bool isBlack)
    //{
    //    int newXPos = xPos;
    //    int newYPos = yPos;
    //    if (isBlack)
    //    {
    //        newXPos = 7 - newXPos;
    //        newYPos = 7 - newYPos;
    //    }
    //    float[,] rookTable =
    //        {{0,  0,  0,  0,  0,  0,  0,  0 },
    //        {0.5f, 1, 1, 1, 1, 1, 1,  0.5f },
    //        {-0.5f,  0,  0,  0,  0,  0,  0, -0.5f },
    //        {-0.5f,  0,  0,  0,  0,  0,  0, -0.5f },
    //        {-0.5f,  0,  0,  0,  0,  0,  0, -0.5f },
    //        {-0.5f,  0,  0,  0,  0,  0,  0, -0.5f },
    //        {-0.5f,  0,  0,  0,  0,  0,  0, -0.5f },
    //        {0,  0,  0,  0.5f,  0.5f,  0,  0,  0 } };
    //    return rookTable[newXPos, newYPos];
    //}

    //public float getPawnTable(int xPos, int yPos, bool isBlack)
    //{
    //    int newXPos = xPos;
    //    int newYPos = yPos;
    //    if (isBlack)
    //    {
    //        newXPos = 7 - newXPos;
    //        newYPos = 7 - newYPos;
    //    }
    //    float[,] pawnTable =
    //        {{10, 10, 10, 10, 10, 10, 10, 10},
    //        {5, 5, 5, 5, 5, 5, 5, 5},
    //        {1, 1, 2, 3, 3, 2, 1, 1},
    //        {5, 5, 1, 2.5f, 2.5f, 1, 0.5f, 0.5f},
    //        {0, 0, 0, 2, 2, 0, 0, 0},
    //        {0.5f, -0.5f, -1, 0, 0, -1, -0.5f, 0.5f},
    //        {0.5f, 1, 1, -2, -2, 1, 1, 0.5f},
    //        {10, 10, 10, 10, 10, 10, 10, 10}};
    //    return pawnTable[newXPos, newYPos];
    //}




    //public void DestroyTiles()
    //{
    //    //Finds all the gameobjects withb the tag "MovePlate" and destroys them
    //    GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
    //    for (int i = 0; i < movePlates.Length; i++)
    //    {
    //        Destroy(movePlates[i]);
    //    }
    //}

}
