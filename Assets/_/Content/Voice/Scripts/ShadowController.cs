using UnityEngine;

[ExecuteInEditMode]
public class ShadowController : MonoBehaviour
{
    [Header("References")]
    public Transform character;
    public Material shadowMaterial;
    
    [Header("Position Settings")]
    public float minHeight = 0f;        // Minimum Y position for shadow
    public float maxHeight = 5f;        // Maximum Y position for shadow
    public Vector3 positionOffset;      // Offset from character in all axes
    
    [Header("Shadow Radius Settings")]
    public float baseRadius = 1f;       // Base radius when at minimum height
    public float minRadius = 0.2f;      // Minimum shadow radius
    public float maxDistance = 10f;     // Distance at which shadow reaches minimum size
    
    private void Update()
    {
        if (character == null || shadowMaterial == null) return;
        
        // Calculate shadow position with offset and Y clamping
        Vector3 shadowPosition = character.position + positionOffset;
        shadowPosition.y = Mathf.Clamp(shadowPosition.y, minHeight, maxHeight);
        transform.position = shadowPosition;
        
        // Calculate vertical distance between character and shadow
        float heightDifference = character.position.y - transform.position.y;
        heightDifference = Mathf.Max(0, heightDifference); // Ensure non-negative
        
        // Calculate shadow radius based on height difference
        float heightRatio = Mathf.Clamp01(heightDifference / maxDistance);
        float currentRadius = Mathf.Lerp(baseRadius, minRadius, heightRatio);
        
        // Update shader parameters
        shadowMaterial.SetFloat("_Radius", currentRadius);
        
        // Optionally update opacity based on height
        // float currentOpacity = Mathf.Lerp(1f, 0.2f, heightRatio);
        // shadowMaterial.SetFloat("_InnerOpacity", currentOpacity);
    }
    
    private void OnDrawGizmosSelected()
    {
        if (character != null)
        {
            // Draw shadow radius
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, baseRadius);
            
            // Draw height range
            Gizmos.color = Color.green;
            Vector3 minPos = transform.position;
            Vector3 maxPos = transform.position;
            minPos.y = minHeight;
            maxPos.y = maxHeight;
            Gizmos.DrawLine(minPos, maxPos);
            
            // Draw line to character
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, character.position);
        }
    }
}