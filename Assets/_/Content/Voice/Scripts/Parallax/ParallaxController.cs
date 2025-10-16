using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    public Transform[] backgrounds;   // Array of all the 2D backgrounds to be parallaxed
    public float[] parallaxScales;    // The proportion of the camera's movement to move the backgrounds by
    public float smoothing = 1f;      // How smooth the parallax effect will be (should be > 0)

    private Transform cam;            // Reference to the camera's transform
    private Vector3 previousCamPos;   // The position of the camera in the previous frame

    void Start()
    {
        cam = Camera.main.transform;
        previousCamPos = cam.position;

        parallaxScales = new float[backgrounds.Length];
        for (int i = 0; i < backgrounds.Length; i++)
        {
            parallaxScales[i] = backgrounds[i].position.z * -1;
        }
    }

    void Update()
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            float parallax = (previousCamPos.x - cam.position.x) * parallaxScales[i];

            float backgroundTargetPosX = backgrounds[i].position.x + parallax;

            Vector3 backgroundTargetPos = new Vector3(backgroundTargetPosX, backgrounds[i].position.y, backgrounds[i].position.z);

            backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, backgroundTargetPos, smoothing * Time.deltaTime);
        }

        previousCamPos = cam.position;
    }
}