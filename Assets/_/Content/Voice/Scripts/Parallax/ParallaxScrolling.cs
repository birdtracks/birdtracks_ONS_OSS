using UnityEngine;

public class ParallaxScrolling : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform[] layerTransforms;
        [Range(0f, 1f)] 
        public float parallaxFactor = 0.5f;
        
        [Header("Axis Movement Scales")]
        [Range(0f, 1f)] 
        public float xMovementScale = 0.1f;
        [Range(0f, 1f)] 
        public float yMovementScale = 0.1f;
        [Range(0f, 1f)] 
        public float zMovementScale = 1f;
    }

    [Header("Parallax Layers")]
    public ParallaxLayer backgroundLayer;
    public ParallaxLayer midgroundLayer;
    public ParallaxLayer foregroundLayer;

    [Header("Scrolling Settings")]
    public float scrollSpeed = 5f;

    private Vector3 lastCameraPosition;

    private void Start()
    {
        // Cache initial camera position
        lastCameraPosition = Camera.main.transform.position;
    }

    private void Update()
    {
        // Calculate camera movement
        Vector3 cameraDelta = Camera.main.transform.position - lastCameraPosition;

        // Update each layer
        UpdateLayer(backgroundLayer, cameraDelta);
        UpdateLayer(midgroundLayer, cameraDelta);
        UpdateLayer(foregroundLayer, cameraDelta);

        // Update last camera position
        lastCameraPosition = Camera.main.transform.position;
    }

    private void UpdateLayer(ParallaxLayer layer, Vector3 cameraDelta)
    {
        if (layer.layerTransforms == null || layer.layerTransforms.Length == 0)
            return;

        // Calculate parallax movement with axis-specific scaling
        Vector3 parallaxMovement = new Vector3(
            -cameraDelta.x * layer.parallaxFactor * layer.xMovementScale,
            -cameraDelta.y * layer.parallaxFactor * layer.yMovementScale,
            -cameraDelta.z * layer.parallaxFactor * layer.zMovementScale
        );

        // Move each transform in the layer
        foreach (Transform layerTransform in layer.layerTransforms)
        {
            if (layerTransform != null)
            {
                layerTransform.localPosition += parallaxMovement;
            }
        }
    }

    // Optional: Manual scrolling method with axis-specific control
    public void ScrollManually(Vector3 scrollDirection, ParallaxLayer targetLayer = null)
    {
        if (targetLayer != null)
        {
            UpdateLayer(targetLayer, scrollDirection);
        }
        else
        {
            UpdateLayer(backgroundLayer, scrollDirection);
            UpdateLayer(midgroundLayer, scrollDirection);
            UpdateLayer(foregroundLayer, scrollDirection);
        }
    }
}