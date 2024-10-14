using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEngine.SceneManagement;


public class ColourPicker : MonoBehaviour
{


    [SerializeField] private Plane img = default;

    public static string airport_player_object;

    public Camera camera;

    private Vector2 pixelCoord;

    private Texture webCam;

    public GameObject SkinColour;

    public GameObject HairColour;


    public Plane m_Plane;   

    
    Vector3 m_DistanceFromCamera;

    private Texture2D modifiableTexture;

    public Renderer renderer, skinColourRenderer, hairColourRenderer;


    private bool runningWebcam = false;

    bool hasStartedCoroutine = false;

    private int counter = 0;

    private Color hairColor;
    private Color skinColor;

    public Texture2D playerTexture;

    private Color defaultHairColor, defaultSkinColor;

    private Color gingerHair, blondeHair, brunetteHair, blackHair;

    private Color blackSkin, brownSkin, yellowSkin, whiteSkin;

    private List<Color> hairs;
    private List<Color> skins;

    public GameObject player;

    public GameObject black_skin_black_hair;
    public GameObject white_skin_black_hair;
    public GameObject brown_skin_black_hair;
    public GameObject yellow_skin_black_hair;
    public GameObject black_skin_brunette_hair;
    public GameObject white_skin_brunette_hair;
    public GameObject brown_skin_brunette_hair;
    public GameObject yellow_skin_brunette_hair;
    public GameObject black_skin_blonde_hair;
    public GameObject white_skin_blonde_hair;
    public GameObject brown_skin_blonde_hair;
    public GameObject yellow_skin_blonde_hair;
    public GameObject black_skin_ginger_hair;
    public GameObject white_skin_ginger_hair;
    public GameObject brown_skin_ginger_hair;
    public GameObject yellow_skin_ginger_hair;


    // Start is called before the first frame update
    void Start()
    {

        gingerHair = new Color(112,59,48);
        blondeHair = new Color(200,159,115);
        brunetteHair = new Color(45, 23, 14);
        blackHair = new Color(0, 0, 0);

        defaultHairColor = brunetteHair;

        hairs = new List<Color> {gingerHair, blondeHair, brunetteHair, blackHair};

        blackSkin = new Color(0,0,0);
        brownSkin = new Color(45, 23, 14);
        yellowSkin = new Color(255, 255, 0);
        whiteSkin = new Color(233, 200, 188);

        defaultSkinColor = yellowSkin;

        skins = new List<Color> {blackSkin, brownSkin, yellowSkin, whiteSkin};

    }

    // closed match in RGB space
    Color closestColor2(List<Color> colors, Color target)
    {
        var colorDiffs = colors.Select(n => ColorDiff(n, target)).Min(n =>n);
        int color = colors.FindIndex(n => ColorDiff(n, target) == colorDiffs);
        return colors[color];
    }

    // distance in RGB space
    int ColorDiff(Color c1, Color c2) 
    { return  (int ) Math.Sqrt((c1.r - c2.r) * (c1.r - c2.r) 
                                + (c1.g - c2.g) * (c1.g - c2.g)
                                + (c1.b - c2.b) * (c1.b - c2.b)); 
    }

    void changeHairDynamic()
    {
        Color[] pixels = playerTexture.GetPixels(0, 0, playerTexture.width, playerTexture.height, 0);

        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i] == defaultHairColor)
            {
                pixels[i] = hairColor;
            }   
        }

        playerTexture.SetPixels(0, 0, playerTexture.width, playerTexture.height, pixels, 0);
        playerTexture.Apply();
        player.GetComponent<Renderer>().material.mainTexture = playerTexture;
    }

    void changeSkinDynamic()    
    {
        Color[] pixels = playerTexture.GetPixels(0, 0, playerTexture.width, playerTexture.height, 0);

        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i] == defaultSkinColor)
            {
                pixels[i] = Color.green;
            } 

        }

        playerTexture.SetPixels(0, 0, playerTexture.width, playerTexture.height, pixels, 0);
        playerTexture.Apply();
        player.GetComponent<Renderer>().material.mainTexture = playerTexture;
    }


    void changeHair()
    {
        // ginger blonde brunette black

        Color closest = closestColor2(hairs, hairColor);

        if(closest == gingerHair)
        {
            Debug.Log("ginger");
        }
        if(closest == blackHair)
        {
            Debug.Log("black");
        }
        if(closest == brunetteHair)
        {
            Debug.Log("brune");
        }
        if(closest == blondeHair)
        {
            Debug.Log("blonde");
        }
    }

    void changeSkin()
    {
        Color closest = closestColor2(skins, skinColor);

        if(closest == blackSkin)
        {
            Debug.Log("black skin");
        }
        if(closest == brownSkin)
        {
            Debug.Log("brown skin");
        }
        if(closest == yellowSkin)
        {
            Debug.Log("yellow skin");
        }
        if(closest == whiteSkin)
        {
            Debug.Log("white skin");
        }
    }



    IEnumerator Render(float delay)
    {
            yield return new WaitForSeconds(delay);
            renderer.material.SetColor("_Color", Color.white);

            renderer.material.mainTexture = webCam;


            // Create a new Texture2D with the same dimensions
            modifiableTexture = new Texture2D(webCam.width, webCam.height, TextureFormat.RGB24, false);

            runningWebcam = true;
    }

    // Update is called once per frame
    void Update()
    {
        webCam = GetComponent<Renderer>().material.mainTexture;

        if(webCam == null) return;


        if (!hasStartedCoroutine)
        {
            StartCoroutine(Render(2.0f));
            hasStartedCoroutine = true;
        }

        if(runningWebcam )
        {
            renderer.material.mainTexture = webCam;
            //Detect when there is a mouse click
            /*// Create a new Texture2D with the same dimensions
            modifiableTexture = new Texture2D(webCam.width, webCam.height, TextureFormat.RGB24, false);
            
            // Copy current frame to modifiable texture
            modifiableTexture.SetPixels32(webCam.GetPixels32());
            
            // Here, add your modifications, example: set a pixel to black at the center
            modifiableTexture.SetPixel(modifiableTexture.width / 2, modifiableTexture.height / 2, Color.black);

            // Apply all changes to the texture
            modifiableTexture.Apply();*/

            modifiableTexture = webCam as Texture2D;
            //modifiableTexture.Apply();
            
            // Optionally: Display this texture on a material or GUI element if needed
            //renderer.material.mainTexture = modifiableTexture;
            
            //Create a ray from the Mouse click position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //Initialise the enter variable

            RaycastHit hit;
            if (!Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit))
                return;

            Renderer rend = renderer;
            MeshCollider meshCollider = hit.collider as MeshCollider;

            if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
                return;

            Texture2D tex = rend.material.mainTexture as Texture2D;

            if(tex is null)
            {
                Debug.Log("tex is null");
            } 

            Vector2 pixelUV = hit.textureCoord;
            
            pixelUV.x *= tex.width;
            pixelUV.y *= tex.height;

            // Ensure pixel coordinates are clamped within the texture's dimensions
            int x = Mathf.Clamp((int)pixelUV.x, 0, tex.width - 1);
            int y = Mathf.Clamp((int)pixelUV.y, 0, tex.height - 1);

            // Get the color of the pixel at the specified coordinates
            Color originalColor = tex.GetPixel(x, y);
            // Log the original color

            if(counter == 0)
            {
                skinColourRenderer.material.color = originalColor;
                skinColor = new Color(originalColor.r * 255, originalColor.g * 255, originalColor.b * 255);
                Debug.Log("skin:" + skinColor);
                changeSkin();
            }
            if(counter == 1)
            {
                hairColourRenderer.material.color = originalColor;
                hairColor = new Color(originalColor.r * 255, originalColor.g * 255, originalColor.b * 255);
                Debug.Log("hair:" + hairColor);
                changeHair();
            }

            if (Input.GetMouseButtonUp(0))
            {
                counter++;

                Color closest = closestColor2(skins, skinColor);
                Color closestHair = closestColor2(hairs, hairColor);

                    black_skin_black_hair.SetActive(false);
                    brown_skin_black_hair.SetActive(false);
                    yellow_skin_black_hair.SetActive(false);
                    white_skin_black_hair.SetActive(false);
                    black_skin_brunette_hair.SetActive(false);
                    brown_skin_brunette_hair.SetActive(false);
                    yellow_skin_brunette_hair.SetActive(false);
                    white_skin_brunette_hair.SetActive(false);
                    black_skin_blonde_hair.SetActive(false);
                    brown_skin_blonde_hair.SetActive(false);
                    yellow_skin_blonde_hair.SetActive(false);
                    white_skin_blonde_hair.SetActive(false);
                    black_skin_ginger_hair.SetActive(false);
                    brown_skin_ginger_hair.SetActive(false);
                    yellow_skin_ginger_hair.SetActive(false);
                    white_skin_ginger_hair.SetActive(false);




                if(closest == blackSkin && closestHair == blackHair)
                {
                    black_skin_black_hair.SetActive(true);
                    black_skin_black_hair.tag = "PlayerSkin";
                    airport_player_object  = "SBlack_HBlack";
                }
                if(closest == brownSkin && closestHair == blackHair)
                {
                    brown_skin_black_hair.SetActive(true);
                    brown_skin_black_hair.tag = "PlayerSkin";
                    airport_player_object  = "SBrown_HBlack";
                }
                if(closest == yellowSkin&& closestHair == blackHair)
                {
                    yellow_skin_black_hair.SetActive(true);
                    yellow_skin_black_hair.tag = "PlayerSkin";
                    airport_player_object  = "SYellow_HBlack";
                }
                if(closest == whiteSkin&& closestHair == blackHair)
                {
                    white_skin_black_hair.SetActive(true);
                    white_skin_black_hair.tag = "PlayerSkin";
                    airport_player_object  = "SWhite_HBlack";
                }

                if(closest == blackSkin&& closestHair == brunetteHair)
                {
                    black_skin_brunette_hair.SetActive(true);
                    black_skin_brunette_hair.tag = "PlayerSkin";
                    airport_player_object  = "SBlack_HBrunette";
                }
                if(closest == brownSkin&& closestHair == brunetteHair)
                {
                    brown_skin_brunette_hair.SetActive(true);
                    brown_skin_brunette_hair.tag = "PlayerSkin";
                    airport_player_object  = "SBrown_HBrunette";
                }
                if(closest == yellowSkin&& closestHair == brunetteHair)
                {
                    yellow_skin_brunette_hair.SetActive(true);
                    yellow_skin_brunette_hair.tag = "PlayerSkin";
                    airport_player_object  = "SYellow_HBrunette";
                }
                if(closest == whiteSkin&& closestHair == brunetteHair)
                {
                    white_skin_brunette_hair.SetActive(true);
                    white_skin_brunette_hair.tag = "PlayerSkin";
                    airport_player_object  = "SWhite_HBrunette";
                }

                if(closest == blackSkin&& closestHair == blondeHair)
                {
                    black_skin_blonde_hair.SetActive(true);
                    black_skin_blonde_hair.tag = "PlayerSkin";
                    airport_player_object  = "SBlack_HBlonde";
                }
                if(closest == brownSkin&& closestHair == blondeHair)
                {
                    brown_skin_blonde_hair.SetActive(true);
                    brown_skin_blonde_hair.tag = "PlayerSkin";
                    airport_player_object  = "SBrown_HBlonde";
                }
                if(closest == yellowSkin&& closestHair == blondeHair)
                {
                    yellow_skin_blonde_hair.SetActive(true);
                    yellow_skin_blonde_hair.tag = "PlayerSkin";
                    airport_player_object  = "SYellow_HBlonde";
                }
                if(closest == whiteSkin&& closestHair == blondeHair)
                {
                    white_skin_blonde_hair.SetActive(true);
                    white_skin_blonde_hair.tag = "PlayerSkin";
                    airport_player_object  = "SWhite_HBlonde";
                }

                if(closest == blackSkin&& closestHair == gingerHair)
                {
                    black_skin_ginger_hair.SetActive(true);
                    black_skin_ginger_hair.tag = "PlayerSkin";
                    airport_player_object  = "SBlack_HGinger";
                }
                if(closest == brownSkin&& closestHair == gingerHair)
                {
                    brown_skin_ginger_hair.SetActive(true);
                    brown_skin_ginger_hair.tag = "PlayerSkin";
                    airport_player_object  = "SBrown_HGinger";
                }
                if(closest == yellowSkin&& closestHair == gingerHair)
                {
                    yellow_skin_ginger_hair.SetActive(true);
                    yellow_skin_ginger_hair.tag = "PlayerSkin";
                    airport_player_object  = "SYellow_HGinger";
                }
                if(closest == whiteSkin&& closestHair == gingerHair)
                {
                    white_skin_ginger_hair.SetActive(true);
                    white_skin_ginger_hair.tag = "PlayerSkin";
                    airport_player_object  = "SWhite_HGinger";
                }
            }

            if(counter == 2)
            {
                StaticScript.charachteristics = airport_player_object;
                SceneManager.LoadScene("Airport2", LoadSceneMode.Single);
                counter = 80085;
            }

            
            
            
        }
    }




}
