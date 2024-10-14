
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Boids : MonoBehaviour
{
  
    public int targetID; 
    private CharacterController agent;
    [SerializeField]
    public List<GameObject> locations;
    [SerializeField]
    List<GameObject> Actions;

    [SerializeField]
    public float speed;

    [SerializeField]    
    GameObject target;
    [SerializeField]
    public string specialID;

    [SerializeField]
    public float range;


    public bool hasPassport = true;
    void Start()
    {
        target = new GameObject();
        agent = GetComponent<CharacterController>();
        pass = true;
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("PlayerSkin");

       
                Quaternion rot = Quaternion.identity;

                GameObject newObj = Instantiate(objectsWithTag[Random.Range(0,objectsWithTag.Length)], transform.position, transform.rotation);
         specialID = newObj.name;
        specialID = specialID.Replace("(Clone)", "");
        newObj.transform.SetParent(this.transform);
                newObj.transform.rotation = Quaternion.Euler(0, 90, 0);
          
        
    }

    bool pass;
    private void Update()
    {
        if (locations.Count > 0)
        {
            if (Vector3.Distance(transform.position, locations[0].transform.position) < range && pass == true)
            {
               
                locations.RemoveAt(0);
              
            }
            else if(Vector3.Distance(target.transform.position,transform.position)<range && pass == false)
            {
                    Actions = new List<GameObject>();
                    pass = true;
                    target = new GameObject();
            }
            else if (Vector3.Distance(target.transform.position, transform.position) > range && pass == false)
            {
                Vector3 targetPosition = target.transform.position;
                targetPosition.y = transform.position.y;
                Vector3 direction = (targetPosition - transform.position).normalized;
                agent.Move(direction * Time.deltaTime * speed);
                Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 10f * Time.deltaTime);
            }

            Vector3 velocity = new Vector3(0, -9.18f, 0);
            if (!agent.isGrounded)
            {
                agent.Move(velocity * Time.deltaTime * speed);
            }

           
                if (locations.Count > 0 && pass == true)
                {
                    if (locations[0].CompareTag("WayPoint") )
                    {
                        Vector3 targetPosition = locations[0].transform.position;
                        targetPosition.y = transform.position.y;
                        Vector3 direction = (targetPosition - transform.position).normalized;
                        agent.Move(direction * Time.deltaTime * speed);
                        Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 90f * Time.deltaTime);
                }
                else if (locations[0].CompareTag("ActionPoint"))
                    {
                       
                            while (locations[0].CompareTag("ActionPoint") && locations.Count > 0)
                            {
                                Actions.Add(locations[0]);
                                locations.RemoveAt(0);
                            }
                            target = Actions[Random.Range(0, Actions.Count) ];
                            pass = false;
                    }
                }

        }
        else
        {
            Destroy(this.gameObject);
        }
      
    }

    
}
