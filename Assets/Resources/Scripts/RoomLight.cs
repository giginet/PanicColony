using UnityEngine;
using System.Collections;

public class RoomLight : MonoBehaviour {

    private GameObject player = null;

    // Use this for initialization
    void Start () {
        this.player = GameObject.FindWithTag("Player"); 
    }
    
    // Update is called once per frame
    void Update () {
        this.light.enabled = Vector3.Distance (this.transform.position, this.player.transform.position) < 50; 
    }
}
