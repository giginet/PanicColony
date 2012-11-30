using UnityEngine;
using System.Collections;

public class Gate : MonoBehaviour {
    public enum GateType {
        Automatic,
        Switch
    }; 
    public int range = 1;
    public int threshold = 5;
    public float speed = 2.5f;
    public GateType type = GateType.Automatic;
    private float distance = 0.0f;
    private GameObject left = null;
    private GameObject right = null;
    private bool isOpeaning = false;

    void Start () {
        left = (GameObject)this.gameObject.transform.Find("left").gameObject;
        right = (GameObject)this.gameObject.transform.Find("right").gameObject;
        this.Open();
    }
    
    void Update () {
        if (this.type == GateType.Automatic) {
            GameObject player = GameObject.FindWithTag("Player");
            isOpeaning = (Vector3.Distance(player.transform.position, this.transform.position) < this.threshold);
        }
        this.Move();
    }
    
    private void Move () {
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
    
    public void Open () {
        this.isOpeaning = true;
    }
    
    public void Close () {
        this.isOpeaning = false;
    }
}
