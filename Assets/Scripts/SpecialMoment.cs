using UnityEngine;
using System.Collections;

public class SpecialMoment : MonoBehaviour {
	public Material specialIconMaterial;
	public float iconAbove,
				 scaler;
	private GameObject specialCopy = null;
	private bool showing = false;

	void Update () {
		if (showing)
			SpecialIndicatorProvider.faceIconToPlayer (specialCopy.transform);
	}

	protected virtual void ShowSpecial () {
		if (specialCopy != null)
			specialCopy.SetActive (true);
		else 
			specialCopy = SpecialIndicatorProvider.getSpecialCopy (transform, specialIconMaterial, iconAbove, scaler);
		showing = true;
	}

	protected virtual void HideSpecial () {
		showing = false;
		specialCopy.SetActive (false);
	}
}
