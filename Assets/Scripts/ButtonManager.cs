using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonManager : MonoBehaviour {
	public GameObject button;
	private GameObject fogGO;
	private ParticleSystem fog;
	void Start () {
		fogGO = GameObject.Find ("Fog");
		if (fogGO != null)
			fog = fogGO.GetComponent<ParticleSystem> ();
	}

	public void startLevel() {
		if (fog != null)
			killFog ();
		Destroy (button);
		CameraReticle.shotsEnabled = true;
		GameObject.Find ("CameraReticle").GetComponent<CameraReticle> ().OnGazeExit (null, null);
		EventManager.TriggerEvent ("StartPlayerMove");
		EventManager.TriggerEvent ("Move");
	}

	public void killFog () {
		fog.startSize -= 10;
	}
}
