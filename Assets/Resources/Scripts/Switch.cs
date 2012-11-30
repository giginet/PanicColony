using UnityEngine;
using System.Collections;

public class Switch : MonoBehaviour {
    private Transform lever = null;
    private bool isMoving = false;
    private int animationTimer = 0;
    private bool toggle = false;
    private GameObject gate = null;

    void Start () {
        this.lever = this.transform.Find("lever");
    }
    
    void Update () {
        if (this.isMoving) {
            float rotate = this.lever.localEulerAngles.x;
            if (this.toggle && this.animationTimer < 9) {
                this.lever.Rotate(Vector3.right * 10);
            } else if (!this.toggle && this.animationTimer < 9) {
                this.lever.Rotate(Vector3.right * -10);
            } else {
                this.isMoving = false;
            }
            ++this.animationTimer;
        }
    }
    
    private void SetToggle (bool s) {
        if (!this.isMoving) {
            this.toggle = s;
            this.animationTimer = 0;
            this.isMoving = true;
            this.audio.Play();
            if (this.gate != null) {
                if (s) {
                    this.gate.SendMessage("Close");
                } else {
                    this.gate.SendMessage("Open");
                }
            }
        }
    }
    
    void Toggle () {
       this.SetToggle(!this.toggle); 
    }
    
    void On () {
        this.SetToggle(true);
    }
    
    void Off () {
        this.SetToggle(false);
    }
    
    void SetGate (GameObject obj) {
        this.gate = obj;
    }
    
}
