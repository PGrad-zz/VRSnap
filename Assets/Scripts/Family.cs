using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Family : SpecialMoment {
	private HashSet <GameObject> countMembers;
	private bool open = true;

	void Awake () {
		specialIconMaterial = (Material) Resources.Load ("SpecialIconMaterial/FamilyMaterial");
	}

	public void Start () {
		CreateFamily ();
	}

	public void CreateFamily () {
		logFamily ();
		if (countMembers.Count != 0) {
			base.RegisterAndScale ();
			ShowSpecial ();
		}
	}

	protected override void Update () {
		base.Update ();
		if (!open && CameraReticle.mapGOtoFacing.Count == 0) {
			BroadcastMessage ("Open");
			open = true;
		}
	}

	public void logFamily () {
		GameObject go = null;
		FamilySentinel member = null;
		bool foundLead = false;
		countMembers = new HashSet<GameObject> ();
		for (int child = 0; child < transform.childCount; child++) {
			go = transform.GetChild (child).gameObject;
			member = go.GetComponent<FamilySentinel> ();
			if (EventManager.isPhotogenic (go) && member != null && !countMembers.Contains (go)) {
				countMembers.Add (go);
				if (member.lead) {
					if (foundLead)
						throw new UnityException ("There can be only one lead!");
					foundLead = true;	
					leadTransform = go.transform;
				}
			}
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