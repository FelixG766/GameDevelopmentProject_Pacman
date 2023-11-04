using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneratorInnovationScene : MonoBehaviour
{
    // [PieceType]  - Tile Type             - [LevelMapVal]
    // [0]          - Outside Corner         - [1]
    // [1]          - Outside Wall           - [2]
    // [2]          - Inside Corner          - [3]
    // [3]          - Inside Wall            - [4]
    // [4]          - Standard Pallet        - [5]
    // [5]          - Power Pallet           - [6]
    // [6]          - Junction Piece         - [7]
    private string manualMap = "ManualMap";
    public GameObject[] mapPieces;
    public Transform levelParent;
    private int[,] levelMap =
    // Original Case
    {
        {1,2,2,2,2,2,2,2,2,2,2,2,2,7},
        {2,0,0,0,0,0,0,0,0,0,0,0,0,4},
        {2,0,3,4,4,3,0,3,4,4,4,3,0,4},
        {2,6,4,0,0,4,0,4,0,0,0,4,0,4},
        {2,0,3,4,4,3,0,3,4,4,4,3,0,3},
        {2,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {2,0,3,4,4,3,0,3,3,0,3,4,4,4},
        {2,0,3,4,4,3,0,4,4,0,3,4,4,3},
        {2,0,0,0,0,0,0,4,4,0,0,0,0,4},
        {1,2,2,2,2,1,0,4,3,4,4,3,0,4},
        {0,0,0,0,0,2,0,4,3,4,4,3,0,3},
        {0,0,0,0,0,2,0,4,4,0,0,0,0,0},
        {0,0,0,0,0,2,0,4,4,0,3,4,4,0},
        {2,2,2,2,2,1,0,3,3,0,4,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,4,0,0,0},
    };

    // Test Case 1
    //    {
    //    {1,2,2,2,2,2,2,2,2,2,2,2,2,2,7},
    //    {2,5,5,5,5,5,5,5,5,5,5,5,5,5,4},
    //    {2,5,3,4,4,4,3,5,3,4,4,4,3,5,4},
    //    {2,6,4,0,0,0,4,5,4,0,0,0,4,5,4},
    //    {2,5,3,4,3,3,3,5,3,4,4,4,3,5,3},
    //    {2,5,5,5,4,4,5,5,5,5,5,5,5,5,5},
    //    {2,5,3,4,3,3,3,5,3,4,3,5,3,4,4},
    //    {2,5,3,4,4,4,3,5,4,0,4,5,3,4,3},
    //    {2,5,5,5,5,5,5,5,4,0,4,5,5,5,4},
    //    {1,2,2,2,2,2,1,5,4,0,3,4,3,0,4},
    //    {0,0,0,0,0,0,2,5,4,3,4,4,3,0,3},
    //    {0,0,0,0,0,0,2,5,4,4,0,0,0,0,0},
    //    {0,0,0,0,0,0,2,5,4,4,0,3,4,4,0},
    //    {2,2,2,2,2,2,1,5,3,3,0,4,0,0,0},
    //    {0,0,0,0,0,0,0,5,0,0,0,4,0,0,0},
    //};

    // Test Case 2
    //    {
    //    {1,2,2,2,2,2,2,2,2,2,2,2,2,2,7},
    //    {2,5,5,5,5,5,5,5,5,5,5,5,5,5,4},
    //    {2,5,3,4,4,4,3,5,3,4,4,4,3,5,4},
    //    {2,6,4,0,0,0,4,5,4,0,0,0,4,5,4},
    //    {2,5,3,4,3,3,3,5,3,4,4,4,3,5,3},
    //    {2,5,5,5,4,4,5,5,5,5,5,5,5,5,5},
    //    {7,4,4,4,3,3,3,5,3,4,3,5,3,4,4},
    //    {7,4,4,4,4,4,3,5,4,0,4,5,3,4,3},
    //    {2,5,5,5,5,5,5,5,4,0,4,5,5,5,4},
    //    {1,2,2,2,2,2,1,5,4,0,3,4,3,0,4},
    //    {0,0,0,0,0,0,7,4,3,3,4,4,3,0,3},
    //    {0,0,0,0,0,0,7,4,3,4,0,0,0,0,0},
    //    {0,0,0,0,0,0,2,5,4,4,0,3,4,4,0},
    //    {2,2,2,2,2,2,1,5,3,3,0,4,0,0,0},
    //    {0,0,0,0,0,0,0,5,0,0,0,4,0,0,0},
    //};


    public int newCol;
    public int newRow;
    public int[,] mapArray;
    public int numRows;
    public int numCols;

    void Awake()
    {
        numRows = levelMap.GetLength(0);
        numCols = levelMap.GetLength(1);
        newCol = 2 * levelMap.GetLength(0) - 1;
        newRow = 2 * levelMap.GetLength(1);
        mapArray = new int[newCol, newRow];
        GenerateMapArray();
        Destroy(GameObject.Find(manualMap));
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateLevel();
    }

    void GenerateMapArray()
    {
        // Copy the original array to the new array
        for (int i = 0; i < numRows; i++)
        {
            for (int j = 0; j < numCols; j++)
            {
                mapArray[i, j] = levelMap[i, j];
            }
        }

        // Horizontal mirroring of the original array
        for (int i = 0; i < numRows; i++)
        {
            for (int j = numCols; j < 2 * numCols; j++)
            {
                mapArray[i, j] = levelMap[i, 2 * numCols - j - 1];
            }
        }

        // Vertical mirroring of the original array without the last row
        for (int i = numRows; i < 2 * numRows - 1; i++)
        {
            for (int j = 0; j < numCols; j++)
            {
                mapArray[i, j] = levelMap[2 * numRows - i - 2, j];
            }
        }

        // Horizontal and vertical mirroring of the original array without the last row
        for (int i = numRows; i < 2 * numRows - 1; i++)
        {
            for (int j = numCols; j < 2 * numCols; j++)
            {
                mapArray[i, j] = levelMap[2 * numRows - i - 2, 2 * numCols - j - 1];
            }
        }
    }

    void GenerateLevel()
    {
        numRows = levelMap.GetLength(0);
        numCols = levelMap.GetLength(1);

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

        // [PieceType]  - Tile Type             - [LevelMapVal]
        // [0]          - Outside Corner         - [1]
        // [1]          - Outside Wall           - [2]
        // [2]          - Inside Corner          - [3]
        // [3]          - Inside Wall            - [4]
        // [4]          - Standard Pallet        - [5]
        // [5]          - Power Pallet           - [6]
        // [6]          - Junction Piece         - [7]

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
                if (currentCol < colLen - 1 && currentRow > 0 && (currentMap[currentRow, rightCol] == 2 || currentMap[currentRow, rightCol] == 7) && (currentMap[upRow, currentCol] == 2 || currentMap[upRow, currentCol] == 7))
                {
                    rotationAngle = 270f;
                }
                else if (currentCol < colLen - 1 && currentRow < rowLen - 1 && (currentMap[currentRow, rightCol] == 2 || currentMap[currentRow, rightCol] == 7) && (currentMap[downRow, currentCol] == 2 || currentMap[downRow, currentCol] == 7))
                {
                    rotationAngle = 180f;
                }
                else if (currentCol > 0 && currentRow < rowLen - 1 && (currentMap[currentRow, leftCol] == 2 || currentMap[currentRow, leftCol] == 7) && (currentMap[downRow, currentCol] == 2 || currentMap[downRow, currentCol] == 7))
                {
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
                    else if (currentMap[currentRow, leftCol] == 5 || currentMap[currentRow, leftCol] == 6)
                    {
                        rotationAngle = 180f;
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
                // Check outside surrounding tiles for normal
                if (currentRow > 0 && currentRow < rowLen - 1 && currentCol > 0 && (currentMap[currentRow, leftCol] == 5 || currentMap[currentRow, leftCol] == 6 || currentMap[currentRow, leftCol] == 0) && (currentMap[downRow, leftCol] == 5 || currentMap[downRow, leftCol] == 6 || currentMap[downRow, leftCol] == 0) && (currentMap[downRow, currentCol] == 5 || currentMap[downRow, currentCol] == 6 || currentMap[downRow, currentCol] == 0))
                {
                    rotationAngle = 270f;
                    break;
                }
                else if (currentRow > 0 && currentCol > 0 && (currentMap[currentRow, leftCol] == 5 || currentMap[currentRow, leftCol] == 6 || currentMap[currentRow, leftCol] == 0) && (currentMap[upRow, leftCol] == 5 || currentMap[upRow, leftCol] == 6 || currentMap[upRow, leftCol] == 0) && (currentMap[upRow, currentCol] == 5 || currentMap[upRow, currentCol] == 6 || currentMap[upRow, currentCol] == 0))
                {
                    rotationAngle = 180f;
                    break;
                }
                else if (currentRow > 0 && currentCol < colLen - 1 && (currentMap[currentRow, rightCol] == 5 || currentMap[currentRow, rightCol] == 6 || currentMap[currentRow, rightCol] == 0) && (currentMap[upRow, rightCol] == 5 || currentMap[upRow, rightCol] == 6 || currentMap[upRow, rightCol] == 0) && (currentMap[upRow, currentCol] == 5 || currentMap[upRow, currentCol] == 6 || currentMap[upRow, currentCol] == 0))
                {
                    rotationAngle = 90f;
                    break;
                }
                else if (currentRow > 0 && currentRow < rowLen - 1 && currentCol < colLen - 1 && (currentMap[currentRow, rightCol] == 5 || currentMap[currentRow, rightCol] == 6 || currentMap[currentRow, rightCol] == 0) && (currentMap[downRow, rightCol] == 5 || currentMap[downRow, rightCol] == 6 || currentMap[downRow, rightCol] == 0) && (currentMap[downRow, currentCol] == 5 || currentMap[downRow, currentCol] == 6 || currentMap[downRow, currentCol] == 0))
                {
                    break;
                }
                // Check outsie surrounding tiles for opposite angles
                else if (currentRow > 1 && currentCol < colLen - 2 && (currentMap[upRow, rightCol] == 5 || currentMap[upRow, rightCol] == 6 || currentMap[upRow, rightCol] == 0) && (currentMap[upRow - 1, rightCol] == 5 || currentMap[upRow - 1, rightCol] == 6 || currentMap[upRow - 1, rightCol] == 0) && (currentMap[upRow, rightCol + 1] == 5 || currentMap[upRow, rightCol + 1] == 6 || currentMap[upRow, rightCol + 1] == 0))
                {
                    rotationAngle = 270f;
                    break;
                }
                else if (currentRow > 0 && currentRow < rowLen - 2 && currentCol < colLen - 2 && (currentMap[downRow, rightCol] == 5 || currentMap[downRow, rightCol] == 6 || currentMap[downRow, rightCol] == 0) && (currentMap[downRow + 1, rightCol] == 5 || currentMap[downRow + 1, rightCol] == 6 || currentMap[downRow + 1, rightCol] == 0) && (currentMap[downRow, rightCol + 1] == 5 || currentMap[downRow, rightCol + 1] == 6 || currentMap[downRow, rightCol + 1] == 0))
                {
                    rotationAngle = 180f;
                    break;
                }
                else if (currentRow > 0 && currentRow < rowLen - 2 && currentCol > 1 && (currentMap[downRow, leftCol] == 5 || currentMap[downRow, leftCol] == 6 || currentMap[downRow, leftCol] == 0) && (currentMap[downRow + 1, leftCol] == 5 || currentMap[downRow + 1, leftCol] == 6 || currentMap[downRow + 1, leftCol] == 0) && (currentMap[downRow, leftCol - 1] == 5 || currentMap[downRow, leftCol - 1] == 6 || currentMap[downRow, leftCol - 1] == 0))
                {
                    rotationAngle = 90f;
                    break;
                }
                else if (currentRow > 1 && currentCol > 1 && (currentMap[upRow, leftCol] == 5 || currentMap[upRow, leftCol] == 6 || currentMap[upRow, leftCol] == 0) && (currentMap[upRow - 1, leftCol] == 5 || currentMap[upRow - 1, leftCol] == 6 || currentMap[upRow - 1, leftCol] == 0) && (currentMap[upRow, leftCol - 1] == 5 || currentMap[upRow, leftCol - 1] == 6 || currentMap[upRow, leftCol - 1] == 0))
                {
                    break;
                }
                // Check narrow angles
                else if (currentRow > 0 && currentCol < colLen - 1 && (currentMap[upRow, rightCol] == 5 || currentMap[upRow, rightCol] == 6))
                {
                    rotationAngle = 270f;
                    break;
                }
                else if (currentRow > 0 && currentRow < rowLen - 1 && currentCol < colLen - 1 && (currentMap[downRow, rightCol] == 5 || currentMap[downRow, rightCol] == 6))
                {
                    rotationAngle = 180f;
                    break;
                }
                else if (currentRow > 0 && currentRow < rowLen - 1 && currentCol > 0 && (currentMap[downRow, leftCol] == 5 || currentMap[downRow, leftCol] == 6))
                {
                    rotationAngle = 90f;
                    break;
                }
                else if (currentRow > 0 && currentCol > 0 && (currentMap[upRow, leftCol] == 5 || currentMap[upRow, leftCol] == 6))
                {
                    break;
                }
                // Check edge cases
                else if (currentRow == 0)
                {
                    if (currentCol > 0 && (currentMap[currentRow, leftCol] == 5 || currentMap[currentRow, leftCol] == 6 || currentMap[currentRow, leftCol] == 0))
                    {
                        rotationAngle = 180f;
                        break;
                    }
                    else
                    {
                        rotationAngle = 90f;
                        break;
                    }
                }
                // Check edge cases
                else if (currentRow == rowLen - 1)
                {
                    if (currentCol > 0 && (currentMap[currentRow, leftCol] == 5 || currentMap[currentRow, leftCol] == 6 || currentMap[currentRow, leftCol] == 0))
                    {
                        rotationAngle = 270f;
                        break;
                    }
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
                if (currentRow < rowLen - 1 && currentCol > 0 && (currentMap[currentRow, leftCol] == 2 || currentMap[currentRow, leftCol] == 1) && currentMap[downRow, currentCol] == 4)
                {
                    rotationAngle = 270f;
                    break;
                }
                else if (currentRow < rowLen - 1 && currentCol < colLen - 1 && (currentMap[downRow, currentCol] == 1 || currentMap[downRow, currentCol] == 2) && currentMap[currentRow, rightCol] == 4)
                {
                    rotationAngle = 0f;
                    break;
                }
                else if (currentRow > 0 && currentCol < colLen - 1 && (currentMap[currentRow, rightCol] == 1 || currentMap[currentRow, rightCol] == 2) && currentMap[upRow, currentCol] == 4)
                {
                    rotationAngle = 90f;
                    break;
                }
                else if (currentRow > 0 && currentCol > 0 && (currentMap[upRow, currentCol] == 2 || currentMap[upRow, currentCol] == 1) && currentMap[currentRow, leftCol] == 4)
                {
                    rotationAngle = 180f;
                    break;
                }
                else if (currentRow < rowLen - 1 && currentCol > 0 && currentMap[currentRow, leftCol] == 4 && (currentMap[downRow, currentCol] == 2 || currentMap[downRow, currentCol] == 1))
                {
                    rotationAngle = 0f;
                }
                else if (currentRow < rowLen - 1 && currentCol < colLen - 1 && (currentMap[currentRow, rightCol] == 2 || currentMap[currentRow, rightCol] == 1) && currentMap[downRow, currentCol] == 4)
                {
                    rotationAngle = 90f;
                }
                else if (currentRow > 0 && currentCol < colLen - 1 && currentMap[currentRow, rightCol] == 4 && (currentMap[upRow, currentCol] == 2 || currentMap[upRow, currentCol] == 1))
                {
                    rotationAngle = 180f;
                }
                else if (currentRow > 0 && currentCol > 0 && (currentMap[currentRow, leftCol] == 2 || currentMap[currentRow, leftCol] == 1) && currentMap[upRow, currentCol] == 4)
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

   public void GenerateTerritory(int row, int col)
    {
        Vector3 position = new Vector3(col * 0.32f, -row * 0.32f, 0);
        GameObject piece = Instantiate(mapPieces[4], position, Quaternion.identity);
        piece.transform.parent = levelParent;
        mapArray[row, col] = 5;
    }
}
