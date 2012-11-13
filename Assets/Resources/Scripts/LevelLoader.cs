using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour {
	public int WIDTH = 10;
	public int HEIGHT = 10;

	// Use this for initialization
	void Start () {
		this.CreateStage(0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void CreateStage(int levelNo) {
		TextAsset asset = (TextAsset)Resources.Load("Levels/Level" + levelNo.ToString(), typeof(TextAsset));
		GameObject level = GameObject.CreatePrimitive(PrimitiveType.Cube);
		level.renderer.enabled = false;
		level.name = "Level" + levelNo;
		string[] lines = asset.text.Split( '\n');
		for (int y = 0; y < lines.Length; ++y) {
			string line = lines[y];
			for (int x = 0; x < line.Length; ++x) {
				char c = line[x];
				Vector3 position = new Vector3(x * WIDTH, 0, -y * HEIGHT);
				if (c == '.' || c == 'S') {
					GameObject floorPrefab = (GameObject)Resources.Load("Prefabs/floorPrefab", typeof(GameObject));
					GameObject floor = (GameObject)Instantiate(floorPrefab, position, Quaternion.identity);
					floor.transform.parent = level.transform;
				}
				if (c == '/') {
					GameObject routePrefab = (GameObject)Resources.Load("Prefabs/routePrefab", typeof(GameObject));
					GameObject route = (GameObject)Instantiate(routePrefab, position, Quaternion.identity);
					route.transform.parent = level.transform;
				} else if (c == '#') {
					GameObject wallPrefab = (GameObject)Resources.Load("Prefabs/wallPrefab", typeof(GameObject));
					GameObject wall = (GameObject)Instantiate(wallPrefab, position + Vector3.up * 1, Quaternion.identity);
					wall.transform.parent = level.transform;
				} else if (c == 'S') {
					GameObject player = GameObject.FindWithTag("Player");
					player.transform.position = position + Vector3.up;
				}
			}
		}
	}
}
