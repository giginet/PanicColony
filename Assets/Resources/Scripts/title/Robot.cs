using UnityEngine;
using System.Collections;

public class Robot : MonoBehaviour {

    public float speed = 0.1f;

    // Use this for initialization
    void Start () {
        this.rigidbody.velocity = new Vector3(1.0f, 1.0f, 1.0f) * this.speed;
        this.rigidbody.AddTorque(new Vector3(0f, 50.0f, 0f));
    }
    
    // Update is called once per frame
    void Update () {
    
    }
}
