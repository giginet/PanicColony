using UnityEngine;
using System.Collections;

public class Shutter : MonoBehaviour {

    public int range = 2;
    public float speed = 2.5f;
    private float distance = 0.0f;
    private GameObject shutter = null;

    void Start () {
        shutter = (GameObject)this.gameObject.transform.Find("shutter").gameObject;
    }
    
    void Update () {
        if (this.transform.localPosition.y > 0) {
            this.transform.Translate(new Vector3(0, -this.speed, 0));
        } else if (this.transform.localPosition.y < 0) {
            this.transform.localPosition = Vector3.zero;
        }
        
    }   
}
