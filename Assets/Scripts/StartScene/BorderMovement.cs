using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BorderMovement : MonoBehaviour
{
    // Moving along an eclipse shaped path, the volley ball is rotating at the same time
    private float a;
    private float b;
    private float speed = 1f;
    private float t;
    public int borderSetIndx = 0;
    private float adjustment = (float)(Math.PI / 8.0);
    private float rotationSpeed = 60.0f;
    private float centerX;
    private float centerY;


    // Start is called before the first frame update
    void Start()
    {
        GameObject bottom = GameObject.Find("Border");
        GameObject left = GameObject.Find("Border1");
        GameObject right = GameObject.Find("Border2");
        GameObject top = GameObject.Find("Border3");

        GameObject title = GameObject.Find("Title");
        GameObject subtitle = GameObject.Find("Subtitle");
        GameObject orangeGhost = GameObject.Find("OrangeGhost");
       

        centerX = title.transform.position.x;
        centerY = (title.transform.position.y + subtitle.transform.position.y)/2;
        a = (centerY - orangeGhost.transform.position.y) * 0.7f;
        b = a * 3f;

        //a = (top.transform.position.y - bottom.transform.position.y) / 2;
        //b = (right.transform.position.x - left.transform.position.x) / 2;
        //centerX = (left.transform.position.x + right.transform.position.x) / 2;
        //centerY = (top.transform.position.y + bottom.transform.position.y) / 2;


        Vector3 currentPosition = transform.position;
        Debug.Log(gameObject.name + "Border: " + currentPosition.y + "    " + currentPosition.x + "Center X: " + centerX);
        t = (float)(Mathf.Atan2((float)((currentPosition.y - centerY) / a), (float)((currentPosition.x - centerX)/ b))) + adjustment * borderSetIndx;
        float x = (float)(b * Mathf.Cos(t) + centerX);
        float y = (float)(a * Mathf.Sin(t) + centerY);
        float z = 0;
        transform.position = new Vector3(x, y, z);

    }

    // Update is called once per frame
    void Update()
    {
        GameObject title = GameObject.Find("Title");
        GameObject subtitle = GameObject.Find("Subtitle");
        GameObject orangeGhost = GameObject.Find("OrangeGhost");

        centerX = title.transform.position.x;
        centerY = (title.transform.position.y + subtitle.transform.position.y) / 2;
        a = (centerY - orangeGhost.transform.position.y) * 0.6f;
        b = a * 3f;

        // Increment the parameter 't' to make the object move.
        t += Time.deltaTime * speed;
        // Calculate the new position based on the parametric equations.
        float x = (float)(b * Mathf.Cos(t) + centerX);
        float y = (float)(a * Mathf.Sin(t) + centerY);
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
