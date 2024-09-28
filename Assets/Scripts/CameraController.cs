using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player; // Reference to the player
    private Camera cam;
    private float screenHeight;
    private float lastCameraY; // Keep track of the camera's last Y position

    void Start()
    {
        cam = Camera.main;
        screenHeight = cam.orthographicSize * 2; // Get the screen height in world units
        lastCameraY = cam.transform.position.y; // Store the initial camera Y position
    }

    void Update()
    {
        // Check if the player has moved above the top of the current screen
        if (player.position.y > lastCameraY + screenHeight / 2)
        {
            MoveCameraUp();
        }

        if (player.position.y < lastCameraY - screenHeight / 2)
        {
            MoveCameraDown();
        }
    }

    void MoveCameraUp()
    {
        // Move the camera up by one screen height
        cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y + screenHeight, cam.transform.position.z);

        // Update the last camera Y position
        lastCameraY = cam.transform.position.y;
    }
    void MoveCameraDown()
    {
        cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y - screenHeight, cam.transform.position.z);
        lastCameraY = cam.transform.position.y;
    }
}
