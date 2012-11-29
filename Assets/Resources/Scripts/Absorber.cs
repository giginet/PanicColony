using UnityEngine;
using System.Collections;

public class Absorber : MonoBehaviour {
    public float absorbSpeed = 70.0f;
    public float range = 15.0f;

    // Use this for initialization
    void Start () {
        this.transform.Translate(Vector3.down * 100);
    }
    
    // Update is called once per frame
    void Update () {
        foreach (GameObject target in GameObject.FindGameObjectsWithTag("Player")) {
            this.Absorb (target);
        } 
        foreach (GameObject target in GameObject.FindGameObjectsWithTag("Enemy")) {
            this.Absorb (target);
        }
    }
    
    void Absorb (GameObject target) {
        Vector3 position = this.transform.position;
        position.y = 0;
        Vector3 targetPosition = target.transform.position;
        targetPosition.y = 0;
        float distance = Vector3.Distance (targetPosition, position);
        if (distance < this.range) {
            Vector3 absorb = position - targetPosition;
            absorb = Vector3.Normalize (absorb) * absorbSpeed / distance;
            target.transform.position += absorb * Time.deltaTime;
        }
    }
}