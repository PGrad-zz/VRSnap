using UnityEngine;
using System.Collections;

public class Popup : MonoBehaviour, IGvrGazeResponder {
	public GameObject monkey,
					  monkeyJoke;
	bool runOnce = false,
		 dropped = true;
	int yTranslate;

	void Start() {
		yTranslate = 10;
	}

	//Pull an object hidden under the ground into view ("spawning" without the cost of spawning)
	public void popUpOnce() {
		GameObject summoned;
		AudioSource sound;
		if (!runOnce) {
			foreach (string child in DataService.getChildren (gameObject.name)) {
				summoned = GameObject.Find (child);
				summoned.transform.Translate (new Vector3 (0, 10, 0));
				sound = summoned.GetComponent<AudioSource> ();
				if (sound != null)
					sound.Play ();
			}	
			runOnce = true;
		}
	}

	//"Spawn" (make visible) a hidden object for a short period of time.
	public void sleight() {
		if (dropped) 
			StartCoroutine (Switch ());
	}

	private void move() {
		monkey.transform.Translate (new Vector3 (0, yTranslate, 0));
		yTranslate = -yTranslate;
		dropped = false;
	}

	private IEnumerator Switch() {
		move ();
		yield return new WaitForSeconds (1.5f);
		move ();
		yield return new WaitForSeconds (0.05f);
		monkeyJoke.SetActive (true);
		yield return new WaitForSeconds (0.1f);
		monkeyJoke.SetActive (false);
		dropped = true;
	}

	public void OnGazeEnter () {
		sleight ();
	}

	public void OnGazeStay () {
	}

	public void OnGazeExit () {
	}

	public void OnGazeTrigger () {
	}
}
