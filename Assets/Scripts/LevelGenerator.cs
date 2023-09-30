using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    // [0] - Outside Corner
    // [1] - Outside Wall
    // [2] - Inside Corner
    // [3] - Inside Wall
    // [4] - Standard Pallet
    // [5] - Power Pallet
    // [6] - Junction Piece
    private string manualMap = "ManualMap";
    public GameObject[] mapPieces;
    public Transform levelParent;
    private int[,] levelMap =
    {
        {1,2,2,2,2,2,2,2,2,2,2,2,2,7},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,4},
        {2,6,4,0,0,4,5,4,0,0,0,4,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,3},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,5},
        {2,5,3,4,4,3,5,3,3,5,3,4,4,4},
        {2,5,3,4,4,3,5,4,4,5,3,4,4,3},
        {2,5,5,5,5,5,5,4,4,5,5,5,5,4},
        {1,2,2,2,2,1,5,4,3,4,4,3,0,4},
        {0,0,0,0,0,2,5,4,3,4,4,3,0,3},
        {0,0,0,0,0,2,5,4,4,0,0,0,0,0},
        {0,0,0,0,0,2,5,4,4,0,3,4,4,0},
        {2,2,2,2,2,1,5,3,3,0,4,0,0,0},
        {0,0,0,0,0,0,5,0,0,0,4,0,0,0},
    };


    // Start is called before the first frame update
    void Start()
    {
        Destroy(GameObject.Find(manualMap));
        GenerateLevel();
    }

    void GenerateLevel()
    {
        int numRows = levelMap.GetLength(0);
        int numCols = levelMap.GetLength(1);

        //generate top left quadrant
        for (int i = 0; i < numRows; i++)
        {
            for (int j = 0; j < numCols; j++)
            {
                int pieceType = levelMap[i, j] - 1;
                if (pieceType >= 0)
                {
                    Vector3 position = new Vector3(j * 0.32f, -i * 0.32f, 0);
                    GameObject piece = Instantiate(mapPieces[pieceType], position, Quaternion.identity);
                    if (pieceType <= 3 || pieceType == 6)
                    {
                        TransformToCorrectAngle(levelMap, piece, pieceType, i, j);
                    }
                    piece.transform.parent = levelParent;
                }
            }
        }

        //mirror the map horizontally
        int[,] rightQuadrant = new int[numRows, numCols];
        for (int i = 0; i < numRows; i++)
        {
            for (int j = 0; j < numCols; j++)
            {
                rightQuadrant[i, j] = levelMap[i, numCols - 1 - j];
            }
        }

        //generate top right quadrant
        for (int i = 0; i < numRows; i++)
        {
            for (int j = 0; j < numCols; j++)
            {
                int pieceType = rightQuadrant[i, j] - 1;
                if (pieceType >= 0)
                {
                    Vector3 position = new Vector3((j + numCols) * 0.32f, -i * 0.32f, 0);
                    GameObject piece = Instantiate(mapPieces[pieceType], position, Quaternion.identity);
                    if (pieceType <= 3 || pieceType == 6)
                    {
                        TransformToCorrectAngle(rightQuadrant, piece, pieceType, i, j);
                    }
                    piece.transform.parent = levelParent;
                }
            }
        }

        //mirror the map vertically
        int[,] bottomLeftQuadrant = new int[numRows - 1, numCols];
        for (int i = 0; i < numRows - 1; i++)
        {
            for (int j = 0; j < numCols; j++)
            {
                bottomLeftQuadrant[i, j] = levelMap[numRows - 2 - i, j];
            }
        }

        //generate bottom left quadrant
        for (int i = 0; i < numRows - 1; i++)
        {
            for (int j = 0; j < numCols; j++)
            {
                int pieceType = bottomLeftQuadrant[i, j] - 1;
                if (pieceType >= 0)
                {
                    Vector3 position = new Vector3(j * 0.32f, -(i + numRows) * 0.32f, 0);
                    GameObject piece = Instantiate(mapPieces[pieceType], position, Quaternion.identity);
                    if (pieceType <= 3 || pieceType == 6)
                    {
                        TransformToCorrectAngle(bottomLeftQuadrant, piece, pieceType, i, j);
                    }
                    piece.transform.parent = levelParent;
                }
            }
        }

        //mirror the map vertically and horizontally
        int[,] bottomRightQuadrant = new int[numRows - 1, numCols];
        for (int i = 0; i < numRows - 1; i++)
        {
            for (int j = 0; j < numCols; j++)
            {
                bottomRightQuadrant[i, j] = levelMap[numRows - 2 - i, numCols - 1 - j];
            }
        }

        //generate bottom left quadrant
        for (int i = 0; i < numRows - 1; i++)
        {
            for (int j = 0; j < numCols; j++)
            {
                int pieceType = bottomRightQuadrant[i, j] - 1;
                if (pieceType >= 0)
                {
                    Vector3 position = new Vector3((numCols + j) * 0.32f, -(i + numRows) * 0.32f, 0);
                    GameObject piece = Instantiate(mapPieces[pieceType], position, Quaternion.identity);
                    if (pieceType <= 3 || pieceType == 6)
                    {
                        TransformToCorrectAngle(bottomRightQuadrant, piece, pieceType, i, j);
                    }
                    piece.transform.parent = levelParent;
                }
            }
        }
    }

    void TransformToCorrectAngle(int[,] currentMap, GameObject piece, int pieceType, int currentRow, int currentCol)
    {
        // Piece type:
        // [0] - Outside Corner
        // [1] - Outside Wall
        // [2] - Inside Corner
        // [3] - Inside Wall
        // [4] - Standard Pallet
        // [5] - Power Pallet
        // [6] - Junction Piece
        float rotationAngle = 0f;
        int leftCol = currentCol - 1;
        int rightCol = currentCol + 1;
        int upRow = currentRow - 1;
        int downRow = currentRow + 1;
        int rowLen = currentMap.GetLength(0);
        int colLen = currentMap.GetLength(1);
        switch (pieceType)
        {
            //Outside Corner
            case 0:
                if(currentCol < colLen - 1 && currentRow > 0 && currentMap[currentRow,rightCol] == 2 && currentMap[upRow,currentCol] == 2){
                    rotationAngle = 270f;
                }else if(currentCol < colLen - 1 && currentRow < rowLen - 1 && currentMap[currentRow,rightCol] == 2 && currentMap[downRow,currentCol] == 2){
                    rotationAngle = 180f;
                }else if(currentCol > 0 && currentRow < rowLen - 1 && currentMap[currentRow,leftCol] == 2 && currentMap[downRow,currentCol] == 2){
                    rotationAngle = 90f;
                }
                break;
            //Outside Wall
            case 1:
                if (currentCol > 0)
                {
                    if (currentMap[currentRow, leftCol] == 1 || currentMap[currentRow, leftCol] == 2 || currentMap[currentRow, leftCol] == 7)
                    {
                        if (currentRow < rowLen - 1 && (currentMap[downRow, currentCol] == 5 || currentMap[downRow, currentCol] == 6))
                        {
                            rotationAngle = 270f;
                            break;
                        }
                        rotationAngle = 90f;
                    }
                }
                else
                {
                    if (currentMap[currentRow, rightCol] == 1 || currentMap[currentRow, rightCol] == 2 || currentMap[currentRow, rightCol] == 7)
                    {
                        if (currentRow < rowLen - 1 && (currentMap[downRow, currentCol] == 5 || currentMap[downRow, currentCol] == 6))
                        {
                            rotationAngle = 270f;
                            break;
                        }
                        rotationAngle = 90f;
                    }
                }
                break;
            //Inside Corner
            case 2:
                if (currentCol < colLen - 1 && currentRow > 0 && (currentMap[currentRow, rightCol] == 4 || currentMap[currentRow, rightCol] == 3) && (currentMap[upRow, currentCol] == 4 || currentMap[upRow, currentCol] == 3))
                {
                    if (currentMap[upRow, rightCol] == 5 || currentMap[upRow, rightCol] == 6 || currentMap[upRow, rightCol] == 0)
                    {
                        rotationAngle = 270f;
                        break;
                    }
                    else if (currentCol > 0 && currentRow < rowLen - 1 && (currentMap[downRow, leftCol] == 5 || currentMap[downRow, leftCol] == 6 || currentMap[downRow, leftCol] == 0))
                    {
                        if ((currentMap[currentRow, leftCol] == 4 || currentMap[currentRow, leftCol] == 3) && (currentMap[downRow, currentCol] == 4 || currentMap[downRow, currentCol] == 3) && (currentMap[upRow, rightCol] == 4 || currentMap[upRow, rightCol] == 3))
                        {
                            rotationAngle = 90f;
                            break;
                        }
                        rotationAngle = 270f;
                        break;
                    }
                    else if (currentRow < rowLen - 1 && currentCol < colLen - 1 && (currentMap[downRow, rightCol] == 0 || currentMap[downRow, rightCol] == 5 || currentMap[downRow, rightCol] == 6))
                    {
                        rotationAngle = 180f;
                        break;
                    }
                }
                else if (currentCol < colLen - 1 && currentRow < rowLen - 1 && (currentMap[currentRow, rightCol] == 4 || currentMap[currentRow, rightCol] == 3) && (currentMap[downRow, currentCol] == 4 || currentMap[downRow, currentCol] == 3))
                {
                    if (currentRow == 0)
                    {
                        rotationAngle = 180f;
                        break;
                    }
                    if (currentMap[downRow, rightCol] == 3)
                    {
                        break;
                    }
                    if (currentMap[downRow, rightCol] == 5 || currentMap[downRow, rightCol] == 6 || currentMap[downRow, rightCol] == 0)
                    {
                        rotationAngle = 180f;
                        break;
                    }
                    else if (currentCol > 0 && currentRow > 0 && (currentMap[upRow, leftCol] == 5 || currentMap[upRow, leftCol] == 6 || currentMap[upRow, leftCol] == 0))
                    {
                        rotationAngle = 180f;
                        break;
                    }
                }
                else if (currentCol > 0 && currentRow < rowLen - 1 && (currentMap[currentRow, leftCol] == 4 || currentMap[currentRow, leftCol] == 3) && (currentMap[downRow, currentCol] == 4 || currentMap[downRow, currentCol] == 3))
                {
                    if (currentRow == 0)
                    {
                        rotationAngle = 90f;
                        break;
                    }
                    if (currentMap[downRow, leftCol] == 3)
                    {
                        break;
                    }
                    if (currentMap[downRow, leftCol] == 5 || currentMap[downRow, leftCol] == 6 || currentMap[downRow, leftCol] == 0)
                    {
                        rotationAngle = 90f;
                        break;
                    }
                    else if (currentCol < colLen - 1 && currentRow > 0 && (currentMap[upRow, rightCol] == 5 || currentMap[upRow, rightCol] == 6 || currentMap[upRow, rightCol] == 0))
                    {
                        rotationAngle = 90f;
                        break;
                    }
                }
                else if (currentCol == colLen - 1 && currentRow < rowLen - 1 && ((currentMap[currentRow, leftCol] == 5 || currentMap[currentRow, leftCol] == 6 || currentMap[currentRow, leftCol] == 0) && (currentMap[downRow, currentCol] == 5 || currentMap[downRow, currentCol] == 6 || currentMap[downRow, currentCol] == 0)))
                {
                    rotationAngle = 270f;
                    break;
                }
                else if (currentCol == colLen - 1 && currentRow > 0 && ((currentMap[currentRow, leftCol] == 5 || currentMap[currentRow, leftCol] == 6 || currentMap[currentRow, leftCol] == 0) && (currentMap[upRow, currentCol] == 5 || currentMap[upRow, currentCol] == 6 || currentMap[upRow, currentCol] == 0)))
                {
                    rotationAngle = 180f;
                    break;
                }
                else if (currentCol == 0 && currentRow > 0 && ((currentMap[currentRow, rightCol] == 5 || currentMap[currentRow, rightCol] == 6 || currentMap[currentRow, rightCol] == 0) && (currentMap[upRow, currentCol] == 5 || currentMap[upRow, currentCol] == 6 || currentMap[upRow, currentCol] == 0)))
                {
                    rotationAngle = 90f;
                    break;
                }
                break;
                // Inside Wall
            case 3:
                if (currentRow > 0 && (currentMap[upRow, currentCol] == 5 || currentMap[upRow, currentCol] == 6 || currentMap[upRow, currentCol] == 0))
                {
                    if (currentRow < rowLen - 1 && (currentMap[downRow, currentCol] == 5 || currentMap[downRow, currentCol] == 6))
                    {
                        rotationAngle = 270f;
                        break;
                    }
                    rotationAngle = 90f;
                    break;
                }
                else if (currentRow < rowLen - 1 && (currentMap[downRow, currentCol] == 5 || currentMap[downRow, currentCol] == 6 || currentMap[downRow, currentCol] == 0))
                {
                    rotationAngle = 270f;
                    break;
                }
                else if (currentCol > 0 && (currentMap[currentRow, leftCol] == 5 || currentMap[currentRow, leftCol] == 6 || currentMap[currentRow, leftCol] == 0))
                {
                    if (currentCol < colLen - 1 && (currentMap[currentRow, rightCol] == 5 || currentMap[currentRow, rightCol] == 6))
                    {
                        break;
                    }
                    rotationAngle = 180f;
                    break;
                }
                break;
                // T Junction
            case 6:
                if (currentRow < rowLen - 1 && currentCol > 0 && currentMap[currentRow, leftCol] == 2 && currentMap[downRow, currentCol] == 4)
                {
                    rotationAngle = 270f;
                    break;
                }
                else if (currentRow < rowLen - 1 && currentCol < colLen - 1 && currentMap[downRow, currentCol] == 2 && currentMap[currentRow, rightCol] == 4)
                {
                    rotationAngle = 0f;
                    break;
                }
                else if (currentRow > 0 && currentCol < colLen - 1 && currentMap[currentRow, rightCol] == 2 && currentMap[upRow, currentCol] == 4)
                {
                    rotationAngle = 90f;
                    break;
                }
                else if (currentRow > 0 && currentCol > 0 && currentMap[upRow, currentCol] == 2 && currentMap[currentRow, leftCol] == 4)
                {
                    rotationAngle = 180f;
                    break;
                }
                else if (currentRow < rowLen - 1 && currentCol > 0 && currentMap[currentRow, leftCol] == 4 && currentMap[downRow, currentCol] == 2)
                {
                    rotationAngle = 0f;
                }
                else if (currentRow < rowLen - 1 && currentCol < colLen - 1 && currentMap[currentRow, rightCol] == 2 && currentMap[downRow, currentCol] == 4)
                {
                    rotationAngle = 90f;
                }
                else if (currentRow > 0 && currentCol < colLen - 1 && currentMap[currentRow, rightCol] == 4 && currentMap[upRow, currentCol] == 4)
                {
                    rotationAngle = 180f;
                }
                else if (currentRow > 0 && currentCol > 0 && currentMap[currentRow, leftCol] == 2 && currentMap[upRow, currentCol] == 4)
                {
                    rotationAngle = 270f;
                }
                Vector3 newScale = piece.transform.localScale;
                newScale.x *= -1;
                piece.transform.localScale = newScale;
                break;
            default:
                break;
        }
        piece.transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle);
    }
}
