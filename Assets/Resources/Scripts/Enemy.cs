using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

public class Enemy : MonoBehaviour {

    enum EnemyState {
        Normal,
        Damage,
        Shocking,
        Death
    }
    
    enum EnemyActionState {
        Follow,
        Search,
        Escape,
        Wait
    }
    
     enum EnemyAI {
        Normal,
        Wait,
        Escape
    }
    
    public float normalSpeed = 5.0f;
    public float fastSpeed = 15.0f;
    public float shockDuration = 3.0f;
    public float rotationSpeed = 2.0f;
    private LevelManager levelManager = null;
    private float shockTime = 0;
    private EnemyAI ai = EnemyAI.Normal;
    private EnemyActionState actionState = EnemyActionState.Search;
    private EnemyState state = 0;
    private AIPath aiPath;
    private Vector3 initialPosition;

    void Awake () {
        this.aiPath = this.gameObject.GetComponent<AIPath>();
        this.initialPosition = this.transform.position;
        this.levelManager = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();
        this.aiPath.enabled = false;
    }
    
    void Start () {
        this.ChangeTarget();
    }
    
    void Reset (){
        this.transform.rotation = Quaternion.identity;
        this.transform.position = this.initialPosition;
    }
    
    void Update () {
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
        this.Action();
        if (this.aiPath.target == null || this.IsNearTarget()) {
            this.ChangeTarget();
        }
        Ray ray = new Ray(this.transform.position, this.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, this.GetComponent<CharacterController>().radius)) {
            if (hit.collider.gameObject.CompareTag("Enemy")) {
                this.ChangeTarget();
            }
        }
    }
    
    public void Shock() {
        this.state = EnemyState.Shocking;
        this.actionState = EnemyActionState.Follow;
        this.shockTime = 0;
    }
    
    public void Death () {
        Destroy(this.gameObject);
        GameObject radar = GameObject.FindWithTag("Radar");
        radar.SendMessage("DestroyChip", this.gameObject);
    }
    
    private void Action () {
        if (this.actionState == EnemyActionState.Search) {
            this.aiPath.speed = this.normalSpeed;
            GameObject player = this.GetSameUnitPlayer();
            if (player != null) {
                this.actionState = EnemyActionState.Follow;
                this.ChangeTarget();
            }
        } else if (this.actionState == EnemyActionState.Follow) {
            this.aiPath.speed = this.fastSpeed;
            GameObject player = this.GetNearestPlayer();
            if (player != null && Vector3.Distance(this.transform.position, player.transform.position) > 50) {
                this.actionState = EnemyActionState.Search;                
                this.ChangeTarget();
            }
        } else if (this.actionState == EnemyActionState.Wait) {
            this.aiPath.speed = 0;
            GameObject player = this.GetSameUnitPlayer();
            if (player != null) {
                this.actionState = EnemyActionState.Search;
                this.ChangeTarget();
            }
        }
    }
     
    private void ChangeTarget () {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (this.actionState == EnemyActionState.Follow) {
            this.aiPath.target = this.GetNearestPlayer().transform;
        }else if (this.actionState == EnemyActionState.Search) {
            this.SetRandomRoom();
        }
    }
    
    private GameObject GetSameUnitPlayer () {
        // get the player who is in same room.
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players) {
            Unit playerUnit = levelManager.GetLevel().GetUnit(this.levelManager.PositionToMatrix(player.transform.position));
            Unit enemyUnit = levelManager.GetLevel().GetUnit(this.levelManager.PositionToMatrix(this.transform.position));
            if ( playerUnit != null && enemyUnit != null && playerUnit == enemyUnit) {
                return player;
            }
        }
        return null;
    }
    
    private GameObject GetNearestPlayer () {
        // Follow the nearest player
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 1) {
            return players[0];
        } else {
            if (Vector3.Distance(players[0].transform.position, this.transform.position) < Vector3.Distance(players[1].transform.position, this.transform.position)) {
                return players[0];
            } else {
                return players[1];
            }
        }
    }
    
    private void SetRandomRoom () {
        List<Room> candidate = new List<Room>();
        Room currentRoom = this.levelManager.GetRoom(this.transform.position);
        foreach (Room room in this.levelManager.GetLevel().GetRooms()) {
            if (room.IsEnable() && room != currentRoom) {
                candidate.Add (room);
            }
        }
        Room targetRoom = candidate[Random.Range(0, candidate.Count)];
        this.aiPath.target = this.levelManager.GetLevel().GetObject(targetRoom.GetCenter()).transform; 
    }
    
    public bool IsNearTarget () {
        GameObject target = this.aiPath.target.gameObject;
        return Vector3.Distance(this.transform.position, target.transform.position) < this.levelManager.WIDTH * 2;
    }
    
    public void SetCanMove (bool b) {
        this.aiPath.enabled = b;
        this.aiPath.canMove = b;
    }
    
}
