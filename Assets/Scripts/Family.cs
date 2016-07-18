using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Family : MonoBehaviour {
	private HashSet <GameObject> countMembers;
	private bool open = true;
	void Awake () {
		GameObject go;
		countMembers = new HashSet<GameObject> ();
		for (int child = 0; child < transform.childCount; child++) {
			go = transform.GetChild (child).gameObject;
			if (EventManager.isPhotogenic (go) && !countMembers.Contains(go)) 
				countMembers.Add (go);
		}
	}

	void Update () {
		if (!open && CameraReticle.mapGOtoFacing.Count == 0) {
			BroadcastMessage ("Open");
			open = true;
		}
	}

	public void searchFamily () {
		int totalHits = 0;
		bool isFacing;
		foreach (var item in countMembers) { 
			if (CameraReticle.mapGOtoFacing.TryGetValue (item, out isFacing) && isFacing)
				totalHits++;
		}
		if (totalHits == countMembers.Count)
			ScoreManager.MultiplyScore (totalHits);
		BroadcastMessage ("Searched");
		open = false;
	}
}