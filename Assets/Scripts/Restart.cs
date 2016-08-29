using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

public class Restart : MonoBehaviour, IGvrGazeResponder, IPointerUpHandler  {
	private CameraShot camShot;

	void Start () {
		camShot = GameObject.Find("VR Main Camera").GetComponent<CameraShot> ();
	}

	public void RestartGame () {
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
		PauseOnTilt.gameOver = false;
		GameStartManager.GameRestarted ();
		camShot.ClearFrame ();
		PauseOnTilt.ResumeGame ();
		//PauseOnTilt.testTiltPause = false;
	}

	public void OnGazeTrigger () {
		RestartGame ();
	}

	public void OnPointerUp (PointerEventData ped) {
		RestartGame ();
	}
		
	public void OnGazeEnter () {
	}

	public void OnGazeExit () {
	}

	public void OnGazeStay () {
	}
}
