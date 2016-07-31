using UnityEngine;
using System.Collections;

public class Quit : MonoBehaviour, IGvrGazeResponder {
	public void QuitGame () {
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#else
			Application.Quit ();
		#endif
	}

	public void OnGazeEnter () {
	}

	public void OnGazeStay () {
	}

	public void OnGazeExit () {
	}

	public void OnGazeTrigger () {
		QuitGame ();
	}
}
