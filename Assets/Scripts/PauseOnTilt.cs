using UnityEngine;
using System.Collections;

public class PauseOnTilt : Singleton <PauseOnTilt> {
	public GameObject PausePanel;
	public static bool gameOver = false;
	private readonly float tiltLimit = Mathf.Cos (Mathf.PI / 3);
	private bool tilted = false,
				 paused = false;
	private float cosOfplayerHeadZAngle;

	void Update () {
		cosOfplayerHeadZAngle = Mathf.Cos (transform.eulerAngles.z * Mathf.PI / 180);
		if (!tilted && cosOfplayerHeadZAngle < tiltLimit) {
			tilted = true;
			if (!gameOver && GameStartManager.isGameStarted ()) {
				if (!paused) 
					PauseGame ();
				else 
					ResumeGame ();
			}
		}
		if (tilted && cosOfplayerHeadZAngle > tiltLimit) 
			tilted = false;
	}

	void OnApplicationPause (bool paused) {
		if (paused && GameStartManager.isGameStarted ())
			PauseGame ();
	}

	public static void PauseGame () {
		Instance.PausePanel.SetActive (true);
		EventManager.TriggerEvent ("Pause");
		if (GameStartManager.isGameStarted ())
			ScoreManager.PauseGame ();
		Instance.paused = true;
	}

	public static void ResumeGame () {
		Instance.PausePanel.SetActive (false);
		EventManager.TriggerEvent ("Resume");
		if (GameStartManager.isGameStarted ())
			ScoreManager.ResumeGame ();
		Instance.paused = false;
	}
}
