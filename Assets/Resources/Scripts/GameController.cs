using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

    public int initialLevel = 1;
    private GameState state = GameState.Start;
    private GameObject levelManager = null;
    private AudioSource audioPlayer = null;
    private int[] scores = {0, 0};
    private int currentLevel = 1;

    public enum GameState {
        Start,
        Main,
        GameOver,
        Clear
    };

    void Awake () {
        this.state = GameState.Start;
        this.levelManager = GameObject.FindWithTag("LevelManager");
        this.audioPlayer = GameObject.Find("AudioPlayer").GetComponent<AudioSource>();
        this.initialLevel = this.currentLevel;
        this.Replay();
    }
    
    void Update () { 
        if (this.state == GameState.Start) {
            this.state = GameState.Main;
        } else if (this.state == GameState.Main) {
            if (this.CheckClear()) {
                this.Clear();
            }
        }
    }
    
    void OnGUI () {
        if (this.state == GameState.GameOver) {
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
            if (GUI.Button( new Rect(width / 2 - 210, height / 2 + 100, 200, 60), "Replay(Space)")) {
                this.Replay();
            } else if (GUI.Button( new Rect(width / 2 + 10, height / 2 + 100, 200, 60), "Exit")) {
                Application.Quit();
            }
        }
   }
   
    void Replay () {
        this.levelManager.SendMessage("DestroyLevel");
        GameObject oldRadar = GameObject.FindWithTag("Radar");
        if (oldRadar) {
            Destroy(oldRadar);
        }
        this.levelManager.SendMessage("CreateLevel", this.currentLevel);
        GameObject radar = (GameObject)Resources.Load("Prefabs/radarPrefab");
        Instantiate(radar, Vector3.zero, Quaternion.identity);
        this.state = GameState.Start;
    }
    
    bool CheckClear () {
        return 0 == GameObject.FindGameObjectsWithTag("Enemy").Length;
    }
    
    void Clear () {
        this.state = GameState.Clear;
        AudioClip clip = (AudioClip)Resources.Load("Sounds/clear");
        this.audioPlayer.PlayOneShot(clip);
    }
    
    void GameOver () {
        this.state = GameState.GameOver; 
    }
    
    int GetScore (int player) {
        return this.scores[player];
    }
    
    void AddScore (int player, int score) {
        this.scores[player] += score;
    }
}
