using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusCanControllerInnovationScene : MonoBehaviour
{
    [SerializeField]
    private GameObject bonusCanPrefab;
    private float spawnInterval = 10.0f;
    private float bonusCanSpeed = 5.0f;

    private float spawnTimer;
    private LevelGenerator levelGenerator;
    private float deltaTimeSpeed = 0.5f;
    [SerializeField]
    private Camera mainCamera;
    private float camH;
    private float camW;
    private GameObject bonusCan;

    // Start is called before the first frame update

    void Start()
    {
        camH = mainCamera.orthographicSize;
        camW = camH * mainCamera.aspect;

        GameObject generatedMapObject = GameObject.Find("GeneratedMap");
        if (generatedMapObject != null)
        {
            levelGenerator = generatedMapObject.GetComponent<LevelGenerator>();
        }
        else
        {
            Debug.LogWarning("GeneratedMap object not found.");
        }

        spawnTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            SpawnBonusCan();
            spawnTimer = 0;
        }
    }

    void SpawnBonusCan()
    {
        Vector3 spawnPos = GetRandomSpawnPosition();

        bonusCan = Instantiate(bonusCanPrefab, spawnPos, Quaternion.identity);

        Vector3 camCenter = new Vector3(4.48f, -4.48f, 0);
        Vector3 tarPos = camCenter - (spawnPos - camCenter);
        if (bonusCan != null)
        {
            bonusCan.transform.position = spawnPos;
            float distance = Vector3.Distance(spawnPos, tarPos);
            float duration = distance / bonusCanSpeed;

            StartCoroutine(MoveBonusCan(bonusCan.transform, tarPos, duration));
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        float cameraAspect = Camera.main.aspect;
        float cameraX = 4.48f;
        float cameraY = -4.48f;
        float minX = cameraX - camW;
        float maxX = cameraX + camW;
        float minY = cameraY - camH;
        float maxY = cameraY + camH;

        int appearSide = Random.Range(0, 4);

        Vector3 spawnPosition = Vector3.zero;

        switch (appearSide)
        {
            case 0: // Top
                spawnPosition = new Vector3(Random.Range(minX, maxX), maxY + 0.32f, 0);
                break;
            case 1: // Right
                spawnPosition = new Vector3(maxX + 0.32f, Random.Range(minY, maxY), 0);
                break;
            case 2: // Bottom
                spawnPosition = new Vector3(Random.Range(minX, maxX), minY - 0.32f, 0);
                break;
            case 3: // Left
                spawnPosition = new Vector3(minX - 0.32f, Random.Range(minY, maxY), 0);
                break;
        }

        return spawnPosition;
    }

    IEnumerator MoveBonusCan(Transform bonusCanTransform, Vector3 targetPosition, float duration)
    {
        float elapsedTime = 0;
        Vector3 startingPosition = bonusCanTransform.position;
        Transform transformToCheckEmpty = bonusCanTransform;
        while (transformToCheckEmpty != null && elapsedTime < duration)
        {
            bonusCanTransform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime * deltaTimeSpeed;
            yield return null;
        }
        if (transformToCheckEmpty != null)
        {
            Destroy(bonusCanTransform.gameObject);
        }
    }
}
