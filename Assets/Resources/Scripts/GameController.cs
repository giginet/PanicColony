using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

    public int initialLevel = 1;
    public int initialLife = 3;
    public int vegetableBorder = 3;
    
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
    private List<Enemy> bombEnemies; 
    
    private Texture2D scoreLabelTexture = null;
    private Texture2D stageLabelTexture = null;
    private Texture2D lifeTexture = null;
    private NumberTexture scoreNumberTexture = null;
    private NumberTexture stageNumberTexture = null;
    
    private bool isVegetableSetted = false;

    public enum GameState {
        Start,
        Main,
        Miss,
        GameOver,
        Clear
    };

    void Awake () {
        this.scoreLabelTexture = (Texture2D)Resources.Load("UI/score");
        this.stageLabelTexture = (Texture2D)Resources.Load("UI/stage");
        this.lifeTexture = (Texture2D)Resources.Load ("UI/life");
        this.scoreNumberTexture = new NumberTexture("UI/numbers", 37, 50);
        this.stageNumberTexture = new NumberTexture("UI/numbers", 37, 50);
        
        this.state = GameState.Start;
        this.levelManager = GameObject.FindWithTag("LevelManager");
        this.currentLevel = this.initialLevel;
        this.audioPlayer = GameObject.Find("AudioPlayer").GetComponent<AudioSource>();
        this.initialLevel = this.currentLevel;
        this.lives[0] = this.initialLife;
        this.lives[1] = this.initialLife;
        this.startAnimation = new Animation2D(new Rect((Screen.width - 800) / 2, (Screen.height - 600) / 2, 800, 600), "UI/Start/start", 57);
        this.bombEnemies = new List<Enemy>();
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
            if (Input.GetButtonDown("Start")) {
                this.PlayMusic("Sounds/gameover1");
                if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0) {
                    this.NextStage();
                } else {
                    this.Replay();
                }
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
                if (!this.audioPlayer.isPlaying) {
                    this.audioPlayer.PlayOneShot(clip);
                }
                int sub = this.targetScores[player] - this.currentScores[player];
                int order = (int)Mathf.Log10(sub);
                if (order > 0) {
                    this.currentScores[player] += (int)Mathf.Pow(10, order - 1);
                } else {
                    this.currentScores[player] += 1;
                }
            } else {
                this.currentScores[player] = this.targetScores[player];
            }
        }
    }
    
    void OnGUI () {
        GUI.DrawTexture(new Rect(30, 30, this.stageLabelTexture.width / 2.0f, this.stageLabelTexture.height / 2.0f), this.stageLabelTexture, ScaleMode.ScaleToFit, true, this.stageLabelTexture.width / this.stageLabelTexture.height);
        this.stageNumberTexture.SetScale(0.75f);
        this.stageNumberTexture.Draw(new Vector2(30 + this.stageLabelTexture.width / 2.0f + 15, 27), this.currentLevel);
        GUI.DrawTexture(new Rect(280, 30, this.scoreLabelTexture.width / 2.0f, this.scoreLabelTexture.height / 2.0f), this.scoreLabelTexture, ScaleMode.ScaleToFit, true, this.scoreLabelTexture.width / this.scoreLabelTexture.height);
        this.scoreNumberTexture.SetScale(0.70f);
        this.scoreNumberTexture.Draw(new Vector2(280 + this.scoreLabelTexture.width / 2.0f + 15, 28), this.currentScores[0]);
        for (int i = 0; i <this.lives[0]; ++i) {
            GUI.DrawTexture(new Rect(30, 80 + (this.lifeTexture.height / 2.0f + 10) * i, this.lifeTexture.width / 2.0f, this.lifeTexture.height / 2.0f), this.lifeTexture);       
        }
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
        Resources.UnloadUnusedAssets();
        this.players = new List<GameObject>();
        this.state = GameState.Main;
        this.levelManager.SendMessage("DestroyLevel");
        GameObject oldRadar = GameObject.FindWithTag("Radar");
        if (oldRadar) {
            Destroy(oldRadar);
        }
        this.levelManager.GetComponent<LevelManager>().CreateLevel(this.currentLevel, this.bombEnemies);
        GameObject radar = (GameObject)Resources.Load("Prefabs/radarPrefab");
        Instantiate(radar, Vector3.zero, Quaternion.identity);
        this.state = GameState.Start;
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
            this.players.Add(player);
        }
        this.startAnimation.Rewind();
        this.timer = 0;
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
        this.bombEnemies = new List<Enemy>();
        this.isVegetableSetted = false;
        TextAsset stage = (TextAsset)Resources.Load ("Levels/Level" + this.currentLevel.ToString());
        if (stage == null) {
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
        if (count > 0) {
            this.AddScore(0, count * 1000 * (int)Mathf.Pow(2, count - 1));
        }
        foreach (GameObject enemy in enemies) {
            Enemy component = enemy.GetComponent<Enemy>();
            this.bombEnemies.Add (component);
        }
        // Add Vegetable to start Point
        if (!this.isVegetableSetted && this.bombEnemies.Count >= this.vegetableBorder) {
            LevelManager manager = this.levelManager.GetComponent<LevelManager>();
            Vector2 pos = manager.GetLevel().GetStartPoint(0);
            GameObject prefab;
            if (this.currentLevel % 3 == 1) {
                prefab = (GameObject)Resources.Load("Prefabs/eggplantPrefab", typeof(GameObject));
            } else if (this.currentLevel % 3 == 2) {
                prefab = (GameObject)Resources.Load("Prefabs/turnipPrefab", typeof(GameObject));
            } else {
                prefab = (GameObject)Resources.Load("Prefabs/carrotPrefab", typeof(GameObject));
            }
            this.isVegetableSetted = true;
            GameObject vegetable = (GameObject)Instantiate(prefab, manager.MatrixToPosition(pos), Quaternion.identity);
            vegetable.transform.parent = manager.GetLevelObject().transform;
        }
    }
    
    public void SetCharacterCanMove (bool b) {
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy")) {
            enemy.SendMessage("SetCanMove", b);
        }
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
            player.SendMessage("SetControl", b);
        }
    }
    
    public void PlayMusic (string fileName) {
        this.audioPlayer.Stop();
        AudioClip clip = (AudioClip)Resources.Load (fileName);
        this.audioPlayer.PlayOneShot(clip);
    }
    
    public int GetCurrentLevel () {
        return this.currentLevel;
    }
}
