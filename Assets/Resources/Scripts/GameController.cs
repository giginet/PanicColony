using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {
 
    public enum DeathType {
        Broken,
        OuterSpace
    };
    
    public int initialLevel = 1;
    public int initialLife = 3;
    public int vegetableBorder = 3;
    public int roomBonus = 3000;
    public int maxLife = 15;
    public int[] tutorialPages = {};
    public float maxVolume = 0.2f;
    
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
    private List<Enemy> destroyedEnemies; 
    private DeathType deathType = DeathType.Broken;
    
    private Texture2D scoreLabelTexture = null;
    private Texture2D stageLabelTexture = null;
    private Texture2D lifeTexture = null;
    private Texture2D continueTexture = null;
    private Texture2D gameOverTexture = null;
    private Texture2D brokenTexture = null;
    private Texture2D outerTexture = null;
    private NumberTexture scoreNumberTexture = null;
    private NumberTexture stageNumberTexture = null;
    private TutorialWindow tutorialWindow = null;
    private bool isLoading = false;
    
    private bool isVegetableSetted = false;

    public enum GameState {
        Start,
        Tutorial,
        Main,
        Miss,
        GameOver,
        Clear,
        Loading,
        Pause
    };

    void Awake () {
        this.scoreLabelTexture = (Texture2D)Resources.Load("UI/score");
        this.stageLabelTexture = (Texture2D)Resources.Load("UI/stage");
        this.gameOverTexture = (Texture2D)Resources.Load("UI/gameover");
        this.brokenTexture = (Texture2D)Resources.Load("UI/broken");
        this.outerTexture = (Texture2D)Resources.Load("UI/outer");
        this.lifeTexture = (Texture2D)Resources.Load ("UI/life");
        this.continueTexture = (Texture2D)Resources.Load ("UI/continue");
        this.scoreNumberTexture = new NumberTexture("UI/numbers", 37, 50);
        this.stageNumberTexture = new NumberTexture("UI/numbers", 37, 50);
        this.audio.volume = this.maxVolume;
        
        this.state = GameState.Start;
        this.levelManager = GameObject.FindWithTag("LevelManager");
        this.currentLevel = this.initialLevel;
        this.audioPlayer = GameObject.Find("AudioPlayer").GetComponent<AudioSource>();
        this.initialLevel = this.currentLevel;
        this.lives[0] = this.initialLife;
        this.lives[1] = this.initialLife;
        this.startAnimation = new Animation2D(new Rect((Screen.width - 800) / 2, (Screen.height - 600) / 2, 800, 600), "UI/Start/start", 57);
        this.destroyedEnemies = new List<Enemy>();
        this.Replay();
    }
     
    void Update () { 
        if (this.state == GameState.Start) {  
            this.startAnimation.Update();
        } else if (this.state == GameState.Tutorial) {
            if (Input.anyKeyDown || Input.GetButtonDown("Start")) {
                this.tutorialWindow.Next();
            }
            if (this.tutorialWindow.GetPage() >= this.tutorialPages[this.currentLevel]) {
                this.state = GameState.Main;
                this.SetCharacterCanMove(true);
            }
        } else if (this.state == GameState.Main) {
            if (this.CheckClear()) {
                StartCoroutine(this.Clear());
            }
        } else if (this.state == GameState.Miss) {
           StartCoroutine(this.PlayGameOverSound());
        } else if (this.state == GameState.GameOver) {
           StartCoroutine(this.PlayGameOverSound());
        }
        for (int i = 0; i < this.players.Count; ++i) {
            this.UpdateScore(i);
        }
    }
    
    IEnumerator OnStartState () {
        yield return new WaitForSeconds(1.0f);
        this.startAnimation.Play();
        yield return new WaitForSeconds(3.4f);
        StartCoroutine(this.PlayMainMusic());
        if (this.currentLevel < this.tutorialPages.Length) {
            this.state = GameState.Tutorial;
            string prefix = "UI/tutorial/tutorial" + this.currentLevel.ToString() + "_";
            this.tutorialWindow = new TutorialWindow(prefix, this.tutorialPages[this.currentLevel], new Rect((Screen.width - 960) / 2.0f, (Screen.height - 720) / 2.0f, 960, 720));
        } else {
            this.state = GameState.Main;
            this.SetCharacterCanMove(true);
        }
    }
    
    IEnumerator PlayGameOverSound () {
        if ((Input.GetButtonDown("Start") || Input.GetKeyDown(KeyCode.Space)) && !this.isLoading) {
            this.isLoading = true;
            this.PlaySound("Sounds/gameover1");
            yield return new WaitForSeconds(2.5f);
            if (this.state == GameState.GameOver) {
                Application.LoadLevel("TitleScene");
            } else {
                if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0) {
                    this.NextStage();
                } else {
                    this.Replay();
                }
            }
            this.isLoading = false;
        }
    }
    
    void UpdateScore(int player) {
        if (!this.audioPlayer.isPlaying) {
            if (this.currentScores[player] < this.targetScores[player]) {
                int before = this.currentScores[player];
                /*if (!this.audioPlayer.isPlaying) {
                    AudioClip clip = (AudioClip)Resources.Load ("Sounds/score");
                    this.audioPlayer.PlayOneShot(clip);
                }*/
                int sub = this.targetScores[player] - this.currentScores[player];
                int order = (int)Mathf.Log10(sub);
                if (order > 0) {
                    this.currentScores[player] += (int)Mathf.Pow(10, order - 1);
                } else {
                    this.currentScores[player] += 1;
                }
                int after = this.currentScores[player];
                // extend
                if (this.lives[player] <= this.maxLife) {
                    if ((before < 20000 && after >= 20000) || (Mathf.Floor((before - 20000) / 40000) < Mathf.Floor((after - 20000) / 40000))) {
                        AudioClip clip = (AudioClip)Resources.Load ("Sounds/extend");
                        this.audioPlayer.PlayOneShot(clip);
                        this.lives[player] += 1;
                    }
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
        } else if (this.state == GameState.Tutorial) {
            this.tutorialWindow.Draw();
        } else if (this.state == GameState.Miss) {
            float alpha = 1.0f;
            alpha = 0.25f + Mathf.PingPong(Time.time / 2.0f, 0.75f);
            Color previous = GUI.color;
            int width = this.continueTexture.width;
            int height = this.continueTexture.height;
            if (this.deathType == DeathType.Broken) {
                GUI.DrawTexture(new Rect((Screen.width - width) / 2.0f, (Screen.height - height) / 4.0f, width, height), this.brokenTexture);
            } else {
                GUI.DrawTexture(new Rect((Screen.width - width) / 2.0f, (Screen.height - height) / 4.0f, width, height), this.outerTexture);
            }
            Color color = new Color(previous.r, previous.g, previous.b, alpha);
            GUI.color = color;
            GUI.DrawTexture(new Rect((Screen.width - width) / 1.2f, (Screen.height - height) / 1.2f, width, height), this.continueTexture);
            GUI.color = previous;
        } else if (this.state == GameState.GameOver) { 
            int width = this.gameOverTexture.width;
            int height = this.gameOverTexture.height;
            GUI.DrawTexture(new Rect((Screen.height - width) / 2 , (Screen.height - height) / 2, width, height), this.gameOverTexture);
            float alpha = 1.0f;
            alpha = 0.25f + Mathf.PingPong(Time.time / 2.0f, 0.75f);
            Color previous = GUI.color;
            Color color = new Color(previous.r, previous.g, previous.b, alpha);
            GUI.color = color;
            width = this.continueTexture.width;
            height = this.continueTexture.height;
            GUI.DrawTexture(new Rect((Screen.width - width) / 2.0f, (Screen.height - height) / 1.2f, width, height), this.continueTexture);
            GUI.color = previous;
        } else if (this.state == GameState.Pause) {
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
            GUI.Label(new Rect(width / 2 - 300 + 3, height / 2 - 200 + 3, 600, 400), "Pause", shadowLabelStyle);
            GUI.Label(new Rect(width / 2 - 300, height / 2 - 200, 600, 400), "Pause", labelStyle);
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
        this.levelManager.GetComponent<LevelManager>().CreateLevel(this.currentLevel, this.destroyedEnemies);
        GameObject radar = (GameObject)Resources.Load("Prefabs/radarPrefab");
        Instantiate(radar, Vector3.zero, Quaternion.identity);
        this.state = GameState.Start;
        this.StartCoroutine(this.OnStartState());
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
            this.players.Add(player);
        }
        this.startAnimation.Rewind();
        this.timer = 0;
        this.SetCharacterCanMove(false);
        this.audio.clip = (AudioClip)Resources.Load("Sounds/start");
        this.audio.Play();
    }
    
    bool CheckClear () {
        return 0 == GameObject.FindGameObjectsWithTag("Enemy").Length;
    }
    
    void Miss (int player) {
        if (this.state == GameState.Main) {
            if (this.lives[player] > 0) {
                this.state = GameState.Miss;
                this.lives[player] -= 1;
            } else {
                this.state = GameState.GameOver; 
                this.StartCoroutine(this.PlayGameOverLoop());
            }
        }
    }
    
    IEnumerator PlayGameOverLoop () {
        yield return new WaitForSeconds(1.5f);
        AudioClip clip = (AudioClip)Resources.Load("Sounds/gameover_loop");
        this.audio.clip = clip;
        this.audio.loop = true;
        this.audio.Play(); 
    }
    
    void NextStage () {
        this.currentLevel += 1;
        this.destroyedEnemies = new List<Enemy>();
        this.isVegetableSetted = false;
        TextAsset stage = (TextAsset)Resources.Load ("Levels/Level" + this.currentLevel.ToString());
        if (stage == null) {
            this.currentLevel = 1;
        }
        this.Replay();
    }
    
    IEnumerator Clear () {
        this.state = GameState.Clear;
        const int count = 5;
        for (int i = 0; i < count; ++i) {
            this.audio.volume -= this.maxVolume / (float)count;
            yield return new WaitForSeconds(0.1f);
        }
        this.StopMainMusic();
        this.audio.volume = this.maxVolume;
        AudioClip intro = (AudioClip)Resources.Load("Sounds/clear_intro", typeof(AudioClip));
        this.audio.clip = intro;
        this.audio.Play();
        yield return new WaitForSeconds(intro.length - 1.0f);
        GameObject.FindWithTag("Player").SendMessage("SetClearState");
        int sum = 0;
        int remain = 0;
        foreach (Room room in this.levelManager.GetComponent<LevelManager>().GetLevel().GetRooms()) { 
            if (!room.IsProtect()) {
                int area = room.GetFloors().Count;
                sum += area;
                if (room.IsEnable()) {
                    remain += area;
                }
            }
        }
        float percent = (float)remain / (float)sum;
        int score = (int)(this.roomBonus * percent);
        this.AddScore(0, score);
        yield return new WaitForSeconds(7.0f);        
        this.NextStage();
    }
    
    public int GetScore (int player) {
        return this.currentScores[player];
    }
    
    public void AddScore (int player, int score) {
        this.targetScores[player] += score;
        int hiscore = PlayerPrefs.GetInt("HiScore");;
        if (hiscore < targetScores[player]) {
            PlayerPrefs.SetInt("HiScore", targetScores[player]);;
        }
    }
    
    
    public void DestroyEnemy (List<GameObject> enemies) { 
        int count = enemies.Count; 
        // Add voice
        if (count == 3) {
            AudioClip joy = (AudioClip)Resources.Load("Sounds/joy0");
            this.audioPlayer.audio.PlayOneShot(joy);
        } else if (count == 4) {
            AudioClip joy = (AudioClip)Resources.Load("Sounds/joy1");
            this.audioPlayer.audio.PlayOneShot(joy);
        } else if (count >= 5) {
            AudioClip joy = (AudioClip)Resources.Load("Sounds/joy2");
            this.audioPlayer.audio.PlayOneShot(joy);
        }
        foreach (GameObject enemy in enemies) {
            if (count > 0) {
                Enemy enemyComponent = enemy.GetComponent<Enemy>();
                this.AddScore(0, enemyComponent.score * (int)Mathf.Pow(1.5f, count - 1));
            }
            this.DestroyEnemy(enemy);
        }
    }
    
    public GameState GetState () {
        return this.state;
    }
    
    public void DestroyEnemy (GameObject enemy) { 
        Enemy component = enemy.GetComponent<Enemy>();
        if (destroyedEnemies.Contains(component)) return;
        GameObject radar = GameObject.FindWithTag ("Radar");
        radar.SendMessage ("DestroyChip", enemy);
        this.destroyedEnemies.Add(component);
        // Add Vegetable to start Point
        if (!this.isVegetableSetted && this.destroyedEnemies.Count >= this.vegetableBorder) {
            LevelManager manager = this.levelManager.GetComponent<LevelManager>();
            Vector2 pos = manager.GetLevel().GetStartPoint(0);
            GameObject prefab;
            if (this.currentLevel % 3 == 2) {
                prefab = (GameObject)Resources.Load("Prefabs/eggplantPrefab", typeof(GameObject));
            } else if (this.currentLevel % 3 == 1) {
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
    
    public void PlaySound (string fileName) {
        this.audio.volume = this.maxVolume;
        this.audio.Stop();
        StopCoroutine("StopMainMusic");
        AudioClip clip = (AudioClip)Resources.Load(fileName);
        this.audio.clip = clip;
        this.audio.PlayOneShot(clip);
    }
    
    public int GetCurrentLevel () {
        return this.currentLevel;
    }
    
    IEnumerator PlayMainMusic () {
        AudioClip intro = (AudioClip)Resources.Load("Sounds/main_intro");
        this.audio.clip = intro;
        this.audio.Play();
        yield return new WaitForSeconds(this.audio.clip.length);
        if (this.audio.clip == intro) {
            this.audio.clip = (AudioClip)Resources.Load("Sounds/main_loop");
            this.audio.loop = true;
            this.audio.Play();
        }
    }
    
    void StopMainMusic () {
        StopCoroutine("PlayMainMusic");
        this.audio.Stop();
        this.audio.clip = null; 
        this.audio.loop = false;
    }
    
    void SetDeathType (DeathType type) {
        this.deathType = type;
    }
}
