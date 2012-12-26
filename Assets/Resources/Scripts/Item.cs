using UnityEngine;
using System.Collections;

[RequireComponent (typeof (AudioSource))]
public class Item : MonoBehaviour {

    public enum VegetableType {
        Eggplant,
        Carrot,
        Turnip
    }
    
    public float rotateSpeed = 1.0f;
    public int baseScore = 500;
    public VegetableType type = VegetableType.Eggplant;

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
            int score = controller.GetCurrentLevel() * this.baseScore;
            controller.AddScore(0, score);
            Destroy(gameObject);
        }
    }
}

