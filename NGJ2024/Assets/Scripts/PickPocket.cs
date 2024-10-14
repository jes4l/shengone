using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class PickPocket : MonoBehaviour
{
    [SerializeField]
    float stealRadius;
    [SerializeField]
    string id;
    [SerializeField]
    float stealTimeMax;
    [SerializeField]
    GameObject passprt;

    [SerializeField]
    GameObject charecter;

    [SerializeField]
    Mesh mesh;

    [SerializeField]
    Texture2D texture;

    [SerializeField]
    GameObject success_passprt;


    float tim;

    // Start is called before the first frame update
    void Start()
    {
        string id = StaticScript.charachteristics;
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("PlayerSkin");

        foreach (GameObject obj in objectsWithTag)
        {
            Debug.Log(obj + " " + id);
            if (obj.name.Equals(id))
            {
                Quaternion rot = Quaternion.identity;
                
                GameObject newObj = Instantiate(obj, transform.position, transform.rotation);
                newObj.transform.SetParent(this.transform);
                newObj.transform.rotation = Quaternion.Euler(0, 90, 0);
            }
        }
    }


    void Update()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Boid");
        if (gos.Length > 0)
        {
            foreach (GameObject go in gos)
            {
                if (Vector3.Distance(go.transform.position, transform.position) <= stealRadius && go.GetComponent<Boids>().hasPassport)
                {
                    if (Input.GetMouseButton(0))
                    {
                        tim += Time.deltaTime;
                        if (tim > stealTimeMax)
                        {

                            tim = 0;
                            go.GetComponent<Boids>().hasPassport = false;
                            Vector3 pos = go.transform.position;
                         
                            GameObject prt = Instantiate(passprt, pos, transform.rotation);
                            prt.GetComponent<PassPortIllegalscrpt>().ID = go.GetComponent<Boids>().specialID;
                        }
                    }
                    else
                    {
                        tim = 0;
                    }
                }
            }
        }
    }
}

        