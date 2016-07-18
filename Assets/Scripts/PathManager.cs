using UnityEngine;
using System.Collections;

public class PathManager : MonoBehaviour {
	public GameObject baseTerrain;
	public int numTers;
	private Vector3 shift;
	private float terWidth;
	private GameObject[] terQueue;
	private int front = 0;

	void Start () {
		Terrain terrain;
		Vector3 size;
		float terLen;
		if (baseTerrain != null) {
			terrain = baseTerrain.GetComponent<Terrain> ();
			if (terrain != null) {
				size = terrain.terrainData.size;
				terWidth = size.x;
				terLen = size.z / 2;
				terQueue = new GameObject[numTers];
				for (int ter = 0; ter < numTers; ter++)
					terQueue[ter] = (GameObject) Instantiate (baseTerrain, new Vector3 (terWidth * ter, 0.0f, -terLen), Quaternion.identity);
				shift = new Vector3 (terWidth * numTers, 0.0f, 0.0f);
			}
		}
	}

	void Update() {
		if (terQueue [front].transform.position.x == -terWidth) {
			terQueue [front].transform.Translate (shift);
			front = ++front % numTers;
		}
	}
}
