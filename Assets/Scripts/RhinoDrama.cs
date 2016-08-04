using UnityEngine;
using System.Collections;

public class RhinoDrama : SpecialMoment {
	void Awake () {
		specialIconMaterial = Resources.Load ("SpecialIconMaterial/DramaMaterial") as Material;
	}

	void Start () {
		EventManager.RegisterGameObject (gameObject, Capture);
		base.RegisterAndScale ();
		ShowSpecial ();
	}

	public void Capture () {
		ScoreManager.AddSpecial ();
		SpecialIsInvoked ();
	}
}
