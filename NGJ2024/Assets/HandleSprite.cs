using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleSprite : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {

        // this currently gets the correct mesh of the player. but not the texture.

        // player is a gameobject with the prefab containing the mesh AND texture titled in the format black_skin_blonde_hair.

        // player object is sent from ColourPicker.cs

        
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("PlayerSkin");
        Debug.Log(playerObjects.Length);
        GameObject player = playerObjects[0];
        player.transform.parent = this.transform;
        this.GetComponent<Renderer>().material = player.GetComponent<Renderer>().material;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
