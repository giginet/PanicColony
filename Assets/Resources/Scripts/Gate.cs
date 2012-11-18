using UnityEngine;
using System.Collections;

public class Gate : MonoBehaviour {
    public int range = 2;
    public float speed = 0.5f;
    private float distance = 0.0f;
    private GameObject left = null;
    private GameObject right = null;
    private bool isOpeaning = false;

    void Start () {
        left = (GameObject)this.gameObject.transform.Find("left").gameObject;
        right = (GameObject)this.gameObject.transform.Find("right").gameObject;
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
        Vector3 leftPosition = this.left.transform.localPosition;
        leftPosition.x = -distance;
        this.left.transform.localPosition = leftPosition;
        Vector3 rightPosition = this.right.transform.localPosition;
        rightPosition.x = distance;
        this.right.transform.localPosition = rightPosition;
    }
}
