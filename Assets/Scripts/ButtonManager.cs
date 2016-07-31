using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonManager : MonoBehaviour {
	public GameObject button;
	private GameObject fogGO;

	public void startLevel() {
		Destroy (button);
		CameraReticle.shotsEnabled = true;
		GameObject.Find ("CameraReticle").GetComponent<CameraReticle> ().OnGazeExit (null, null);
		EventManager.TriggerEvent ("Start");
	}
}
