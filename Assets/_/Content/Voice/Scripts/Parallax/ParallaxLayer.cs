using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Tooltip("How much this layer moves relative to the camera (0 = fixed to camera, 1 = moves normally)")]
    [Range(0f, 1f)]
    public float parallaxFactor = 0.5f;

    [Tooltip("Should this layer move horizontally with camera?")]
    public bool parallaxHorizontal = true;

    [Tooltip("Should this layer move vertically with camera?")]
    public bool parallaxVertical = true;

    [Tooltip("Optional starting position reference")]
    public Vector2 startingOffset = Vector2.zero;

    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
    private Vector3 startPosition;

    private void Start()
    {
        // Get reference to the main camera (controlled by Cinemachine)
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
        
        // Store the starting position of this layer
        startPosition = transform.position;
    }

    private void LateUpdate()
    {
        // Calculate how much the camera has moved since last frame
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        
        // Apply parallax effect based on camera movement
        float moveX = parallaxHorizontal ? deltaMovement.x * (1 - parallaxFactor) : 0f;
        float moveY = parallaxVertical ? deltaMovement.y * (1 - parallaxFactor) : 0f;
        
        // Move the layer
        transform.position += new Vector3(moveX, moveY, 0);
        
        // Update the last camera position for next frame
        lastCameraPosition = cameraTransform.position;
    }
}
