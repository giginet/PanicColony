using UnityEngine;
using System.Collections;

public class Exploder : MonoBehaviour {

    // Use this for initialization
    void Start () {
    
    }
    
    // Update is called once per frame
    void Update () {
    
    }
 
    void Explode () {
        LevelManager manager = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();
        Room room = manager.GetRoom(this.transform.position);
        if (room != null && !room.IsProtect()) {
            GameObject explosionPrefab = (GameObject)Resources.Load("Prefabs/bombExplosionPrefab");
            Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
            manager.BombRoom(room);
        } else {
            GameObject audioPlayer = GameObject.Find("AudioPlayer");
            AudioClip miss = (AudioClip)Resources.Load("Sounds/bomb_miss");
            audioPlayer.audio.PlayOneShot(miss);
        }
        Destroy(gameObject);
    }
    
    public void UpdateRadar () {
        GameObject radar = GameObject.FindWithTag("Radar");
        LevelManager manager = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();
        Room room = manager.GetRoom(this.transform.position);
        if (room != null && !room.IsProtect()) {
            radar.SendMessage("SetWarning", room);
        }
        this.transform.parent = manager.GetLevelObject().transform;
    }
}
