using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

    public int initialLevel = 1;
    public int initialLife = 3;
    public int levelCount = 2;
    private GameState state = GameState.Start;
    private GameObject levelManager = null;
    private AudioSource audioPlayer = null;
    private int[] lives = {0, 0};
    private int currentLevel = 1;
    private int [] currentScores = {0, 0};
    private int [] targetScores = {0, 0};
    private List<GameObject> players;
    private float timer = 0;
    private Animation2D startAnimation;

    public enum GameState {
        Start,
        Main,
        Miss,
        GameOver,
        Clear
    };

    void Awake () {
        this.state = GameState.Start;
        this.levelManager = GameObject.FindWithTag("LevelManager");
        this.currentLevel = this.initialLevel;
        this.audioPlayer = GameObject.Find("AudioPlayer").GetComponent<AudioSource>();
        this.initialLevel = this.currentLevel;
        this.lives[0] = this.initialLife;
        this.lives[1] = this.initialLife;
        this.startAnimation = new Animation2D(new Rect((Screen.width - 800) / 2, (Screen.height - 600) / 2, 800, 600), "UI/Start/start", 57);
        this.Replay();
    }
    
    void Update () { 
        if (this.state == GameState.Start) {
            this.timer += Time.deltaTime;
            if (timer > 1.0f && !this.startAnimation.IsPlaying()) {
                this.startAnimation.Play();
            } else if (timer > 3.0f) {
                this.timer = 0;
                this.state = GameState.Main;
                this.SetCharacterCanMove(true);
            }
            this.startAnimation.Update();
        } else if (this.state == GameState.Main) {
            if (this.CheckClear()) {
                this.Clear();
            }
        } else if (this.state == GameState.Miss) {
            this.timer += Time.deltaTime;
            if (this.timer > 3.0) {
                this.timer = 0;
                this.Replay();
            }
        } else if (this.state == GameState.Clear) {
            this.timer += Time.deltaTime;
            if (this.timer > 6.0) {
                this.timer = 0;
                this.NextStage();
            }
        }
        for (int i = 0; i < this.players.Count; ++i) {
            this.UpdateScore(i);
        }
    }
    
    void UpdateScore(int player) {
        if (!this.audioPlayer.isPlaying) {
            if (this.currentScores[player] < this.targetScores[player]) {
                AudioClip clip = (AudioClip)Resources.Load ("Sounds/score");
                this.audioPlayer.PlayOneShot(clip);
                int sub = this.targetScores[player] - this.currentScores[player];
                int order = (int)Mathf.Log10(sub);
                this.currentScores[player] += (int)Mathf.Pow(10, order);
            } else {
                this.currentScores[player] = this.targetScores[player];
            }
        }
    }
    
    void OnGUI () {
        GUIStyle scoreStyle = new GUIStyle();
        scoreStyle.fontSize = 36;
        scoreStyle.alignment = TextAnchor.MiddleCenter;
        scoreStyle.normal.textColor = Color.yellow;
        if (this.state == GameState.Start) {
            if (this.startAnimation.GetTexture() != null) {
                GUI.DrawTexture(this.startAnimation.GetRect(), this.startAnimation.GetTexture(), ScaleMode.ScaleToFit, true, this.startAnimation.GetAspectRatio());
            }
        } else if (this.state == GameState.GameOver) {
            GUIStyle labelStyle = new GUIStyle();
            labelStyle.fontSize = 64;
            labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.normal.textColor = Color.white;
            GUIStyle shadowLabelStyle = new GUIStyle();
            shadowLabelStyle.fontSize = 64;
            shadowLabelStyle.alignment = TextAnchor.MiddleCenter;
            shadowLabelStyle.normal.textColor = Color.gray;
            int width = Screen.width;
            int height = Screen.height;
            GUI.Label(new Rect(width / 2 - 300 + 3, height / 2 - 200 + 3, 600, 400), "Game Over", shadowLabelStyle);
            GUI.Label(new Rect(width / 2 - 300, height / 2 - 200, 600, 400), "Game Over", labelStyle);
        }
   }
   
    void Replay () {
        this.players = new List<GameObject>();
        this.state = GameState.Main;
        Resources.UnloadUnusedAssets();
        this.levelManager.SendMessage("DestroyLevel");
        GameObject oldRadar = GameObject.FindWithTag("Radar");
        if (oldRadar) {
            Destroy(oldRadar);
        }
        this.levelManager.SendMessage("CreateLevel", this.currentLevel);
        GameObject radar = (GameObject)Resources.Load("Prefabs/radarPrefab");
        Instantiate(radar, Vector3.zero, Quaternion.identity);
        this.state = GameState.Start;
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
            this.players.Add(player);
        }
        this.startAnimation.Rewind();
        this.SetCharacterCanMove(false);
    }
    
    bool CheckClear () {
        return 0 == GameObject.FindGameObjectsWithTag("Enemy").Length;
    }
    
    void Miss (int player) {
        if (this.lives[player] > 0) {
            this.state = GameState.Miss;
            this.lives[player] -= 1;
        } else {
            this.state = GameState.GameOver;
        }
    }
    
    void NextStage () {
        this.currentLevel += 1;
        if (this.currentLevel > this.levelCount) {
            this.currentLevel = 1;
        }
        this.Replay();
    }
    
    void Clear () {
        this.state = GameState.Clear;
        AudioClip clip = (AudioClip)Resources.Load("Sounds/clear");
        this.audioPlayer.PlayOneShot(clip);
    }
    
    void GameOver () {
        this.state = GameState.GameOver; 
    }
    
    public int GetScore (int player) {
        return this.currentScores[player];
    }
    
    public void AddScore (int player, int score) {
        this.targetScores[player] += score;
    }
    
    public void BombEnemy (List<GameObject> enemies) {
        int count = enemies.Count;
        this.AddScore(0, 1000 * (int)Mathf.Pow(2, count));
    }
    
    public void SetCharacterCanMove (bool b) {
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy")) {
            enemy.SendMessage("SetCanMove", b);
        }
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
            player.SendMessage("SetControl", b);
        }
    }
}
