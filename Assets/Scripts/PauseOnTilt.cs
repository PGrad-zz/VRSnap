using UnityEngine;
using System.Collections;

public class PauseOnTilt : Singleton <PauseOnTilt> {
	public GameObject PausePanel,
					  TiltInstruction,
					  Monkey,
					  Frame;
	public static bool gameOver = false,
					   occupied = false,
					   testTiltPause = false;
	private readonly float tiltLimit = Mathf.Cos (Mathf.PI / 4);
	private bool monkeyMoved = false,
				 tilted = false,
				 paused = false;
	private float cosOfplayerHeadZAngle;

	void Update () {
		cosOfplayerHeadZAngle = Mathf.Cos (transform.eulerAngles.z * Mathf.PI / 180);
		if (!tilted && cosOfplayerHeadZAngle < tiltLimit) {
			tilted = true;
			if (!gameOver) {
				if (!paused) {
					if (!testTiltPause) {
						Destroy (Instance.TiltInstruction);
						testTiltPause = true;
					}
					PauseGame ();
				} else {
					ResumeGame ();
				}
			}
		}
		if (tilted && cosOfplayerHeadZAngle > tiltLimit) 
			tilted = false;
	}

	void OnApplicationPause (bool paused) {
		if (paused)
			PauseGame ();
	}

	public static void PauseGame () {
		if (!occupied) {
			Instance.PausePanel.SetActive (true);
			EventManager.TriggerEvent ("Pause");
			if (GameStartManager.isGameStarted ())
				ScoreManager.PauseGame ();
			Instance.paused = true;
		}
	}

	public static void ResumeGame () {
		if (!occupied) {
			/*if (testTiltPause && !Instance.monkeyMoved) {
				Instance.Monkey.transform.Translate (Vector3.up * 4.74f);
				Instance.Frame.SetActive (true);
				Instance.monkeyMoved = true;
			}*/
			Instance.PausePanel.SetActive (false);
			EventManager.TriggerEvent ("Resume");
			if (GameStartManager.isGameStarted ())
				ScoreManager.ResumeGame ();
			Instance.paused = false;
		}
	}
}
