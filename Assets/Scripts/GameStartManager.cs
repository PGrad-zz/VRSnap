using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameStartManager : MonoBehaviour, IGvrGazeResponder {
	private static bool gameStarted = false;

	public void startLevel() {
		CameraReticle.shotsEnabled = true;
		GameObject.Find ("CameraReticle").GetComponent<CameraReticle> ().OnGazeExit (null, null);
		EventManager.TriggerEvent ("StartPlayerMove");
		Destroy (transform.parent.gameObject);
		gameStarted = true;
	}

	public static bool isGameStarted () {
		return gameStarted;
	}

	public static void GameRestarted () {
		gameStarted = false;
	}

	public void OnGazeTrigger () {
		startLevel ();
	}

	public void OnGazeEnter () {
	}

	public void OnGazeStay () {
	}

	public void OnGazeExit () {
	}
}
