using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacCatController : MonoBehaviour
{
    private Vector3 targetPosition;
    private Vector3 lastInputDirection = Vector3.zero;
    private Vector3 currentInputDirection = Vector3.zero;
    private float speed = 1.5f;
    private bool isLearping = false;
    private float stepLength = 0.32f;
    private Vector3 startingPosition;
    private LevelGenerator levelGenerator;
    private int currentCol = 1;
    private int currentRow = 1;

    [SerializeField]
    private AudioSource walkingAudioSource;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private AudioSource eatingAudioSource;
    // levelGenerator.mapArray
    [SerializeField]
    private ParticleSystem dustParticles;

    void Start()
    {
        GameObject generatedMapObject = GameObject.Find("GeneratedMap");
        if (generatedMapObject != null)
        {
            levelGenerator = generatedMapObject.GetComponent<LevelGenerator>();
        }
        else
        {
            Debug.LogWarning("GeneratedMap object not found.");
        }
        startingPosition = transform.position;
    }

    void Update()
    {
        HandleInput();

        if (!isLearping)
        {
            if (CheckMoveValidity(lastInputDirection))
            {
                currentInputDirection = lastInputDirection;
            }
            else
            {
                lastInputDirection = currentInputDirection;
            }
            MovePacCat(currentInputDirection);
        }

    }

    void HandleInput()
    {
        if (Input.GetKey(KeyCode.W) && lastInputDirection != Vector3.up)
        {
            lastInputDirection = Vector3.up;
        }
        else if (Input.GetKey(KeyCode.A) && lastInputDirection != Vector3.left)
        {
            lastInputDirection = Vector3.left;
        }
        else if (Input.GetKey(KeyCode.S) && lastInputDirection != Vector3.down)
        {
            lastInputDirection = Vector3.down;
        }
        else if (Input.GetKey(KeyCode.D) && lastInputDirection != Vector3.right)
        {
            lastInputDirection = Vector3.right;
        }
    }

    void MovePacCat(Vector3 direction)
    {
        if (CheckMoveValidity(direction))
        {
            targetPosition = startingPosition + direction * stepLength;
            StartCoroutine(LerpPacCat(targetPosition));
        }
    }

    IEnumerator LerpPacCat(Vector3 target)
    {
        isLearping = true;
        walkingAudioSource.Play();
        dustParticles.Play();
        //eatingAudioSource.Play();
        float lerpLength = stepLength;
        float distanceCoverted = 0;
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            distanceCoverted += Time.deltaTime * speed;
            float lerpFraction = distanceCoverted / lerpLength;
            transform.position = Vector3.Lerp(startingPosition, target, lerpFraction);
            yield return null;
        }
        transform.position = target;
        startingPosition = target;
        if (currentInputDirection == Vector3.up)
        {
            currentRow--;
        }
        else if (currentInputDirection == Vector3.left)
        {
            currentCol--;
        }
        else if (currentInputDirection == Vector3.down)
        {
            currentRow++;
        }
        else if (currentInputDirection == Vector3.right)
        {
            currentCol++;
        }
        isLearping = false;
        walkingAudioSource.Stop();
        dustParticles.Stop();
        //eatingAudioSource.Stop();
    }

    private bool CheckMoveValidity(Vector3 direction)
    {
        int targetCol = -1;
        int targetRow = -1;
        if (direction == Vector3.up)
        {
            targetCol = currentCol;
            targetRow = currentRow - 1;
        }
        else if (direction == Vector3.left)
        {
            targetCol = currentCol - 1;
            targetRow = currentRow;
        }
        else if (direction == Vector3.down)
        {
            targetCol = currentCol;
            targetRow = currentRow + 1;
        }
        else if (direction == Vector3.right)
        {
            targetCol = currentCol + 1;
            targetRow = currentRow;
        }
        return CheckMovable(targetCol,targetRow);
    }

    private bool CheckMovable(int targetCol, int targetRow)
    {
        if (targetCol <= 0 || targetRow <= 0 || targetCol == levelGenerator.newCol || targetRow == levelGenerator.newRow)
        {
            return false;
        }
        else
        {
            int valueToCheck = levelGenerator.mapArray[targetRow,targetCol];
            if (valueToCheck == 5 || valueToCheck == 6 || valueToCheck == 0)
            {
                return true;
            }
            return false;
        }
    }
}
