using UnityEngine;
using System.Collections;

public class Sneak : MonoBehaviour, IGvrGazeResponder {
	private Animator anim;
	void Awake () {
		anim = GetComponent<Animator> ();
	}
	
	public void OnGazeEnter () {
		anim.SetTrigger ("Spotted");
	}

	public void OnGazeStay () {
	}

	public void OnGazeExit () {
	}

	public void OnGazeTrigger () {
	}
}
