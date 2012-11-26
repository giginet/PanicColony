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

    void Start () {
        this.aiPath = this.gameObject.GetComponent<AIPath>();
        this.aiPath.target = GameObject.FindWithTag("Player").transform;
    }
    
    void Update () {
        if (this.state == EnemyState.Normal) {
            this.aiPath.enabled = true;
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
 
}
