using UnityEngine;
using System.Collections;

public class Shutter : MonoBehaviour {

    public int range = 2;
    public float speed = 2.5f;
    private float distance = 0.0f;
    private GameObject shutter = null;
    private bool isOpeaning = false;

    void Start () {
        shutter = (GameObject)this.gameObject.transform.Find("shutter").gameObject;
    }
    
    void Update () {
        GameObject player = GameObject.FindWithTag("Player");
        isOpeaning = (Vector3.Distance(player.transform.position, this.transform.position) < 2.0);
        if (isOpeaning) {
            distance += speed * Time.deltaTime;
        } else {
            distance -= speed * Time.deltaTime;
        }
        if (distance > range) {
            distance = range;
        } else if (distance < 0) {
            distance = 0;
        }
        Vector3 position = this.shutter.transform.localPosition;
        position.y = 1 + distance;
        this.shutter.transform.localPosition = position;
    }   
}
