using UnityEngine;
using System.Collections;

public class Colony : MonoBehaviour {

    public float rotateSpeed = 1.0f;

    // Use this for initialization
    void Start () {
    
    }
    
    // Update is called once per frame
    void Update () {
        this.transform.Rotate(Vector3.up * this.rotateSpeed); 
    }
}
