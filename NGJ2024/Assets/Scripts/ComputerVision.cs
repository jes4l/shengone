using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class ComputerVision : MonoBehaviour
{

    private WebCamTexture webCam;

    public Camera camera;

    private Vector2 pixelCoord;
    public Plane m_Plane;   

    
    //This is the distance the clickable plane is from the camera. Set it in the Inspector before running.
    public float m_DistanceZ;
    Vector3 m_DistanceFromCamera;

    private Texture2D modifiableTexture;

    public Renderer renderer;

    public Animator animator;

    private bool runningWebcam = false;

    bool hasStartedCoroutine = false;
    bool hasStartedCoroutine2 = false;

    public GameObject doneButton;

    private bool changeScene;

    public GameObject crt, crtLeft, crtRight;

    float speed = 0.7f;

    public Transform destination, destinationLeft, destinationRight;

    private bool takenPhoto = false;

    public GameObject crtScreen;

    





    // Start is called before the first frame update
    void Start()
    {
        webCam = new WebCamTexture();
            
        if(!webCam.isPlaying) webCam.Play();
    }

    IEnumerator Render(float delay)
    {
            yield return new WaitForSeconds(delay);

            doneButton.SetActive(true);
            renderer.material.SetColor("_Color", Color.white);

            renderer.material.mainTexture = webCam;

            //This is how far away from the Camera the plane is placed
            m_DistanceFromCamera = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y - m_DistanceZ, Camera.main.transform.position.z);

            //Create a new plane with normal (0,0,1) at the position away from the camera you define in the Inspector. This is the plane that you can click so make sure it is reachable.
            m_Plane = new Plane(Vector3.up, m_DistanceFromCamera);

            // Create a new Texture2D with the same dimensions
            modifiableTexture = new Texture2D(webCam.width, webCam.height, TextureFormat.RGB24, false);

            runningWebcam = true;
    }

    // Update is called once per frame

    // Update is called once per frame
    void Update()
    {

        if (animator.GetBool("PressedStart"))
        {
            if (!hasStartedCoroutine)
            {
                StartCoroutine(Render(2.0f));
                hasStartedCoroutine = true;
            }
        }


        if(runningWebcam && webCam.isPlaying)
        {
            renderer.material.mainTexture = webCam;
            //Detect when there is a mouse click
            if (Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    // Check if the hit object has a box collider
                    BoxCollider boxCollider = hit.collider as BoxCollider;
                    if (boxCollider != null)
                    {
                        GameObject hitObject = hit.collider.gameObject;
                        if(hitObject.name == "Button")
                        {
                            // handle changing scene

                            if (!hasStartedCoroutine2)
                            {
                                // Create a new Texture2D with the same dimensions
                                Texture2D screenshot = new Texture2D(webCam.width, webCam.height, TextureFormat.RGB24, false);
                                
                                // Copy current frame to modifiable texture
                                screenshot.SetPixels32(webCam.GetPixels32());
                                
                                // Apply all changes to the texture
                                screenshot.Apply();

                                // apply the texture to the crt screen

                                crtScreen.GetComponent<Renderer>().material.mainTexture = screenshot;

                                StartCoroutine(TakeImage(2.0f));
                                hasStartedCoroutine2 = true;
                                changeScene = true;
                            }
                                
                        }
                    }
                }
            }
        }

        if(changeScene && takenPhoto)
        {
            crt.transform.position = Vector3.Lerp(crt.transform.position, destination.position, Time.deltaTime * speed);
            crtLeft.transform.position = Vector3.Lerp(crtLeft.transform.position, destinationLeft.position, Time.deltaTime * speed);
            crtRight.transform.position = Vector3.Lerp(crtRight.transform.position, destinationRight.position, Time.deltaTime * speed);

            renderer.material.mainTexture = null;
            renderer.material.SetColor("_Color", Color.grey);



        }
    }

    public IEnumerator TakeImage(float delay)
    {
        yield return new WaitForSeconds(delay);

        takenPhoto = true;
    }






    public Texture2D GetTexture()
    {
        // Create a new Texture2D with the same dimensions as the WebCamTexture
        Texture2D texture2D = new Texture2D(webCam.width, webCam.height);
        
        // Apply the current frame of the webcam to the Texture2D
        texture2D.SetPixels(webCam.GetPixels());
        texture2D.Apply();

        return texture2D;
    }

    public void PauseWebcam()
    {
        webCam.Pause();
    }

    public void PlayWebcam()
    {
        webCam.Play();
    }


}
