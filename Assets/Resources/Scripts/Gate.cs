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
    public int maxHP = 100;
    private int HP = 0;
    private float distance = 0.0f;
    private GameObject left = null;
    private GameObject right = null;
    private bool isOpeaning = false;

    void Start () {
        left = (GameObject)this.gameObject.transform.Find("left").gameObject;
        right = (GameObject)this.gameObject.transform.Find("right").gameObject;
        this.isOpeaning = true;
        this.HP = maxHP;
    }
    
    void Update () {
        if (this.type == GateType.Automatic) {
            GameObject player = GameObject.FindWithTag("Player");
            if (Vector3.Distance(player.transform.position, this.transform.position) < this.threshold) {
                this.Open();
            } else {
                this.Close();
            }
        }
        this.Move();
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy")) {
            if (Vector3.Distance(enemy.transform.position, this.transform.position) < 3.0) {
                this.HP -= 1;
            }
        }
        if (this.HP <= 0) {
            AudioClip clip = (AudioClip)Resources.Load("Sounds/door_break");
            GameObject.Find("AudioPlayer").audio.PlayOneShot(clip);
            Destroy(this.gameObject);
        }
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
        if (!this.isOpeaning) {
            this.isOpeaning = true;
            this.audio.Play();
        }
    }
    
    public void Close () {
        if (this.isOpeaning) {
            this.isOpeaning = false;
        }
    }
    
    public bool IsOpen () {
        return this.isOpeaning;
    }
}
