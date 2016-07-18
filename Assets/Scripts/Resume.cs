using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Resume : MonoBehaviour, IGvrGazeResponder, IPointerUpHandler {
	public void ResumeGame () {
		PauseOnTilt.ResumeGame ();
	}

	public void OnGazeTrigger () {
		PauseOnTilt.ResumeGame ();
	}

	public void OnPointerUp (PointerEventData ped) {
		PauseOnTilt.ResumeGame ();
	}

	public void OnGazeEnter () {
	}

	public void OnGazeExit () {
	}

	public void OnGazeStay () {
	}
}
