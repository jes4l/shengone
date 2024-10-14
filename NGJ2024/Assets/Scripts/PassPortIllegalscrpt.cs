using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassPortIllegalscrpt : MonoBehaviour
{
    [SerializeField]
    float MaxTime = 5f;

    [SerializeField]
    public string ID;
    float tim = 0f;
    // Start is called before the first frame update
    void Start()
    {
        this.tag= "Illegal";
        this.gameObject.layer = LayerMask.NameToLayer("holdLayer");

    }

    // Update is called once per frame
    void Update()
    {
        tim += Time.deltaTime;

        if (MaxTime<tim)
        {
            this.tag= "passport";
        }
    }
}
