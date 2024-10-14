using UnityEngine;
using UnityEngine.EventSystems; // Required for UI events
using UnityEngine.UI;
using TMPro;

public class DragDropper : MonoBehaviour
{
    RectTransform rectTransform;

    public Button m_YourFirstButton;

    private int rateLimiter = 0;

    public Image targetImage; // Assign this in the inspector
    private Texture2D texture;

    private bool selecting = false;

    public TMP_Text textObject;






    private ComputerVision computerVision;

    

    void Start()
    {
        // Get the RectTransform component of the button
        m_YourFirstButton.onClick.AddListener(OnPointerClick);

        computerVision = FindObjectOfType<ComputerVision>();
    }

    public void Update()
    {


        if (selecting) // 0 is the button number for the left mouse button
        {
            HandlePixelColour();
            rateLimiter++;
            if(Input.GetMouseButtonDown(0))
            {
                selecting = false;
                textObject.text = "put droppers on face :)";
            }
        }

    }

    Vector2 ScreenToTextureCoordinates(Vector2 screenPosition)
    {
        // Convert screen position to texture coordinates directly
        Vector2 pixelUV = Camera.main.ScreenToViewportPoint(screenPosition);
        return new Vector2(pixelUV.x * texture.width, pixelUV.y * texture.height);
    }


    public void HandlePixelColour()
    {
        if(rateLimiter >= 50)
        {
            rateLimiter = 0;

            

            //Color pixelColor = computerVision.getColor();

            //targetImage.color = pixelColor;

        }
    }

    

    public void OnPointerClick()
    {
        
        textObject.text = "Select Component";
        selecting = true;
        
    }
}
