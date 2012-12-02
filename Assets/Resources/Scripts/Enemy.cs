using UnityEngine;
using System.Collections;
using Pathfinding;

public class Enemy : MonoBehaviour {

    enum EnemyState {
        Normal,
        Damage,
        Shocking,
        Death
    }
    
    public float shockDuration = 3.0f;
    public float rotationSpeed = 2.0f;
    private float shockTime = 0;
    private EnemyState state = 0;
    private AIPath aiPath;
    private Vector3 initialPosition;

    void Awake () {
        this.aiPath = this.gameObject.GetComponent<AIPath>();
        this.aiPath.target = GameObject.FindWithTag("Player").transform;
        this.initialPosition = this.transform.position;
    }
    
    void Reset (){
        this.transform.rotation = Quaternion.identity;
        this.transform.position = this.initialPosition;
    }
    
    void Update () {
        this.aiPath = this.gameObject.GetComponent<AIPath>();
        this.aiPath.target = GameObject.FindWithTag("Player").transform;
        if (this.transform.position.y < -10) {
            Destroy(gameObject);
        }
        if (this.state == EnemyState.Normal) {
            this.aiPath.enabled = true;
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")){
                if (Vector3.Distance(player.transform.position, this.transform.position) < 2.5f) {
                    player.SendMessage("Death");
                }
            }
        } else if (this.state == EnemyState.Shocking) {
            this.transform.Rotate(Vector3.up * rotationSpeed);
            this.aiPath.enabled = false;
            this.shockTime += Time.deltaTime;
            if (this.shockTime > this.shockDuration) {
                this.shockTime = 0;
                this.state = EnemyState.Normal;
            }
        }
        
    }
    
    public void Shock() {
        this.state = EnemyState.Shocking;
        this.shockTime = 0;
    }
    
    public void Death () {
        Destroy(this.gameObject);
        GameObject radar = GameObject.FindWithTag("Radar");
        radar.SendMessage("DestroyChip", this.gameObject);
    }
 
}
