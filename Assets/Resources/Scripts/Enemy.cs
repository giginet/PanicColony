using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

public class Enemy : MonoBehaviour {

    public enum EnemyState {
        Normal,
        Damage,
        Attack,
        Shocking,
        Death,
        PlayerDeathIntro,
        PlayerDeath
    }
    
    enum EnemyActionState {
        Follow,
        Search,
        Escape,
        Wait
    }
    
    public enum EnemyAI {
        Normal,
        Wait,
        Escape
    }
    
    public float normalSpeed = 5.0f;
    public float fastSpeed = 15.0f;
    public float shockDuration = 3.0f;
    public float rotationSpeed = 2.0f;
    public int score = 1000;
    public EnemyAI ai = EnemyAI.Normal;
    public float attackRange = 1.5f;
    private float shockTime = 0;
    private EnemyActionState actionState = EnemyActionState.Search;
    private EnemyState state = 0;
    private Vector2 initialPosition;    
    private GameObject worm = null;
    private int stopCount = 0;
    protected AIPath aiPath;
    protected LevelManager levelManager = null;
    protected Unit lastUnit = null;

    void Awake () {
        this.aiPath = this.gameObject.GetComponent<AIPath> ();
        this.levelManager = GameObject.FindWithTag ("LevelManager").GetComponent<LevelManager> ();
        this.initialPosition = this.levelManager.PositionToMatrix (this.transform.position);
        this.aiPath.enabled = false;
        Transform wormTransform = this.transform.Find("worm");
        if (wormTransform != null) {
                this.worm = wormTransform.gameObject;
        }
    }
    
    virtual protected void Start () {
        this.ChangeTarget ();
    }
    
    void Reset () {
        this.transform.rotation = Quaternion.identity;
        this.transform.position = this.initialPosition;
    }
    
    void AttackGate (Gate gate) {
        if (this.state != EnemyState.Attack) {
            this.state = EnemyState.Attack;
            StartCoroutine(this.PlayAttackGateMotion(gate));
        }
    }
    
    virtual protected IEnumerator PlayAttackGateMotion (Gate gate) { 
        this.worm.animation.Play("attack");
        yield return new WaitForSeconds(this.worm.animation["attack"].length / 1.5f);
        if (gate != null) {
            gate.gameObject.SendMessage("Damage", 1);
            this.state = EnemyState.Normal;
        }
    }
    
    protected virtual void Update () {
        if (this.transform.position.y < -10) {
            this.Death();
        }
        if (this.state == EnemyState.Normal) {
            this.aiPath.enabled = true;
            if (!this.Attack()) {
                if (!this.worm.animation.IsPlaying("walk")) {
                    this.worm.animation.CrossFade("walk");
                }
            }
            
        } else if (this.state == EnemyState.Shocking) {
            this.transform.Rotate (Vector3.up * rotationSpeed);
            this.aiPath.enabled = false;
            this.shockTime += Time.deltaTime;
            if (this.shockTime > this.shockDuration) {
                this.shockTime = 0;
                this.state = EnemyState.Normal;
            }
        } else if (this.state == EnemyState.PlayerDeath) {
            if (this.worm != null && !this.worm.animation.IsPlaying("win")) {
                this.worm.animation.CrossFade("win");
            }
        }
        this.ChangeState();
        this.Action ();
        if (this.aiPath.target == null || this.IsNearTarget ()) {
            this.ChangeTarget ();
        }
        Ray ray = new Ray (this.transform.position, this.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast (ray, out hit, this.GetComponent<CharacterController> ().radius)) {
            if (hit.collider.gameObject.CompareTag ("Enemy")) {
                this.ChangeTarget ();
            }
        }
        Unit currentUnit = this.levelManager.GetUnit(this.transform.position);
        if (this.lastUnit != null && currentUnit == this.lastUnit ) {
            this.stopCount += 1;
            if (this.stopCount > 120) {
               this.stopCount = 0;
               this.SetRandomRoom();
               this.aiPath.TrySearchPath();
            }
        }
        this.lastUnit = currentUnit; 
        this.OnPlayerIsDead(); 
    }
    
    virtual public void OnPlayerIsDead () {
        // win motion (when player is dead.)
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
            if (player.GetComponent<Player>().IsDead() && this.state != EnemyState.PlayerDeathIntro && this.state != EnemyState.PlayerDeathIntro) {
                this.state = EnemyState.PlayerDeathIntro;
                this.aiPath.canMove = false;
                StartCoroutine(this.PlayWinMotion());
            }
        }
    }
    
    virtual public bool Attack () {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
            if (Vector3.Distance (player.transform.position, this.transform.position) < this.attackRange) {
                if (this.state != EnemyState.Attack && !this.worm.animation.IsPlaying("attack") && player.GetComponent<Player>().GetPlayerState() == Player.PlayerState.Normal) {
                    this.state = EnemyState.Attack;
                    StartCoroutine(this.PlayAttackMotion(player));
                    return true;
                }
            }
        }
        return false;
    }
    
    IEnumerator PlayAttackMotion (GameObject player) {
        this.worm.animation.CrossFade("attack"); 
        yield return new WaitForSeconds(this.worm.animation["attack"].length / 1.5f);
        if (Vector3.Distance (player.transform.position, this.transform.position) < this.attackRange) {
            player.SendMessage ("Death", true);
           this.state = EnemyState.PlayerDeathIntro;             
            StartCoroutine(this.PlayWinMotion());
        }
        this.state = EnemyState.Normal;
    }
    
    virtual protected IEnumerator PlayWinMotion () {
        this.worm.animation.Play("win_intro");
        yield return new WaitForSeconds(this.worm.animation["win_intro"].length);
        this.state = EnemyState.PlayerDeath;             
    }
    
    virtual public void Shock () {
        this.state = EnemyState.Shocking;
        this.SetActionState(EnemyActionState.Follow);
        this.shockTime = 0;
        this.worm.animation.Stop("walk");
        this.worm.animation.Stop("attack");
        this.worm.animation.Stop("win_intro");
        this.worm.animation.Stop("win");
        StopCoroutine("PlayAttackMotion");
        StopCoroutine("PlayGateAttackMotion");
    }
    
    virtual public void Death () {
        Destroy (this.gameObject); 
        GameObject controller = GameObject.FindWithTag("GameController");
        controller.SendMessage("DestroyEnemy", this.gameObject);
    }
    
    private void ChangeState () {
        if (this.ai == EnemyAI.Normal) {
             
        } else if (this.ai == EnemyAI.Wait) {
            if (this.actionState == EnemyActionState.Wait) {
                this.aiPath.speed = 0;
                GameObject player = this.GetSameUnitPlayer ();
                if (player != null) {
                    this.SetActionState(EnemyActionState.Search);
                }
            }
        } else if (this.ai == EnemyAI.Escape) {
            GameObject player = this.GetNearestPlayer();
            Unit playerUnit = this.levelManager.GetUnit(player.transform.position); 
            if (playerUnit == this.levelManager.GetUnit(this.transform.position)) {
                this.SetActionState(EnemyActionState.Escape);
            }
        }
    }
    
    private void Action () {
        if (this.actionState == EnemyActionState.Search) {
            this.aiPath.speed = this.normalSpeed;
            GameObject player = this.GetSameUnitPlayer ();
            if (player != null) {
                this.SetActionState(EnemyActionState.Follow);
            }
        } else if (this.actionState == EnemyActionState.Follow) {
            this.aiPath.speed = this.fastSpeed;
            GameObject player = this.GetNearestPlayer ();
            if (player != null) {
                if (Vector3.Distance (this.transform.position, player.transform.position) > 100) {
                    this.actionState = EnemyActionState.Search;                
                    this.SetActionState(EnemyActionState.Search);
                } else {
                    this.aiPath.target = player.transform;
                }
            }
        } else if (this.actionState == EnemyActionState.Escape) {
            GameObject player = this.GetNearestPlayer();
            Unit playerUnit = this.levelManager.GetUnit(player.transform.position);
            if (playerUnit == this.levelManager.GetUnit(this.transform.position)) {
                this.aiPath.speed = this.fastSpeed;
                if (this.aiPath.target && playerUnit == this.levelManager.GetUnit(this.aiPath.target.position)) {
                    this.SetRandomRoom();
                }
            }
        }
    }
    
    private void SetActionState (EnemyActionState aState) {
        this.actionState = aState;
        this.ChangeTarget();
    }
     
    private void ChangeTarget () {
        GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
        if (this.actionState == EnemyActionState.Follow) {
            this.aiPath.target = this.GetNearestPlayer ().transform;
            this.aiPath.TrySearchPath();
        } else if (this.actionState == EnemyActionState.Search) {
            this.SetRandomRoom ();
        }
    }
    
    private GameObject GetSameUnitPlayer () {
        // get the player who is in same room.
        GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
        foreach (GameObject player in players) {
            Unit playerUnit = levelManager.GetLevel ().GetUnit (this.levelManager.PositionToMatrix (player.transform.position));
            Unit enemyUnit = levelManager.GetLevel ().GetUnit (this.levelManager.PositionToMatrix (this.transform.position));
            if (playerUnit != null && enemyUnit != null && playerUnit == enemyUnit) {
                return player;
            }
        }
        return null;
    }
    
    private GameObject GetNearestPlayer () {
        // Follow the nearest player
        GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
        if (players.Length == 1) {
            return players [0];
        } else {
            if (Vector3.Distance (players [0].transform.position, this.transform.position) < Vector3.Distance (players [1].transform.position, this.transform.position)) {
                return players [0];
            } else {
                return players [1];
            }
        }
    }
    
    private void SetRandomRoom () {
        List<Room> candidate = new List<Room> ();
        Room currentRoom = this.levelManager.GetRoom (this.transform.position);
        foreach (Room room in this.levelManager.GetLevel().GetRooms()) {
            if (room.IsEnable () && room != currentRoom) {
                candidate.Add (room);
            }
        }
        Room targetRoom = candidate [Random.Range (0, candidate.Count)];
        this.aiPath.target = this.levelManager.GetLevel ().GetObject (targetRoom.GetCenter ()).transform; 
    }
    
    public bool IsNearTarget () {
        GameObject target = this.aiPath.target.gameObject;
        return Vector3.Distance (this.transform.position, target.transform.position) < this.levelManager.WIDTH * 2;
    }
    
    public void SetCanMove (bool b) {
        this.aiPath.enabled = b;
        this.aiPath.canMove = b;
    }
    
    public Vector2 GetInitialPosition () {
        return this.initialPosition;
    }
    
}
