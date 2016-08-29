using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SelectLevel : MonoBehaviour, IGvrGazeResponder {
	private GameObject back;
	private bool ready = false;
	private AudioSource planeFlying;
	private SceneTransition sceneTransition;

	void Awake () {
		planeFlying = GetComponent<AudioSource> ();
		sceneTransition = GetComponent<SceneTransition> ();
	}

	void Start () {
		back = transform.GetChild (1).gameObject;
		back.SetActive (false);
	}

	public void OnGazeEnter () {
	}

	public void OnGazeStay () {
	}

	public void OnGazeExit () {
		back.SetActive (false);
		ready = false;
	}

	public void OnGazeTrigger () {
		if (!ready) {
			back.SetActive (true);
			ready = true;
		} else {
			planeFlying.Play ();
			sceneTransition.Transition ();
		}
	}
}
