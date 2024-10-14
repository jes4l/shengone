using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyBoidBrain : MonoBehaviour
{
    // Start is called before the first frame update
    CharacterController contr;
    AiSensor aiSensor;
    [SerializeField]
    List<GameObject> locs;

    private GameObject currentTarget = null;
    void Start()
    {
       contr = GetComponent<CharacterController>();
        aiSensor = GetComponent<AiSensor>();
    }

    // Update is called once per frame
    void Update()
    {
         Vector3 velocity = new Vector3(0,0,0);
        if (!contr.isGrounded)
        {
            // ... apply gravity
            velocity.y += -9.18f;
            contr.Move(velocity * Time.deltaTime);
        }
        else
        {

            if (aiSensor.Objects.Count > 0)
            {
                Vector3 des = (((aiSensor.Objects.ElementAt(0).transform.position - transform.position).normalized) * Time.deltaTime);
                des.y = 0f;
                contr.Move(des);
                Quaternion targetRotation = Quaternion.LookRotation(des, Vector3.up);
                targetRotation.z = 0f;
                targetRotation.x = 0f;
                // Set our rotation to this rotation
                transform.rotation = targetRotation;
            }
            else
            {
                if (currentTarget == null || Vector3.Distance(transform.position, currentTarget.transform.position) < 0.1f)
                {
                    // ... find the next closest GameObject
                    GameObject closest = null;
                    float closestDistance = Mathf.Infinity;

                    foreach (GameObject loc in locs)
                    {
                        float distance = Vector3.Distance(transform.position, loc.transform.position);
                        if (distance < closestDistance && loc != currentTarget)
                        {
                            closest = loc;
                            closestDistance = distance;
                        }
                    }

                    // Set the new target
                    currentTarget = closest;
                }

                // If a closest GameObject was found...
                if (currentTarget != null)
                {
                    // ... move towards it
                    Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
                    direction.y = 0f;
                    contr.Move(direction * Time.deltaTime);
                    Quaternion targetRotation = Quaternion.LookRotation(currentTarget.transform.position, Vector3.up);
                    targetRotation.z = 0f;
                    targetRotation.x = 0f;
                    // Set our rotation to this rotation
                    transform.rotation = targetRotation;
                }
            }
        }
    }
    }

