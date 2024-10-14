using UnityEngine;

public class HoverObjects : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }
    [SerializeField]
    Camera cam;
    // Update is called once per frame
    // Store the original position
    Vector3 originalPosition;
    Transform child;

    public Animator animator;

    private bool hovering = false;
    private string childName;
 
    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (child != null)
        {
            child.gameObject.transform.localScale = new Vector3(0.8f,0.8f,0.8f);
            child = null;
        }
        if (Physics.Raycast(ray, out hit, 100f, 1 << 31))
        {
            child = hit.transform.Find("Emulsion");

            if (child != null)
            {
                child.gameObject.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);

                childName = hit.transform.gameObject.name;

                float y = Mathf.Clamp(hit.transform .position.y+ Mathf.Sin(Time.time) * 0.0001f,0,hit.transform.position.y+1);
                hit.transform.position= new Vector3(hit.transform.position.x,0,hit.transform.position.z);
                hit.transform.position = hit.transform.position+ new Vector3(0,y,0);

                hovering = true;

            }
              
        }

        if(Input.GetMouseButtonDown(0) && hovering && childName == "Spassport")
        {
            animator.SetBool("PressedStart", true);
        }

          
       
    }
   
            
    }


