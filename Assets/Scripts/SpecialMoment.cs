using UnityEngine;
using System.Collections;

public abstract class SpecialMoment : MonoBehaviour, ISpecialInvoked {
	private float scaler,
				  iconAbove;
	protected Material specialIconMaterial;
	protected GameObject specialCopy = null;
	protected bool showing,
				   specialInvoked;

	protected void RegisterAndScale () {
		Vector3 colliderDims = GetComponent<Family> () == null ? GetComponent<Collider> ().bounds.size : GetComponentInChildren<Collider> ().bounds.size;
		EventManager.RegisterEvent ("Resume", ResumeSpecial);
		EventManager.RegisterEvent ("Pause", PauseSpecial);
		showing = false;
		specialInvoked = false;
		scaler = colliderDims.x * 1.5f;
		iconAbove = colliderDims.y;
	}

	protected virtual void Update () {
		if (showing) 
			SpecialIndicatorProvider.faceIconToPlayer (specialCopy);
	}

	void PauseSpecial () {
		if (showing) 
			specialCopy.SetActive (false);
	}

	void ResumeSpecial () {
		if (showing)
			specialCopy.SetActive (true);
	}

	protected void ShowSpecial () {
		if (specialCopy != null)
			specialCopy.SetActive (!specialInvoked);
		else 
			specialCopy = SpecialIndicatorProvider.getSpecialCopy (transform, specialIconMaterial, iconAbove, scaler);
		showing = !specialInvoked;
	}

	protected void HideSpecial () {
		showing = false;
		specialCopy.SetActive (false);
	}

	public virtual void SpecialIsInvoked () {
		specialInvoked = true;
		HideSpecial ();
		EventManager.NowInvoked (gameObject);
	}

	protected void SetSpecialIconMaterial (Material mat) {
		specialIconMaterial = mat;
	}
}
