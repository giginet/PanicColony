using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour {
    private bool ground = false;

    void Start () {
        this.animation.Play("setup");
    }
    
    void Update () {
        Ray floorRay = new Ray(this.transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(floorRay, out hit, this.transform.localScale.y / 2) && !this.ground) {
            this.animation.Play("setup");
            this.audio.Play();
            this.ground = true;
            this.SendMessage("UpdateRadar"); 
        } else if (!this.ground) {
            this.animation.Stop("setup");
        }
    
    } 
}
