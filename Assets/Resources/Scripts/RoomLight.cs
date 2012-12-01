using UnityEngine;
using System.Collections;

public class RoomLight : MonoBehaviour {


    // Use this for initialization
    void Start () {
    }
    
    // Update is called once per frame
    void Update () {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) { 
            this.light.enabled = Vector3.Distance (this.transform.position, player.transform.position) < 50; 
        }
    }
}
