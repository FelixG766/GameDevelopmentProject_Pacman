using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BorderMovement : MonoBehaviour
{
    // Moving along an eclipse shaped path, the volley ball is rotating at the same time
    private float a = 1.5f;
    private float b = 5f;
    private float speed = 1f;
    private float t;
    public int borderSetIndx = 0;
    private float adjustment = (float)(Math.PI / 8.0);
    private float rotationSpeed = 60.0f;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 currentPosition = transform.position;
        t = (float)(Mathf.Atan2((float)((currentPosition.y - 1.5) / a), (float)(currentPosition.x / b))) + adjustment * borderSetIndx;
        float x = (float)(b * Mathf.Cos(t));
        float y = (float)(a * Mathf.Sin(t) + 1.5);
        float z = 0;
        transform.position = new Vector3(x, y, z);
    }

    // Update is called once per frame
    void Update()
    {
        // Increment the parameter 't' to make the object move.
        t += Time.deltaTime * speed;
        // Calculate the new position based on the parametric equations.
        float x = (float)(b * Mathf.Cos(t));
        float y = (float)(a * Mathf.Sin(t) + 1.5);
        float z = 0;

        // Update the object's position.
        transform.position = new Vector3(x, y, z);
        if (borderSetIndx != 3) 
        {
            RotateObjects();
        }
    }

    void RotateObjects()
    {
        float rotationAngle = rotationSpeed * Time.deltaTime;
        transform.Rotate(-Vector3.forward, rotationAngle);
    }
}
