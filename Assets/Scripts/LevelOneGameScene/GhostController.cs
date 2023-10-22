using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    private UIManager uIManager;
    private PacCatController pacCatController;

    private GameObject ghost1;
    private GameObject ghost2;
    private GameObject ghost3;
    private GameObject ghost4;

    private bool isGhost1Learping = false;
    private bool isGhost2Learping = false;
    private bool isGhost3Learping = false;
    private bool isGhost4Learping = false;

    private Vector3 currentGhost1Direction;
    private Vector3 currentGhost2Direction;
    private Vector3 currentGhost3Direction;
    private Vector3 currentGhost4Direction;


    private Vector3 ghost1StartPosition;
    private Vector3 ghost1TargetPosition;
    private Vector3 ghost1SpawnPosition;
    private int[] ghost1SpawnGridPosition;

    private Vector3 ghost2StartPosition;
    private Vector3 ghost2TargetPosition;
    private Vector3 ghost2SpawnPosition;
    private int[] ghost2SpawnGridPosition;

    private Vector3 ghost3StartPosition;
    private Vector3 ghost3TargetPosition;
    private Vector3 ghost3SpawnPosition;
    private int[] ghost3SpawnGridPosition;

    private Vector3 ghost4StartPosition;
    private Vector3 ghost4TargetPosition;
    private Vector3 ghost4SpawnPosition;
    private int[] ghost4SpawnGridPosition;

    private bool isGhost1Dead = false;
    private bool isGhost2Dead = false;
    private bool isGhost3Dead = false;
    private bool isGhost4Dead = false;

    private int[,] mapArray;

    private LevelGenerator levelGenerator;
    private int ghost1CurrentCol;
    private int ghost1CurrentRow;
    private int ghost2CurrentCol;
    private int ghost2CurrentRow;
    private int ghost3CurrentCol;
    private int ghost3CurrentRow;
    private int ghost3PreviousCol;
    private int ghost3PreviousRow;
    private int ghost4CurrentCol;
    private int ghost4CurrentRow;

    private float stepLength = 0.32f;
    private float deltaTimeSpeed = 1.5f;

    private Animator ghost1Animator;
    private Animator ghost2Animator;
    private Animator ghost3Animator;
    private Animator ghost4Animator;

    private int[,] mapArrayEdgePoints;
    private int ghost4CurrentTargetEdgePointsIndex = 0;
    private bool startClockwiseMovement = false;
    private int[] nextCriticalPosition;

    private bool startLooking = false;

    private int[] topLeftExitGridPosition;
    private int[] topRightExitGridPosition;
    private int[] bottomLeftExitGridPosition;
    private int[] bottomRightExitGridPosition;

    private bool isGhost3GoingBackToExitFromLeftTeleport = false;
    private bool isGhost3GoingBackToExitFromRightTeleport = false;
    private int[] targetTeleportExitGridPosition;

    // Start is called before the first frame update
    void Start()
    {
        LoadObjectsOnStart();
        LoadGhosts();
        LoadMapInfo();
    }

    // Update is called once per frame
    void Update()
    {

        if (uIManager.CheckGameStarted() && !uIManager.CheckGameEnd())
        {
            if (!isGhost1Learping && !pacCatController.CheckPacCatDead() && !isGhost1Dead)
            {
                currentGhost1Direction = GenerateMovementForGhost1();
                SetGhost1MovingAnimation(currentGhost1Direction);
                MoveGhost1(currentGhost1Direction);
            }
            else if (isGhost1Dead)
            {

                RespawnGhost(ghost1);

            }

            if (!isGhost2Learping && !pacCatController.CheckPacCatDead() && !isGhost2Dead)
            {
                currentGhost2Direction = GenerateMovementForGhost2();
                SetGhost2MovingAnimation(currentGhost2Direction);
                MoveGhost2(currentGhost2Direction);
            }
            else if (isGhost2Dead)
            {
                RespawnGhost(ghost2);
            }

            if (!isGhost3Learping && !pacCatController.CheckPacCatDead() && !isGhost3Dead)
            {
                currentGhost3Direction = GenerateMovementForGhost3();
                SetGhost3MovingAnimation(currentGhost3Direction);
                MoveGhost3(currentGhost3Direction);
            }
            else if (isGhost3Dead)
            {
                RespawnGhost(ghost3);
            }

            if (!isGhost4Learping && !pacCatController.CheckPacCatDead() && !isGhost4Dead)
            {
                currentGhost4Direction = GenerateMovementForGhost4();
                SetGhost4MovingAnimation(currentGhost4Direction);
                MoveGhost4(currentGhost4Direction);
            }
            else if (isGhost4Dead)
            {
                RespawnGhost(ghost4);
            }
        }
    }

    private void LoadMapInfo()
    {
        mapArray = levelGenerator.mapArray;

        mapArrayEdgePoints = new int[4, 2]
        {
            {1, 1},
            {1, mapArray.GetLength(1) - 2},
            {mapArray.GetLength(0) - 2, mapArray.GetLength(1) - 2},
            {mapArray.GetLength(0) - 2,1}
        };

        topLeftExitGridPosition = new int[2] { -Mathf.RoundToInt(ghost3SpawnPosition.y / stepLength) - 2, (int)((ghost3SpawnPosition.x + ghost1SpawnPosition.x) / (2f * stepLength)) };
        topRightExitGridPosition = new int[2] { -Mathf.RoundToInt(ghost3SpawnPosition.y / stepLength) - 2, (int)((ghost3SpawnPosition.x + ghost1SpawnPosition.x) / (2f * stepLength)) + 1 };
        bottomLeftExitGridPosition = new int[2] { -Mathf.RoundToInt(ghost4SpawnPosition.y / stepLength) + 2, (int)((ghost4SpawnPosition.x + ghost2SpawnPosition.x) / (2f * stepLength)) };
        bottomRightExitGridPosition = new int[2] { -Mathf.RoundToInt(ghost4SpawnPosition.y / stepLength) + 2, (int)((ghost4SpawnPosition.x + ghost2SpawnPosition.x) / (2f * stepLength)) + 1 };

    }

    private void LoadObjectsOnStart()
    {
        GameObject uiManagerObj = GameObject.Find("Managers");
        if (uiManagerObj != null)
        {
            uIManager = uiManagerObj.GetComponent<UIManager>();
        }
        else
        {
            Debug.LogWarning("UIManager object not found.");
        }

        GameObject pacCatObj = GameObject.Find("PacCat");
        if (pacCatObj != null)
        {
            pacCatController = pacCatObj.GetComponent<PacCatController>();
        }
        else
        {
            Debug.LogWarning("PacCat object not found.");
        }

        GameObject generatedMapObject = GameObject.Find("GeneratedMap");
        if (generatedMapObject != null)
        {
            levelGenerator = generatedMapObject.GetComponent<LevelGenerator>();
        }
        else
        {
            Debug.LogWarning("GeneratedMap object not found.");
        }
    }

    private void LoadGhosts()
    {
        ghost1 = GameObject.Find("Ghost_1");
        ghost1Animator = ghost1.GetComponent<Animator>();
        ghost1StartPosition = ghost1.transform.position;
        ghost1SpawnPosition = ghost1.transform.position;
        ghost1CurrentCol = Mathf.RoundToInt(ghost1.transform.position.x / stepLength);
        ghost1CurrentRow = -Mathf.RoundToInt(ghost1.transform.position.y / stepLength);
        ghost1SpawnGridPosition = new int[2] { ghost1CurrentRow, ghost1CurrentCol };

        ghost2 = GameObject.Find("Ghost_2");
        ghost2Animator = ghost2.GetComponent<Animator>();
        ghost2StartPosition = ghost2.transform.position;
        ghost2SpawnPosition = ghost2.transform.position;
        ghost2CurrentCol = Mathf.RoundToInt(ghost2.transform.position.x / stepLength);
        ghost2CurrentRow = -Mathf.RoundToInt(ghost2.transform.position.y / stepLength);
        ghost2SpawnGridPosition = new int[2] { ghost2CurrentRow, ghost2CurrentCol };

        ghost3 = GameObject.Find("Ghost_3");
        ghost3Animator = ghost3.GetComponent<Animator>();
        ghost3StartPosition = ghost3.transform.position;
        ghost3SpawnPosition = ghost3.transform.position;
        ghost3CurrentCol = Mathf.RoundToInt(ghost3.transform.position.x / stepLength);
        ghost3CurrentRow = -Mathf.RoundToInt(ghost3.transform.position.y / stepLength);
        ghost3PreviousCol = ghost3CurrentCol;
        ghost3PreviousRow = ghost3CurrentRow;
        ghost3SpawnGridPosition = new int[2] { ghost3CurrentRow, ghost3CurrentCol };

        ghost4 = GameObject.Find("Ghost_4");
        ghost4Animator = ghost4.GetComponent<Animator>();
        ghost4StartPosition = ghost4.transform.position;
        ghost4SpawnPosition = ghost4.transform.position;
        ghost4CurrentCol = Mathf.RoundToInt(ghost4.transform.position.x / stepLength);
        ghost4CurrentRow = -Mathf.RoundToInt(ghost4.transform.position.y / stepLength);
        ghost4SpawnGridPosition = new int[2] { ghost4CurrentRow, ghost4CurrentCol };
    }

    private Vector3 GenerateMovementForGhost1()
    {
        float currentX = ghost1.transform.position.x;
        float currentY = ghost1.transform.position.y;
        int currentCol = Mathf.RoundToInt(currentX / stepLength);
        int currentRow = -Mathf.RoundToInt(currentY / stepLength);
        List<int[]> pathForGhost1 = CalculateShortestPathToTarget(mapArray, new int[] { ghost1CurrentRow, ghost1CurrentCol }, GetMirrorredGridPostionToCenter(pacCatController.GetCurrentMapGridPosition()));
        if (pathForGhost1.Count > 0)
        {
            int targetXD = pathForGhost1[0][1] - ghost1CurrentCol;
            int targetYD = pathForGhost1[0][0] - ghost1CurrentRow;
            return new Vector3(pathForGhost1[0][1] - ghost1CurrentCol, -pathForGhost1[0][0] + ghost1CurrentRow, 0);
        }
        return Vector3.zero;
    }

    private int[] GetMirrorredGridPostionToCenter(int[] currentPosition)
    {
        int rowNums = mapArray.GetLength(0);
        int colNums = mapArray.GetLength(1);
        return new int[] { rowNums - 1 - currentPosition[0], colNums - 1 - currentPosition[1] };
    }

    private void SetGhost1MovingAnimation(Vector3 currentGhost1Direction)
    {
        //Set a dummy value
        int directionValue = -1;

        //Direction value of animator parameters
        if (currentGhost1Direction == Vector3.left)
        {
            directionValue = 0;
        }
        else if (currentGhost1Direction == Vector3.up)
        {
            directionValue = 1;
        }
        else if (currentGhost1Direction == Vector3.right)
        {
            directionValue = 2;
        }
        else
        {
            directionValue = 3;
        }

        //Check if direction has been provided
        if (directionValue != -1)
        {
            ghost1Animator.SetInteger("Direction", directionValue);
        }
    }

    private void MoveGhost1(Vector3 currentGhost1Direction)
    {
        ghost1TargetPosition = ghost1StartPosition + currentGhost1Direction * stepLength;
        StartCoroutine(LerpGhost1(ghost1TargetPosition));
    }

    private void MoveGhost2(Vector3 currentGhost2Direction)
    {
        ghost2TargetPosition = ghost2StartPosition + currentGhost2Direction * stepLength;
        StartCoroutine(LerpGhost2(ghost2TargetPosition));
    }

    private void MoveGhost3(Vector3 currentGhost3Direction)
    {
        ghost3TargetPosition = ghost3StartPosition + currentGhost3Direction * stepLength;
        StartCoroutine(LerpGhost3(ghost3TargetPosition));
    }

    private void MoveGhost4(Vector3 currentGhost4Direction)
    {
        ghost4TargetPosition = ghost4StartPosition + currentGhost4Direction * stepLength;
        StartCoroutine(LerpGhost4(ghost4TargetPosition));
    }

    private IEnumerator LerpGhost1(Vector3 target)
    {
        Transform currentTransform;
        Vector3 startingPosition;
        isGhost1Learping = true;
        currentTransform = ghost1.transform;
        startingPosition = ghost1StartPosition;
        //Lerping
        float lerpLength;
        float distanceCovered;
        float lerpFraction;
        if (!isGhost1Dead)
        {
            distanceCovered = 0;
            lerpFraction = 0;
            lerpLength = Vector3.Distance(startingPosition, target);
            while (Vector3.Distance(currentTransform.position, target) > 0.01f)
            {
                if (isGhost1Dead) 
                {
                    break;
                }
                distanceCovered += Time.deltaTime * deltaTimeSpeed;
                lerpFraction = distanceCovered / lerpLength;
                currentTransform.position = Vector3.Lerp(startingPosition, target, lerpFraction);
                yield return null;
            }
            currentTransform.position = target;
            ghost1StartPosition = target;
        }
        else
        {
            distanceCovered = 0;
            lerpFraction = 0;
            lerpLength = Vector3.Distance(startingPosition, target);
            while (Vector3.Distance(currentTransform.position, target) > 0.01f)
            {
                distanceCovered += Time.deltaTime * deltaTimeSpeed;
                lerpFraction = distanceCovered / lerpLength;
                currentTransform.position = Vector3.Lerp(startingPosition, target, lerpFraction);
                yield return null;
            }
            currentTransform.position = target;
            ghost1StartPosition = target;
        }


        //Update current index of map array
        if (!isGhost1Dead)
        {
            if (currentGhost1Direction == Vector3.up)
            {
                ghost1CurrentRow--;
            }
            else if (currentGhost1Direction == Vector3.left)
            {
                ghost1CurrentCol--;
            }
            else if (currentGhost1Direction == Vector3.down)
            {
                ghost1CurrentRow++;
            }
            else if (currentGhost1Direction == Vector3.right)
            {
                ghost1CurrentCol++;
            }
        }


        isGhost1Learping = false;

    }

    private IEnumerator LerpGhost2(Vector3 target)
    {
        Transform currentTransform;
        Vector3 startingPosition;
        isGhost2Learping = true;
        currentTransform = ghost2.transform;
        startingPosition = ghost2StartPosition;

        //Lerping
        float lerpLength;
        float distanceCovered;
        float lerpFraction;
        if (!isGhost2Dead)
        {
            distanceCovered = 0;
            lerpFraction = 0;
            lerpLength = Vector3.Distance(startingPosition, target);
            while (Vector3.Distance(currentTransform.position, target) > 0.02f)
            {
                if (isGhost2Dead)
                {
                    break;
                }
                distanceCovered += Time.deltaTime * deltaTimeSpeed;
                lerpFraction = distanceCovered / lerpLength;
                currentTransform.position = Vector3.Lerp(startingPosition, target, lerpFraction);
                yield return null;
            }
            currentTransform.position = target;
            ghost2StartPosition = target;
        }
        else
        {
            distanceCovered = 0;
            lerpFraction = 0;
            lerpLength = Vector3.Distance(startingPosition, target);
            while (Vector3.Distance(currentTransform.position, target) > 0.02f)
            {
                distanceCovered += Time.deltaTime * deltaTimeSpeed;
                lerpFraction = distanceCovered / lerpLength;
                currentTransform.position = Vector3.Lerp(startingPosition, target, lerpFraction);
                yield return null;
            }
            currentTransform.position = target;
            ghost2StartPosition = target;
        }

        //Update current index of map array
        if (!isGhost2Dead)
        {
            if (currentGhost2Direction == Vector3.up)
            {
                ghost2CurrentRow--;
            }
            else if (currentGhost2Direction == Vector3.left)
            {
                ghost2CurrentCol--;
            }
            else if (currentGhost2Direction == Vector3.down)
            {
                ghost2CurrentRow++;
            }
            else if (currentGhost2Direction == Vector3.right)
            {
                ghost2CurrentCol++;
            }
        }
        isGhost2Learping = false;
    }

    private IEnumerator LerpGhost3(Vector3 target)
    {
        Transform currentTransform;
        Vector3 startingPosition;
        isGhost3Learping = true;
        currentTransform = ghost3.transform;
        startingPosition = ghost3StartPosition;

        float lerpLength;
        float distanceCovered;
        float lerpFraction;
        if (!isGhost3Dead)
        {
            distanceCovered = 0;
            lerpFraction = 0;
            lerpLength = Vector3.Distance(startingPosition, target);
            while (Vector3.Distance(currentTransform.position, target) > 0.01f)
            {
                if (isGhost3Dead)
                {
                    break;
                }
                distanceCovered += Time.deltaTime * deltaTimeSpeed;
                lerpFraction = distanceCovered / lerpLength;
                currentTransform.position = Vector3.Lerp(startingPosition, target, lerpFraction);
                yield return null;
            }
            currentTransform.position = target;
            ghost3StartPosition = target;
        }
        else
        {
            distanceCovered = 0;
            lerpFraction = 0;
            lerpLength = Vector3.Distance(startingPosition, target);
            while (Vector3.Distance(currentTransform.position, target) > 0.01f)
            {
                distanceCovered += Time.deltaTime * deltaTimeSpeed;
                lerpFraction = distanceCovered / lerpLength;
                currentTransform.position = Vector3.Lerp(startingPosition, target, lerpFraction);
                yield return null;
            }
            currentTransform.position = target;
            ghost3StartPosition = target;
        }

        //Update current index of map array
        if (!isGhost3Dead)
        {
            ghost3PreviousCol = ghost3CurrentCol;
            ghost3PreviousRow = ghost3CurrentRow;
            if (currentGhost3Direction == Vector3.up)
            {
                ghost3CurrentRow--;
            }
            else if (currentGhost3Direction == Vector3.left)
            {
                ghost3CurrentCol--;
            }
            else if (currentGhost3Direction == Vector3.down)
            {
                ghost3CurrentRow++;
            }
            else if (currentGhost3Direction == Vector3.right)
            {
                ghost3CurrentCol++;
            }
        }
        isGhost3Learping = false;
    }

    private IEnumerator LerpGhost4(Vector3 target)
    {
        Transform currentTransform;
        Vector3 startingPosition;
        isGhost4Learping = true;
        currentTransform = ghost4.transform;
        startingPosition = ghost4StartPosition;

        float lerpLength;
        float distanceCovered;
        float lerpFraction;
        if (!isGhost4Dead)
        {
            distanceCovered = 0;
            lerpFraction = 0;
            lerpLength = Vector3.Distance(startingPosition, target);
            while (Vector3.Distance(currentTransform.position, target) > 0.01f)
            {
                if (isGhost4Dead)
                {
                    break;
                }
                distanceCovered += Time.deltaTime * deltaTimeSpeed;
                lerpFraction = distanceCovered / lerpLength;
                currentTransform.position = Vector3.Lerp(startingPosition, target, lerpFraction);
                yield return null;
            }
            currentTransform.position = target;
            ghost4StartPosition = target;
        }
        else
        {
            distanceCovered = 0;
            lerpFraction = 0;
            lerpLength = Vector3.Distance(startingPosition, target);
            while (Vector3.Distance(currentTransform.position, target) > 0.01f)
            {
                distanceCovered += Time.deltaTime * deltaTimeSpeed;
                lerpFraction = distanceCovered / lerpLength;
                currentTransform.position = Vector3.Lerp(startingPosition, target, lerpFraction);
                yield return null;
            }
            currentTransform.position = target;
            ghost4StartPosition = target;
        }

        //Update current index of map array
        if (!isGhost4Dead)
        {
            if (currentGhost4Direction == Vector3.up)
            {
                ghost4CurrentRow--;
            }
            else if (currentGhost4Direction == Vector3.left)
            {
                ghost4CurrentCol--;
            }
            else if (currentGhost4Direction == Vector3.down)
            {
                ghost4CurrentRow++;
            }
            else if (currentGhost4Direction == Vector3.right)
            {
                ghost4CurrentCol++;
            }
        }
        isGhost4Learping = false;
    }



    //Find shortest path using BFS Algorithms
    public List<int[]> CalculateShortestPathToTarget(int[,] mapGrid, int[] start, int[] target)
    {
        Queue<int[]> queue = new Queue<int[]>();
        Dictionary<int[], int[]> lastPosition = new Dictionary<int[], int[]>();
        bool[,] mapGridVisitStatus = new bool[mapGrid.GetLength(0), mapGrid.GetLength(1)];

        queue.Enqueue(start);
        mapGridVisitStatus[start[0], start[1]] = true;

        while (queue.Count > 0)
        {
            int[] current = queue.Dequeue();
            if (current[0] == target[0] && current[1] == target[1])
            {
                List<int[]> path = new List<int[]>();
                int[] backtrack = current;

                while (backtrack[0] != start[0] || backtrack[1] != start[1])
                {
                    path.Add(backtrack);
                    backtrack = lastPosition[backtrack];
                }
                path.Reverse();
                return path;
            }

            List<int[]> neighbors = GetNeighbors(current, mapGrid);

            foreach (int[] neighbor in neighbors)
            {
                if (!mapGridVisitStatus[neighbor[0], neighbor[1]] && (mapGrid[neighbor[0], neighbor[1]] == 0 || mapGrid[neighbor[0], neighbor[1]] == 5 || mapGrid[neighbor[0], neighbor[1]] == 6))
                {
                    if (!IsInSpawnArea(start) && IsInSpawnArea(neighbor))
                    {
                        continue;
                    }
                    queue.Enqueue(neighbor);
                    mapGridVisitStatus[neighbor[0], neighbor[1]] = true;
                    lastPosition[neighbor] = current;
                }
            }
        }
        return new List<int[]>();
    }

    private int[] FindNearestExit(int[] objectGridPosition)
    {
        float distanceToTopLeftExit = Mathf.Sqrt(Mathf.Pow(objectGridPosition[0] - topLeftExitGridPosition[0], 2) + Mathf.Pow(objectGridPosition[1] - topLeftExitGridPosition[1], 2));
        float distanceToTopRightExit = Mathf.Sqrt(Mathf.Pow(objectGridPosition[0] - topRightExitGridPosition[0], 2) + Mathf.Pow(objectGridPosition[1] - topRightExitGridPosition[1], 2));
        float distanceToBottomLeftExit = Mathf.Sqrt(Mathf.Pow(objectGridPosition[0] - bottomLeftExitGridPosition[0], 2) + Mathf.Pow(objectGridPosition[1] - bottomLeftExitGridPosition[1], 2));
        float distanceToBottomRightExit = Mathf.Sqrt(Mathf.Pow(objectGridPosition[0] - bottomRightExitGridPosition[0], 2) + Mathf.Pow(objectGridPosition[1] - bottomRightExitGridPosition[1], 2));

        float minDistance = Mathf.Min(distanceToTopLeftExit, Mathf.Min(distanceToTopRightExit, Mathf.Min(distanceToBottomLeftExit, distanceToBottomRightExit)));

        int[] nearestExit = null;

        if (minDistance == distanceToTopLeftExit)
        {
            nearestExit = topLeftExitGridPosition;
        }
        else if (minDistance == distanceToTopRightExit)
        {
            nearestExit = topRightExitGridPosition;
        }
        else if (minDistance == distanceToBottomLeftExit)
        {
            nearestExit = bottomLeftExitGridPosition;

        }
        else
        {
            nearestExit = bottomRightExitGridPosition;
        }

        return nearestExit;

    }


    List<int[]> GetNeighbors(int[] position, int[,] grid)
    {
        List<int[]> neighbors = new List<int[]>();

        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };

        for (int i = 0; i < dx.Length; i++)
        {
            int x = position[0] + dx[i];
            int y = position[1] + dy[i];

            if (x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1))
            {
                neighbors.Add(new int[] { x, y });
            }
        }

        return neighbors;
    }

    public bool IsInSpawnArea(int[] currentGridPosition)
    {
        if (
            currentGridPosition[1] >= Mathf.RoundToInt(ghost3SpawnPosition.x / stepLength) &&
            currentGridPosition[1] <= Mathf.RoundToInt(ghost1SpawnPosition.x / stepLength) &&
            currentGridPosition[0] >= -Mathf.RoundToInt((ghost3SpawnPosition.y + stepLength) / stepLength) &&
            currentGridPosition[0] <= -Mathf.RoundToInt((ghost4SpawnPosition.y - stepLength) / stepLength)
            )
        {
            return true;
        }
        return false;
    }

    private Vector3 GenerateMovementForGhost2()
    {
        int[] ghost2CurrentGridPosition = new int[2] { ghost2CurrentRow, ghost2CurrentCol };
        int[] ghost2TargetGridPosition;
        if (IsInSpawnArea(ghost2CurrentGridPosition))
        {
            ghost2TargetGridPosition = FindNearestExit(ghost2CurrentGridPosition);
        }
        else if (ghost3Animator.GetBool("isRecovering") || ghost3Animator.GetBool("isScared"))
        {
            ghost2TargetGridPosition = GetMirrorredGridPostionToCenter(pacCatController.GetCurrentMapGridPosition());
        }
        else
        {
            ghost2TargetGridPosition = pacCatController.GetCurrentMapGridPosition();
        }

        List<int[]> pathForGhost2 = CalculateShortestPathToTarget(mapArray, ghost2CurrentGridPosition, ghost2TargetGridPosition);
        if (pathForGhost2.Count > 0)
        {
            int targetXD = pathForGhost2[0][1] - ghost2CurrentCol;
            int targetYD = pathForGhost2[0][0] - ghost2CurrentRow;
            return new Vector3(pathForGhost2[0][1] - ghost2CurrentCol, -pathForGhost2[0][0] + ghost2CurrentRow, 0);
        }
        return Vector3.zero;
    }

    private void SetGhost2MovingAnimation(Vector3 currentGhost2Direction)
    {
        //Set a dummy value
        int directionValue = -1;

        //Direction value of animator parameters
        if (currentGhost2Direction == Vector3.left)
        {
            directionValue = 0;
        }
        else if (currentGhost2Direction == Vector3.up)
        {
            directionValue = 1;
        }
        else if (currentGhost2Direction == Vector3.right)
        {
            directionValue = 2;
        }
        else
        {
            directionValue = 3;
        }

        //Check if direction has been provided
        if (directionValue != -1)
        {
            ghost2Animator.SetInteger("Direction", directionValue);
        }
    }

    private Vector3 GenerateMovementForGhost3()
    {

        int[] ghost3CurrentGridPosition = new int[2] { ghost3CurrentRow, ghost3CurrentCol };
        int[] ghost3TargetGridPosition;
        if (IsInSpawnArea(ghost3CurrentGridPosition) || (ghost3Animator.GetBool("isRecovering") || ghost3Animator.GetBool("isScared")) || IsAtLeftTeleport(ghost3CurrentGridPosition) || IsAtRightTeleport(ghost3CurrentGridPosition) || isGhost3GoingBackToExitFromLeftTeleport || isGhost3GoingBackToExitFromRightTeleport)
        {
            if (IsInSpawnArea(ghost3CurrentGridPosition))
            {
                ghost3TargetGridPosition = FindNearestExit(ghost3CurrentGridPosition);
            }
            else if (IsAtLeftTeleport(ghost3CurrentGridPosition) && !isGhost3GoingBackToExitFromLeftTeleport)
            {
                int teleportExitDistance = 0;
                for (int i = 1; i < mapArray.GetLength(1); i++)
                {
                    if (mapArray[ghost3CurrentRow + 1, i - 1] == 1)
                    {
                        teleportExitDistance = i;
                        break;
                    }
                }
                targetTeleportExitGridPosition = new int[2] { ghost3CurrentRow, ghost3CurrentCol + teleportExitDistance };
                ghost3TargetGridPosition = targetTeleportExitGridPosition;
                isGhost3GoingBackToExitFromLeftTeleport = true;
            }
            else if (isGhost3GoingBackToExitFromLeftTeleport)
            {
                if (ghost3CurrentGridPosition[0] == targetTeleportExitGridPosition[0] && ghost3CurrentGridPosition[1] == targetTeleportExitGridPosition[1])
                {
                    isGhost3GoingBackToExitFromLeftTeleport = false;
                }
                ghost3TargetGridPosition = targetTeleportExitGridPosition;

            }
            else if (IsAtRightTeleport(ghost3CurrentGridPosition) && !isGhost3GoingBackToExitFromRightTeleport)
            {
                int teleportExitDistance = 0;
                for (int i = 1; i < mapArray.GetLength(1); i++)
                {
                    if (mapArray[ghost3CurrentRow + 1, mapArray.GetLength(1) - i] == 1)
                    {
                        teleportExitDistance = i;
                        break;
                    }
                }
                targetTeleportExitGridPosition = new int[2] { ghost3CurrentRow, ghost3CurrentCol - teleportExitDistance };
                ghost3TargetGridPosition = targetTeleportExitGridPosition;
                isGhost3GoingBackToExitFromRightTeleport = true;
            }
            else if (isGhost3GoingBackToExitFromRightTeleport)
            {
                if (ghost3CurrentGridPosition[0] == targetTeleportExitGridPosition[0] && ghost3CurrentGridPosition[1] == targetTeleportExitGridPosition[1])
                {
                    isGhost3GoingBackToExitFromRightTeleport = false;
                }
                ghost3TargetGridPosition = targetTeleportExitGridPosition;

            }
            else
            {
                ghost3TargetGridPosition = GetMirrorredGridPostionToCenter(pacCatController.GetCurrentMapGridPosition());
            }

            List<int[]> pathForGhost3 = CalculateShortestPathToTarget(mapArray, ghost3CurrentGridPosition, ghost3TargetGridPosition);
            if (pathForGhost3.Count > 0)
            {
                int targetXD = pathForGhost3[0][1] - ghost3CurrentCol;
                int targetYD = pathForGhost3[0][0] - ghost3CurrentRow;
                return new Vector3(pathForGhost3[0][1] - ghost3CurrentCol, -pathForGhost3[0][0] + ghost3CurrentRow, 0);
            }
        }


        Vector3 nextDirection = GetRandomDirection();
        int[] nextTarget = GetNextTargetGridPosition(nextDirection);

        while ((mapArray[nextTarget[0], nextTarget[1]] != 0 && mapArray[nextTarget[0], nextTarget[1]] != 5 && mapArray[nextTarget[0], nextTarget[1]] != 6) || (nextTarget[0] == ghost3PreviousRow && nextTarget[1] == ghost3PreviousCol))
        {
            if (nextTarget[0] == ghost3PreviousRow && nextTarget[1] == ghost3PreviousCol)
            {
                int upNeighborGridValue = mapArray[ghost3CurrentRow - 1, ghost3CurrentCol];
                int downNeighborGridValue = mapArray[ghost3CurrentRow + 1, ghost3CurrentCol];
                int leftNeighborGridValue = mapArray[ghost3CurrentRow, ghost3CurrentCol - 1];
                int rightNeighborGridValue = mapArray[ghost3CurrentRow, ghost3CurrentCol + 1];

                int invalidDirectionCount = 0;

                if (!CheckGridValueValidity(upNeighborGridValue))
                {
                    invalidDirectionCount++;
                }
                if (!CheckGridValueValidity(downNeighborGridValue))
                {
                    invalidDirectionCount++;
                }
                if (!CheckGridValueValidity(leftNeighborGridValue))
                {
                    invalidDirectionCount++;
                }
                if (!CheckGridValueValidity(rightNeighborGridValue))
                {
                    invalidDirectionCount++;
                }

                if (invalidDirectionCount == 3)
                {
                    return nextDirection;
                }

            }
            nextDirection = GetRandomDirection();
            nextTarget = GetNextTargetGridPosition(nextDirection);
        }
        return nextDirection;
    }

    private bool IsAtLeftTeleport(int[] currentGridPosition)
    {
        if (currentGridPosition[1] == 0)
        {
            return true;
        }
        return false;
    }

    private bool IsAtRightTeleport(int[] currentGridPosition)
    {
        if (currentGridPosition[1] == (mapArray.GetLength(1) - 1))
        {
            return true;
        }
        return false;
    }

    private bool CheckGridValueValidity(int val)
    {
        if (val == 5 || val == 6 || val == 0)
        {
            return true;
        }
        return false;
    }

    private void SetGhost3MovingAnimation(Vector3 currentGhost3Direction)
    {
        {
            //Set a dummy value
            int directionValue = -1;

            //Direction value of animator parameters
            if (currentGhost3Direction == Vector3.left)
            {
                directionValue = 0;
            }
            else if (currentGhost3Direction == Vector3.up)
            {
                directionValue = 1;
            }
            else if (currentGhost3Direction == Vector3.right)
            {
                directionValue = 2;
            }
            else
            {
                directionValue = 3;
            }

            //Check if direction has been provided
            if (directionValue != -1)
            {
                ghost3Animator.SetInteger("Direction", directionValue);
            }
        }
    }

    private Vector3 GetRandomDirection()
    {
        int randomInt = Random.Range(0, 4);
        Vector3 direction = Vector3.zero;
        switch (randomInt)
        {
            case 0: // Up
                direction = Vector3.up;
                break;
            case 1: // Down
                direction = Vector3.down;
                break;
            case 2: // Left
                direction = Vector3.left;
                break;
            case 3: // Right
                direction = Vector3.right;
                break;
        }

        return direction;
    }

    private int[] GetNextTargetGridPosition(Vector3 nextDirection)
    {
        Vector3 currentPositionVar = new Vector3(ghost3CurrentCol, ghost3CurrentRow, 0);
        Vector3 nextGridDirection = new Vector3(nextDirection.x, -nextDirection.y, 0);
        Vector3 nextPositionVar = currentPositionVar + nextGridDirection;
        return new int[2] { Mathf.RoundToInt(nextPositionVar.y), Mathf.RoundToInt(nextPositionVar.x) };
    }

    private Vector3 GenerateMovementForGhost4()
    {
        //Clockwise leftbottom -> top left -> top right -> right bottom

        int[] ghost4CurrentGridPosition = new int[2] { ghost4CurrentRow, ghost4CurrentCol };
        int[] ghost4TargetGridPosition;
        if (IsInSpawnArea(ghost4CurrentGridPosition) || (ghost4Animator.GetBool("isRecovering") || ghost4Animator.GetBool("isScared")))
        {
            if (IsInSpawnArea(ghost4CurrentGridPosition))
            {
                ghost4TargetGridPosition = FindNearestExit(ghost4CurrentGridPosition);
            }
            else
            {
                ghost4TargetGridPosition = GetMirrorredGridPostionToCenter(pacCatController.GetCurrentMapGridPosition());
            }
            List<int[]> pathForGhost4 = CalculateShortestPathToTarget(mapArray, ghost4CurrentGridPosition, ghost4TargetGridPosition);
            if (pathForGhost4.Count > 0)
            {
                int targetXD = pathForGhost4[0][1] - this.ghost4CurrentCol;
                int targetYD = pathForGhost4[0][0] - this.ghost4CurrentRow;
                return new Vector3(pathForGhost4[0][1] - this.ghost4CurrentCol, -pathForGhost4[0][0] + this.ghost4CurrentRow, 0);
            }
        }
        else
        {
            ghost4TargetGridPosition = new int[2] { mapArrayEdgePoints[ghost4CurrentTargetEdgePointsIndex, 0], mapArrayEdgePoints[ghost4CurrentTargetEdgePointsIndex, 1] };

            if (ghost4TargetGridPosition[0] == this.ghost4CurrentRow && ghost4TargetGridPosition[1] == this.ghost4CurrentCol)
            {
                if (!startClockwiseMovement)
                {
                    startClockwiseMovement = true;
                }
                ghost4CurrentTargetEdgePointsIndex = (ghost4CurrentTargetEdgePointsIndex + 1) % 4;
                ghost4TargetGridPosition = new int[2] { mapArrayEdgePoints[ghost4CurrentTargetEdgePointsIndex, 0], mapArrayEdgePoints[ghost4CurrentTargetEdgePointsIndex, 1] };
            }

            if (!startClockwiseMovement)
            {
                nextCriticalPosition = ghost4TargetGridPosition;
            }
            else
            {
                if (ghost4CurrentRow == nextCriticalPosition[0] && ghost4CurrentCol == nextCriticalPosition[1])
                {
                    nextCriticalPosition = LookForNextCriticalPosition(ghost4TargetGridPosition);
                }
            }
            List<int[]> pathForGhost4 = CalculateShortestPathToTarget(mapArray, new int[] { this.ghost4CurrentRow, this.ghost4CurrentCol }, nextCriticalPosition);
            if (pathForGhost4.Count > 0)
            {
                int targetXD = pathForGhost4[0][1] - this.ghost4CurrentCol;
                int targetYD = pathForGhost4[0][0] - this.ghost4CurrentRow;
                return new Vector3(pathForGhost4[0][1] - this.ghost4CurrentCol, -pathForGhost4[0][0] + this.ghost4CurrentRow, 0);
            }
        }

        return Vector3.zero;
    }

    private int[] LookForNextCriticalPosition(int[] currentTargetEdgePointPosition)
    {
        int rowStartOffset = 0;
        int colStartOffset = 0;
        if (ghost4CurrentCol == 1 && ghost4CurrentRow != 1)
        {
            rowStartOffset = -1;
            if (startLooking)
            {
                rowStartOffset = -2;
            }
        }
        if ((ghost4CurrentCol == mapArray.GetLength(1) - 2) && (ghost4CurrentRow != mapArray.GetLength(0) - 2))
        {
            rowStartOffset = 1;
            if (startLooking)
            {
                rowStartOffset = 2;
            }

        }
        if ((ghost4CurrentRow == 1) && (ghost4CurrentCol != mapArray.GetLength(1) - 2))
        {
            colStartOffset = 1;
            if (startLooking)
            {
                colStartOffset = 2;
            }

        }
        if ((ghost4CurrentRow == mapArray.GetLength(0) - 2) && (ghost4CurrentCol != 1))
        {
            colStartOffset = -1;
            if (startLooking)
            {
                colStartOffset = -2;
            }
        }

        if (ghost4CurrentRow == 1 || ghost4CurrentCol == mapArray.GetLength(1) - 2)
        {
            for (int row = ghost4CurrentRow + rowStartOffset; row <= currentTargetEdgePointPosition[0]; row++)
            {
                for (int col = ghost4CurrentCol + colStartOffset; col <= currentTargetEdgePointPosition[1]; col++)
                {
                    if (mapArray[row, col] != 5 && mapArray[row, col] != 6 && mapArray[row, col] != 0 && !startLooking)
                    {
                        startLooking = true;

                        if (row == 1)
                        {

                            return new int[2] { row, col - 1 };
                        }
                        return new int[2] { row - 1, col };
                    }
                    if (startLooking && (mapArray[row, col] != 5 && mapArray[row, col] != 6 && mapArray[row, col] != 0))
                    {
                        startLooking = false;

                        if (row == 1)
                        {

                            return new int[2] { row, col + 1 };
                        }
                        return new int[2] { row + 1, col };
                    }
                }
            }
        }

        if (ghost4CurrentRow == mapArray.GetLength(0) - 2 || ghost4CurrentCol == 1)
        {
            for (int row = ghost4CurrentRow + rowStartOffset; row >= currentTargetEdgePointPosition[0]; row--)
            {
                for (int col = ghost4CurrentCol + colStartOffset; col >= currentTargetEdgePointPosition[1]; col--)
                {
                    if (mapArray[row, col] != 5 && mapArray[row, col] != 6 && mapArray[row, col] != 0 && !startLooking)
                    {
                        startLooking = true;
                        if (col == 1)
                        {
                            return new int[2] { row + 1, col };
                        }

                        return new int[2] { row, col + 1 };
                    }
                    if (startLooking && (mapArray[row, col] != 5 && mapArray[row, col] != 6 && mapArray[row, col] != 0))
                    {

                        startLooking = false;

                        if (col == 1)
                        {
                            return new int[2] { row - 1, col };
                        }
                        return new int[2] { row, col - 1 };

                    }
                }
            }
        }
        return new int[2] { currentTargetEdgePointPosition[0], currentTargetEdgePointPosition[1] };
    }

    private void SetGhost4MovingAnimation(Vector3 currentGhost4Direction)
    {
        //Set a dummy value
        int directionValue = -1;

        //Direction value of animator parameters
        if (currentGhost4Direction == Vector3.left)
        {
            directionValue = 0;
        }
        else if (currentGhost4Direction == Vector3.up)
        {
            directionValue = 1;
        }
        else if (currentGhost4Direction == Vector3.right)
        {
            directionValue = 2;
        }
        else
        {
            directionValue = 3;
        }

        //Check if direction has been provided
        if (directionValue != -1)
        {
            ghost4Animator.SetInteger("Direction", directionValue);
        }
    }


    public void RespawnGhost(GameObject ghost)
    {
        if (ghost == ghost1)
        {
            StartCoroutine(LerpGhost1(ghost1SpawnPosition));
            ghost1CurrentCol = ghost1SpawnGridPosition[1];
            ghost1CurrentRow = ghost1SpawnGridPosition[0];
        }

        else if (ghost == ghost2)
        {
            StartCoroutine(LerpGhost2(ghost2SpawnPosition));
            ghost2CurrentCol = ghost2SpawnGridPosition[1];
            ghost2CurrentRow = ghost2SpawnGridPosition[0];
        }
        else if (ghost == ghost3)
        {
            StartCoroutine(LerpGhost3(ghost3SpawnPosition));
            ghost3CurrentCol = ghost3SpawnGridPosition[1];
            ghost3CurrentRow = ghost3SpawnGridPosition[0];
        }
        else if (ghost == ghost4)
        {
            StartCoroutine(LerpGhost4(ghost4SpawnPosition));
            ghost4CurrentCol = ghost4SpawnGridPosition[1];
            ghost4CurrentRow = ghost4SpawnGridPosition[0];
        }
    }

    public void SetGhostDeadState(GameObject ghost, bool state)
    {
        if (ghost == ghost1)
        {
            isGhost1Dead = state;
        }
        else if (ghost == ghost2)
        {
            isGhost2Dead = state;
        }
        else if (ghost == ghost3)
        {
            isGhost3Dead = state;
        }
        else if (ghost == ghost4)
        {
            isGhost4Dead = state;
        }
    }
}
