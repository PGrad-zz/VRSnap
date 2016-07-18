using UnityEngine;
using System.Collections;

public class PauseOnTilt : Singleton <PauseOnTilt> {
	public GameObject PausePanel;
	public static bool gameOver = false;
	private GameObject startButton;
	private readonly float tiltLimit = Mathf.Cos (Mathf.PI / 3);
	private bool tilted = false,
				 paused = false;
	private float cosOfplayerHeadZAngle;
	private GameObject jeep;

	void Start () {
		jeep = GameObject.Find ("jeepus");
		transform.forward = jeep.transform.forward;
		startButton = GameObject.Find ("StartButton");
	}

	void Update () {
		cosOfplayerHeadZAngle = Mathf.Cos (transform.eulerAngles.z * Mathf.PI / 180);
		if (!tilted && cosOfplayerHeadZAngle < tiltLimit) {
			tilted = true;
			if (!gameOver) {
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
		if (paused)
			PauseGame ();
	}

	public static void PauseGame () {
		ScoreManager.PauseGame ();
		Instance.PausePanel.SetActive (true);
		if (Instance.startButton != null)
			Instance.startButton.SetActive (false);
		Instance.paused = true;
	}

	public static void ResumeGame () {
		Instance.PausePanel.SetActive (false);
		if (Instance.startButton != null)
			Instance.startButton.SetActive (true);
		else
			ScoreManager.ResumeGame ();
		Instance.paused = false;
	}
}
