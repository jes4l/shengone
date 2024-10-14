
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WebcamLogic : MonoBehaviour
{
    //Make sure to attach these Buttons in the Inspector
    public Button DoneButton;

    public TMP_Text DisplayText;

    public Button RedoButton;
    public GameObject[] objectsToEnable;

    private ComputerVision webcamManager;

    public int ButtonCounter = 0;

    


    void Start()
    {
        DoneButton.onClick.AddListener(TaskOnClick);
        RedoButton.onClick.AddListener(RedoOnClick);
        webcamManager = FindObjectOfType<ComputerVision>();

    }

    void TaskOnClick()
    {
        webcamManager.PauseWebcam();
        DisplayText.text = "put droppers on face :)";

        for(int i = 0; i < objectsToEnable.Length; i++)
        {
            objectsToEnable[i].SetActive(true); // Enable the GameObject
        }


    }

    void RedoOnClick()
    {
        webcamManager.PlayWebcam();

        for(int i = 0; i < objectsToEnable.Length; i++)
        {
            objectsToEnable[i].SetActive(false); // Enable the GameObject
        }


    }
}