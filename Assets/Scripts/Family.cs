using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Family : SpecialMoment {
	private HashSet <GameObject> countMembers;
	private bool open = true;

	void Awake () {
		specialIconMaterial = (Material) Resources.Load ("SpecialIconMaterial/FamilyMaterial");
		GameObject go;
		countMembers = new HashSet<GameObject> ();
		for (int child = 0; child < transform.childCount; child++) {
			go = transform.GetChild (child).gameObject;
			if (EventManager.isPhotogenic (go) && !countMembers.Contains(go)) 
				countMembers.Add (go);
		}
	}

	void Start () {
		base.RegisterAndScale ();
		ShowSpecial ();
	}

	protected override void Update () {
		base.Update ();
		if (!open && CameraReticle.mapGOtoFacing.Count == 0) {
			BroadcastMessage ("Open");
			open = true;
		}
	}

	public void searchFamily () {
		showing = true;
		int totalHits = 0;
		bool isFacing;
		foreach (var item in countMembers) { 
			if (CameraReticle.mapGOtoFacing.TryGetValue (item, out isFacing) && isFacing)
				totalHits++;
		}
		if (totalHits == countMembers.Count) {
			ScoreManager.AddSpecial ();
			BroadcastMessage ("SpecialIsInvoked");
		}
		BroadcastMessage ("Searched");
		open = false;
	}

	public override void SpecialIsInvoked () {
		specialInvoked = true;
		HideSpecial ();
	}
}