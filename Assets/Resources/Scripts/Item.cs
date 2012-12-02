using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {
    public float rotateSpeed = 1.0f;

    // Use this for initialization
    void Start () {
        this.transform.position += Vector3.up;
    
    }
    
    // Update is called once per frame
    void Update () {
        this.transform.Rotate(Vector3.up * this.rotateSpeed); 
        Color c = this.GetComponentInChildren<Renderer>().material.color;
        c.a = 0.5f;
        this.GetComponentInChildren<Renderer>().material.color = c;
    }
    
    void OnTriggerEnter (Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            GameObject player = GameObject.Find("AudioPlayer");
            AudioClip clip = (AudioClip)Resources.Load("Sounds/get", typeof(AudioClip));
            player.audio.PlayOneShot(clip);
            GameController controller = GameObject.FindWithTag("GameController").GetComponent<GameController>();
            controller.AddScore(0, 9999);
            Destroy(gameObject);
        }
    }
}
