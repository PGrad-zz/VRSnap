using UnityEngine;
using System.Collections;

public class SpecialIndicatorProvider : Singleton <SpecialIndicatorProvider> {
	private GameObject specialIndicator; //Icon made by Freepik (http://www.flaticon.com/authors/freepik) from www.flaticon.com (http://www.flaticon.com) is licensed by CC 3.0 BY (http://creativecommons.org/licenses/by/3.0/)
	private Transform playerHeadTransform;
	void Awake () {
		specialIndicator = Resources.Load ("SpecialIndicator") as GameObject;
		playerHeadTransform = GameObject.Find ("PlayerHead").transform;
	}

	public static GameObject getSpecialCopy (Transform goTransform, Material specialIconMat, float iconAbove, float scaler) {
		GameObject specialCopy = null;
		specialCopy = Instantiate (Instance.specialIndicator, Vector3.up * iconAbove + goTransform.position, goTransform.rotation) as GameObject;
		specialCopy.transform.GetChild (0).gameObject.GetComponent<MeshRenderer> ().material = specialIconMat;
		specialCopy.transform.parent = goTransform;
		specialCopy.transform.localScale *= scaler;
		return specialCopy;
	}

	public static void faceIconToPlayer (Transform specialObjectTransform) {
		specialObjectTransform.forward = Instance.playerHeadTransform.position - specialObjectTransform.position;
	}
}
